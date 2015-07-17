using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;

public class PathFollowing : MonoBehaviour 
{
    public List<Vector3> pathPoints = new List<Vector3>();             // Points on path
	public List<LineRenderer> pathRenderer = new List<LineRenderer>(); // Line renderers to draw the path. Multiple line renderers needed to avoid bends in line
	public GameObject pathRendererParent;                              // All line renderers are a child of this object

	public int touchID;
	public ITouchManager touchManager;

    public enum PathState
    {
        Drawing = 0,     // User is drawing path
        Playing,          // Object is moving along path
		Idle
    };

    public PathState currentState = PathState.Drawing;

    public float minDist = 0.4f;   // The minimum distance a new point has to be from the previous point to be added to the pathPoint list

    private int currentPoint = 0;   // Index of point this gameobject is closest to on the path

    public float pathSpeed = 5.0f;  // Speed to traverse through the path

    public bool movableObject = false;  // Does this game object move along the path? Set to true if it does

    public bool movingForward = true;   // True if positively iterating through pathPoints

    public float pathDrawTimeInSeconds = 3.0f;  // Time in seconds that the path is drawn for
    private float currentPathDrawTime = 0.0f;

    public Material pathRendererMaterial;

	// Use this for initialization
	void Start () 
    {
        pathRendererParent = new GameObject("PathRenderer");

		touchManager = TouchManager.Instance;

        // Init material for path renderer
        if (!movableObject)
        {
            pathRendererMaterial = new Material(Shader.Find("Particles/Additive"));
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
		if (currentState != PathState.Playing && (Input.GetKey(KeyCode.I) || (int) (Input.GetAxisRaw ("Horizontal")) != 0 || (int) (Input.GetAxisRaw ("Vertical")) != 0))
		{
			setStateToPlaying();
		}

        if (currentState == PathState.Drawing)
        {
            AddPointToPath();
        }
        else if (currentState == PathState.Playing && movableObject && pathPoints.Count > 0)
        {
            Vector3 p1, p2;
            float tStep, t1, t2, u;
            //tStep = (pathPlaybackTimeInSeconds / (pathPoints.Count - 1));

            if (movingForward)
            {
//                currentPathPlayBackTime += Time.deltaTime;
//
//                if (currentPathPlayBackTime >= pathPlaybackTimeInSeconds)
//                {
//                    movingForward = !movingForward;
//                    currentPathPlayBackTime = pathPlaybackTimeInSeconds;
//                    return; // avoid array out of range
//                }
//
//                currentPoint = (int)Mathf.Floor(currentPathPlayBackTime / tStep);

//                t1 = currentPoint * tStep;
//                t2 = (currentPoint + 1) * tStep;
                p1 = pathPoints[currentPoint];
                p2 = pathPoints[currentPoint + 1];
            }
            else
            {
//                currentPathPlayBackTime -= Time.deltaTime;
//
//                if (currentPathPlayBackTime <= 0.0f)
//                {
//                    movingForward = !movingForward;
//                    currentPathPlayBackTime = 0.0f;
//                    return;
//                }
//
//                currentPoint = (int)Mathf.Ceil(currentPathPlayBackTime / tStep);

//                t1 = currentPoint * tStep;
//                t2 = (currentPoint - 1) * tStep;
                p1 = pathPoints[currentPoint];
                p2 = pathPoints[currentPoint - 1]; 
            }

//			u = Mathf.Abs(1 - ((t2 - currentPathPlayBackTime) / (t2 - t1)));

			Vector3 position;

			Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
			if(rb != null && !rb.isKinematic)
			{
				p2 = new Vector3(p2.x, transform.position.y, p2.z);
			}
				
			position = Vector3.MoveTowards(transform.position, p2, Time.deltaTime * pathSpeed);
       		transform.position = position;
			
			if((position - p2).magnitude < 0.05f)
			{
				if(movingForward)
				{
					if(currentPoint < pathPoints.Count - 1)
					{
						currentPoint++;
					}

					if(currentPoint == pathPoints.Count - 1)
					{
						movingForward = !movingForward;
					}
				}
				else
				{
					if(currentPoint > 0)
					{
						currentPoint--;
					}

					if(currentPoint == 0)
					{
						movingForward = !movingForward;
					}
				}
			}
            //Debug.Log("t1 = " + t1 + " t2 = " + t2 + " u = " + u + " time = " + currentPathPlayBackTime);
        }
        else if(pathRendererParent.activeSelf)
        {
            currentPathDrawTime += Time.deltaTime;

            if (currentPathDrawTime >= pathDrawTimeInSeconds)
                pathRendererParent.SetActive(false);
        }
	}

    // Description:
    // Adds the current mouse position (in World Space) to the
    // pathPoints list. Before a new point is added to the list, we
    // test if it is far enough from the previously added point.
    void AddPointToPath()
    {
		Vector3 mouseWorldSpace = new Vector3();
		bool mousePosSet = false;
		for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
		{
			if(touchManager.ActiveTouches[i].Id == touchID)
			{
				mouseWorldSpace = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position);
				mousePosSet = true;
				break;
			}
		}

		if(!mousePosSet)
		{
			setStateToIdle();
			return;
		}

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
			pathPoints.Add((new Vector3(0f, LevelManager.SCREEN_GAP, 0f)) + mouseWorldSpace); 

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
        //lr.material = pathRendererMaterial;
    }

	public PathFollowing initDrawing(int id, bool movingObject)
	{
		touchID = id;
		movableObject = movingObject;

		return this;
	}

	public PathFollowing setStateToIdle()
	{
		currentState = PathState.Idle;
		currentPoint = 0;
		
		Animator anim = GetComponent<Animator>();
		if(anim != null)
		{
			anim.SetBool("HorizontalMovement", false);
		}
		
		return this;
	}

	public PathFollowing setStateToPlaying()
	{
		currentState = PathState.Playing;
		currentPoint = 0;

		Animator anim = GetComponent<Animator>();
		if(anim != null)
		{
			anim.SetBool("HorizontalMovement", true);
		}

		return this;
	}

	void OnDestroy()
	{
		DestroyImmediate(pathRendererParent);
	}

    // Meant to be called when this game object is touched
	public PathFollowing startDrawingPath()
    {
        currentPathDrawTime = 0.0f;
        if (pathRendererParent)
			pathRendererParent.SetActive(true);
		
		return this;
    }

	public bool isEmpty()
	{
		return pathPoints.Count == 0;
	}
}
