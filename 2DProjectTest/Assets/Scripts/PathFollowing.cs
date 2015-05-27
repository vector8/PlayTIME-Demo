using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;

public class PathFollowing : MonoBehaviour 
{
    private List<Vector3> pathPoints = new List<Vector3>();             // Points on path
    private List<LineRenderer> pathRenderer = new List<LineRenderer>(); // Line renderers to draw the path. Multiple line renderers needed to avoid bends in line
    private GameObject pathRendererParent;                              // All line renderers are a child of this object

    enum PathState
    {
        Drawing = 0,     // User is drawing path
        Playing          // Object is moving along path
    };

    private PathState currentState = PathState.Drawing;

    public float minDist = 0.1f;   // The minimum distance a new point has to be from the previous point to be added to the pathPoint list

    private int currentPoint = 0;   // Index of point this gameobject is closest to on the path

    public float pathPlaybackTimeInSeconds = 5.0f;  // Time it takes to traverse through the path, in seconds
    public float currentPathPlayBackTime = 0.0f;

    public bool movableObject = false;  // Does this game object move along the path? Set to true if it does

	// Use this for initialization
	void Start () 
    {
        pathRendererParent = new GameObject("PathRenderer");

//         // Create a new line renderer and store a reference to it
//         GameObject go = new GameObject();
//         go.transform.parent = pathRendererParent.transform;
//         LineRenderer lr = gameObject.AddComponent<LineRenderer>();
//         pathRenderer.Add(lr);
// 
//         lr.SetVertexCount(2);
//         lr.SetWidth(0.10f, 0.10f);
//         lr.SetPosition(0, new Vector3(2, 2, -1));
//         lr.SetPosition(1, new Vector3(2, 4, -1));
// 
//         go = new GameObject();
//         go.transform.parent = pathRendererParent.transform;
//         lr = go.AddComponent<LineRenderer>();
//         lr.SetVertexCount(2);
//         lr.SetWidth(0.10f, 0.10f);
//         lr.SetPosition(0, new Vector3(2, 4, -1));
//         lr.SetPosition(1, new Vector3(4, 4, -1)	

    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyUp(KeyCode.Space))
            currentState = PathState.Playing;

        if (currentState == PathState.Drawing)
        {
            AddPointToPath();
        }
        else if (currentState == PathState.Playing && movableObject)
        {
            currentPathPlayBackTime += Time.deltaTime;

            // Return to start of path
            if (currentPathPlayBackTime >= pathPlaybackTimeInSeconds)
                currentPathPlayBackTime = 0.0f;

            float tStep = (pathPlaybackTimeInSeconds / (pathPoints.Count-1));

            currentPoint = (int)Mathf.Floor(currentPathPlayBackTime / tStep);

            Vector3 p1 = pathPoints[currentPoint];
            Vector3 p2 = pathPoints[currentPoint+1];
 
            float t1 = currentPoint * tStep;
            float t2 = (currentPoint+1) * tStep;

            float u = Mathf.Abs(1 - ((t2 - currentPathPlayBackTime) / (t2 - t1)));

            Debug.Log("t1 = " + t1 + " t2 = " + t2 + " u = " + u);
            transform.position = Vector3.Lerp(p1, p2, u);
        }
	}

    // Description:
    // Adds the current mouse position (in World Space) to the
    // pathPoints list. Before a new point is added to the list, we
    // test if it is far enough from the previously added point.
    void AddPointToPath()
    {
        Vector3 mouseWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (pathPoints.Count > 0)
        {
            Vector3 previousPoint = pathPoints[pathPoints.Count - 1];
            float dist = Vector3.Magnitude(mouseWorldSpace - previousPoint);

            // If this mouseWorldSpace is too close to the previously added point, 
            // do not add to pathPoints
            if (dist < minDist)
                return;
        }

        mouseWorldSpace.z = -1.0f; // to make line draw above everything else
        
        if (!movableObject)
            pathPoints.Add(mouseWorldSpace);
        else
            pathPoints.Add((new Vector3(0f, 10f, 0f)) + mouseWorldSpace); 

        if (!movableObject)
            UpdatePathRenderer();
    }

    // Description:
    // Updates the path renderer. Should only be called when a new point has
    // been added to the path.
    void UpdatePathRenderer()
    {
        // Not enough points to draw
        if (pathPoints.Count <= 1)
            return;

        // Create a new game object to attach the line renderer to
        // and store a reference to it
        GameObject go = new GameObject();
        go.transform.parent = pathRendererParent.transform;
        LineRenderer lr = go.AddComponent<LineRenderer>();
        pathRenderer.Add(lr);

        // Configure line renderer
        lr.SetVertexCount(2);
        lr.SetWidth(0.10f, 0.10f);
        lr.SetPosition(0, pathPoints[pathPoints.Count - 2]);
        lr.SetPosition(1, pathPoints[pathPoints.Count - 1]);
    }

}
