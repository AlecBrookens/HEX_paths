using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{
    private Rigidbody rb;
    //path selection enum
    public enum MovementPath { Square, Circle, FigureEight, Off };
    public MovementPath path;
    
    //radius for path sizes
    public float radius = 0.0f; 

    //Sphere speed (.25 to 1.5)
    public float speed = 0f;

    //how much lower the sphere is from the rig
    public float heightOffset = 0f;

    //boolean to render path trace or not
    public bool renderPath = false;

    //path trace resolution (square always 5)
    public int renderPathResolution = 0;

    //path trace width
    public float renderPathWidth = 0.0f;
    
    //path trace height
    public float renderPathHeight = 0.0f;

    public GameObject vrCamera;
    private float timeElapsed = 0.0f;
    private float SphereHeight = 0.0f;
    private LineRenderer lineRenderer;
    private List<Vector3> pathPositions = new List<Vector3>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 newPosition = new Vector3(2, SphereHeight, 2);
        transform.position = newPosition;
        AddPath(true);
        PositionUpdate();//its a feature not a bug i promise
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SphereHeight = vrCamera.transform.position.y + heightOffset;
        }
        PositionUpdate();
        
    }
    public void PositionUpdate()
    {
        timeElapsed += Time.deltaTime * speed;
        Vector3 newPosition = new Vector3(0.0f, SphereHeight, 0.0f);
        switch (path)
        {
            case MovementPath.Square:
                float t = timeElapsed % 4;  // This will give a value between 0 and 4
                if (t < 1)
                { // Bottom side (from left to right)
                    newPosition.x = Mathf.Lerp(-radius / 2, radius / 2, t);
                    newPosition.z = -radius / 2;
                }
                else if (t < 2)
                { // Right side (from bottom to top)
                    newPosition.x = radius / 2;
                    newPosition.z = Mathf.Lerp(-radius / 2, radius / 2, t - 1);
                }
                else if (t < 3)
                { // Top side (from right to left)
                    newPosition.x = Mathf.Lerp(radius / 2, -radius / 2, t - 2);
                    newPosition.z = radius / 2;
                }
                else
                { // Left side (from top to bottom)
                    newPosition.x = -radius / 2;
                    newPosition.z = Mathf.Lerp(radius / 2, -radius / 2, t - 3);
                }
                break;

            case MovementPath.Circle:
                newPosition.x = radius * Mathf.Cos(timeElapsed);
                newPosition.z = radius * Mathf.Sin(timeElapsed);
                break;

            case MovementPath.FigureEight:
                newPosition.x = radius * Mathf.Sin(timeElapsed);
                newPosition.z = radius * Mathf.Sin(timeElapsed) * Mathf.Cos(timeElapsed);
                break;
            case MovementPath.Off:
                newPosition.x = 0;
                newPosition.z = 0;
                newPosition.y = vrCamera.transform.position.y + heightOffset;
                break;
        }
        transform.position = newPosition;
    }//changes the sphere's position
    public void AddPath(Boolean create)
    {
        if (create)
        {
            if (renderPath)
            {

                lineRenderer = gameObject.AddComponent<LineRenderer>();

                lineRenderer.material = new Material(Shader.Find("Standard"));
                lineRenderer.positionCount = renderPathResolution + 2;
                lineRenderer.widthMultiplier = renderPathWidth;

                float curTheta = 0.0f;
                float stepSize = 2 * Mathf.PI / renderPathResolution;
                List<Vector3> pathPositions = new List<Vector3>();

                switch (path)
                {
                    case MovementPath.Circle:

                        while (curTheta <= 2 * Mathf.PI + 2 * stepSize)
                        {
                            Vector3 newPosition = new Vector3(0.0f, 0.0f, 0.0f);
                            newPosition.x = radius * Mathf.Cos(curTheta);
                            newPosition.y = renderPathHeight;
                            newPosition.z = radius * Mathf.Sin(curTheta);

                            pathPositions.Add(newPosition);

                            curTheta += stepSize;
                        }

                        lineRenderer.SetPositions(pathPositions.ToArray());

                        break;

                    case MovementPath.Square:
                        lineRenderer.positionCount = 5;
                        Vector3[] corners = new Vector3[] {
                                new Vector3(-radius / 2 , renderPathHeight, -radius / 2),
                                new Vector3(radius / 2, renderPathHeight, -radius / 2),
                                new Vector3(radius / 2, renderPathHeight, radius / 2),
                                new Vector3(-radius / 2, renderPathHeight, radius / 2),
                                new Vector3(-radius / 2, renderPathHeight, -radius / 2 )
                            };
                        lineRenderer.SetPositions(corners);
                        break;

                    case MovementPath.FigureEight:

                        while (curTheta <= 2 * Mathf.PI + 2 * stepSize)
                        {
                            Vector3 newPosition = new Vector3(0.0f, 0.0f, 0.0f);
                            newPosition.x = radius * Mathf.Sin(curTheta);
                            newPosition.y = renderPathHeight;
                            newPosition.z = radius * Mathf.Sin(curTheta) * Mathf.Cos(curTheta);

                            pathPositions.Add(newPosition);

                            curTheta += stepSize;
                        }

                        lineRenderer.SetPositions(pathPositions.ToArray());

                        break;
                }
            }
        }
        else
        {
            lineRenderer.enabled = true;
        }
    }//renders the path trace (true = create, false = reveal)
    public void RemovePath(Boolean destroy)
    {
        if (destroy)
        {
            Destroy(lineRenderer);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }//destroys the path trace (true = destroy, false = hide)
}