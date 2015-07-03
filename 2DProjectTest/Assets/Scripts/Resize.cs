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

	private ITouchManager touchManager;
	private Vector4 originalExtents = new Vector4(); 
	private int topTouchID = -1, rightTouchID = -1, bottomTouchID = -1, leftTouchID = -1;

	// Use this for initialization
	void Start () 
	{
		touchManager = TouchManager.Instance;
		
		if(right == null)
		{
			print ("instantiating...");
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
			
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		if(sr != null)
		{
			Vector3 position = right.transform.position;
			position.x = sr.bounds.max.x;
			position.y = gameObject.transform.position.y;
			position.z = -5f;
			right.transform.position = position;
			originalExtents.x = position.x;
			position.x = sr.bounds.min.x;
			position.y = gameObject.transform.position.y;
			position.z = -5f;
			left.transform.position = position;
			originalExtents.z = position.x;

			position = top.transform.position;
			position.y = sr.bounds.max.y;
			position.x = gameObject.transform.position.x;
			position.z = -5f;
			top.transform.position = position;
			originalExtents.w = position.y;
			position.y = sr.bounds.min.y;
			position.x = gameObject.transform.position.x;
			position.z = -5f;
			bottom.transform.position = position;
			originalExtents.y = position.y;
		}

		top.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		right.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		bottom.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		left.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
	}
	
	// Update is called once per frame
	void Update () 
	{
		bool touched = false;

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
					
					top.transform.position = new Vector3(top.transform.position.x, ypos, top.transform.position.z);
					touched = true;
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
					
					right.transform.position = new Vector3(xpos, right.transform.position.y, right.transform.position.z);
					touched = true;
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
					ypos = Mathf.Min(ypos, gameObject.transform.position.y + 0.1f);
					
					bottom.transform.position = new Vector3(bottom.transform.position.x, ypos, bottom.transform.position.z);
					touched = true;
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
					xpos = Mathf.Min(xpos, gameObject.transform.position.x + 0.1f);
					
					left.transform.position = new Vector3(xpos, left.transform.position.y, left.transform.position.z);
					touched = true;
					break;
				}
			}
			
			if(!found)
			{
				leftTouchID = -1;
			}
		}

		if(touched)
		{
			Vector3 newPos = gameObject.transform.position;
			newPos.x = (right.transform.position.x + left.transform.position.x) / 2f;
			newPos.y = (top.transform.position.y + bottom.transform.position.y) / 2f;
			Vector3 difference = newPos - gameObject.transform.position;
			gameObject.transform.Translate(difference);
			top.transform.position = top.transform.position + new Vector3(0f, -difference.y, 0f);
			right.transform.position = right.transform.position + new Vector3(-difference.x, 0f, 0f);
			bottom.transform.position = bottom.transform.position + new Vector3(0f, -difference.y, 0f);
			left.transform.position = left.transform.position + new Vector3(-difference.x, 0f, 0f);

			Vector3 newScale = new Vector3(1f, 1f, 1f);
			newScale.x = (right.transform.position.x - left.transform.position.x) / (originalExtents.x - originalExtents.z);
			newScale.y = (top.transform.position.y - bottom.transform.position.y) / (originalExtents.w - originalExtents.y);

			gameObject.transform.localScale = newScale;
			newScale = new Vector3(1f / newScale.x, 1f / newScale.y, 1f);
			right.transform.localScale = newScale;
			left.transform.localScale = newScale;

			newScale = new Vector3(newScale.y, newScale.x, 1f);
			top.transform.localScale = newScale;
			bottom.transform.localScale = newScale;
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
}
