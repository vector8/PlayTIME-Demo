using UnityEngine;
using System;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class Resize : MonoBehaviour 
{
	public GameObject arrowPrefab;
	public GameObject nonStatic;
	public GameObject top, right, bottom, left;
    public Vector2 originalDimensions = new Vector2(); 

	private ITouchManager touchManager;
	private int topTouchID = -1, rightTouchID = -1, bottomTouchID = -1, leftTouchID = -1;

    private void initialize()
    {
        right = GameObject.Instantiate(arrowPrefab);
        right.transform.parent = gameObject.transform;
        right.SetActive(true);
        bottom = GameObject.Instantiate(right);
        bottom.transform.parent = gameObject.transform;
        bottom.transform.Rotate(0f, 0f, -90f);
        left = GameObject.Instantiate(right);
        left.transform.parent = gameObject.transform;
        left.transform.Rotate(0f, 0f, -180f);
        top = GameObject.Instantiate(right);
        top.transform.parent = gameObject.transform;
        top.transform.Rotate(0f, 0f, -270f);
    }

    private void calculateOriginalDimensions()
    {
        originalDimensions.x = (right.transform.position.x - left.transform.position.x) / gameObject.transform.localScale.x;
        originalDimensions.y = (top.transform.position.y - bottom.transform.position.y) / gameObject.transform.localScale.y;
    }

	// Use this for initialization
	void Start () 
	{
		touchManager = TouchManager.Instance;
		
		if(right == null)
		{
            initialize();
		}
			
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		if(sr != null)
		{
			Vector3 position = right.transform.position;
			position.x = sr.bounds.max.x;
			position.y = gameObject.transform.position.y;
			position.z = -5f;
			right.transform.position = position;
			position.x = sr.bounds.min.x;
			position.y = gameObject.transform.position.y;
			position.z = -5f;
			left.transform.position = position;

			position = top.transform.position;
			position.y = sr.bounds.max.y;
			position.x = gameObject.transform.position.x;
			position.z = -5f;
			top.transform.position = position;
			position.y = sr.bounds.min.y;
			position.x = gameObject.transform.position.x;
			position.z = -5f;
			bottom.transform.position = position;
		}

		top.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		right.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		bottom.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		left.GetComponent<PressGesture>().Pressed += buttonPressedHandler;

        calculateOriginalDimensions();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float yDiffTop = 0f, yDiffBot = 0f, xDiffRight = 0f, xDiffLeft = 0f;

		if(topTouchID > -1)
		{
			bool found = false;
			
			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
			{
				if(touchManager.ActiveTouches[i].Id == topTouchID)
				{
					found = true;
					
					float ypos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position).y;
					ypos = Mathf.Max(ypos, gameObject.transform.position.y + 0.1f);

					yDiffTop = (ypos - top.transform.position.y);
					break;
				}
			}
			
			if(!found)
			{
				topTouchID = -1;
			}
		}

		if(rightTouchID > -1)
		{
			bool found = false;
			
			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
			{
				if(touchManager.ActiveTouches[i].Id == rightTouchID)
				{
					found = true;
					
					float xpos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position).x;
					xpos = Mathf.Max(xpos, gameObject.transform.position.x + 0.1f);
					
					xDiffRight = (xpos - right.transform.position.x);
					break;
				}
			}
			
			if(!found)
			{
				rightTouchID = -1;
			}
		}

		if(bottomTouchID > -1)
		{
			bool found = false;
			
			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
			{
				if(touchManager.ActiveTouches[i].Id == bottomTouchID)
				{
					found = true;
					
					float ypos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position).y;
					ypos = Mathf.Min(ypos, gameObject.transform.position.y - 0.1f);
					
					yDiffBot = (ypos - bottom.transform.position.y);
					break;
				}
			}
			
			if(!found)
			{
				bottomTouchID = -1;
			}
		}

		if(leftTouchID > -1)
		{
			bool found = false;
			
			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
			{
				if(touchManager.ActiveTouches[i].Id == leftTouchID)
				{
					found = true;
					
					float xpos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position).x;
					xpos = Mathf.Min(xpos, gameObject.transform.position.x - 0.1f);
					
					xDiffLeft = (xpos - left.transform.position.x);
					break;
				}
			}
			
			if(!found)
			{
				leftTouchID = -1;
			}
		}

		if(!Mathf.Approximately(yDiffTop, 0f) || !Mathf.Approximately(yDiffBot, 0f) ||
		   !Mathf.Approximately(xDiffRight, 0f) || !Mathf.Approximately(xDiffLeft, 0f))
		{
			Vector3 newScale = new Vector3(1f, 1f, 1f);
			newScale.x = ((xDiffRight - xDiffLeft) + right.transform.position.x - left.transform.position.x) / originalDimensions.x;
			newScale.y = ((yDiffTop - yDiffBot) + top.transform.position.y - bottom.transform.position.y) / originalDimensions.y;

            setNewScale(newScale);

			Vector3 newpos = gameObject.transform.position;
			newpos.x += 0.5f * (xDiffRight + xDiffLeft);
			newpos.y += 0.5f * (yDiffTop + yDiffBot);
			gameObject.transform.position = newpos;

			// Transform top-screen object to reflect the bottom-screen object
			Vector3 pos	= gameObject.transform.position;
			pos.y += LevelManager.SCREEN_GAP;
			nonStatic.transform.position = pos;
		}
	}

	private void buttonPressedHandler(object sender, EventArgs e)
	{
		PressGesture gesture = (PressGesture) sender;
		GameObject s = gesture.gameObject;
		if(s.Equals(top))
		{
			topTouchID = touchManager.ActiveTouches[touchManager.ActiveTouches.Count - 1].Id;
		}
		else if(s.Equals(right))
		{
			rightTouchID = touchManager.ActiveTouches[touchManager.ActiveTouches.Count - 1].Id;
		}
		else if(s.Equals(bottom))
		{
			bottomTouchID = touchManager.ActiveTouches[touchManager.ActiveTouches.Count - 1].Id;
		}
		else if(s.Equals(left))
		{
			leftTouchID = touchManager.ActiveTouches[touchManager.ActiveTouches.Count - 1].Id;
		}
	}

    public void setNewScale(Vector3 newScale)
    {
        if (right == null)
        {
            initialize();
        }

        gameObject.transform.localScale = newScale;
        newScale = new Vector3(1f / newScale.x, 1f / newScale.y, 1f);
        right.transform.localScale = newScale;
        left.transform.localScale = newScale;

        newScale = new Vector3(newScale.y, newScale.x, 1f);
        top.transform.localScale = newScale;
        bottom.transform.localScale = newScale;

        nonStatic.transform.localScale = gameObject.transform.localScale;
    }
}
