using UnityEngine;
using System;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class CameraControl : MonoBehaviour 
{
	public GameObject topCamera;
	public GameObject[] panBtns;
	public GameObject[] zoomBtns;
	public GameObject cameraOutline;
	public GameObject targetBtn;

	public float panSpeed;
	public float zoomSpeed;
	
	private ITouchManager touchManager;
	private LevelManager levelManager;
	
	private bool[] panBtnsPressed = {false, false, false, false};
	private bool[] zoomBtnsPressed = {false, false};
	private bool targetBtnPressed = false;

	private const float OUTLINE_WIDTH = 19.2f;
	private const float OUTLINE_HEIGHT = 10.8f;

	private Vector3 originalTargetBtnPosition;
	private int targetBtnTouchID = -1;
	private GameObject followTarget = null;

	// Use this for initialization
	void Start () 
	{
		touchManager = TouchManager.Instance;
		levelManager = LevelManager.instance;
		
		for(int i = 0; i < panBtns.Length; i++)
		{
			panBtns[i].GetComponent<PressGesture>().Pressed += buttonPressedHandler;
			panBtns[i].GetComponent<ReleaseGesture>().Released += buttonReleasedHandler;
		}
		for(int i = 0; i < zoomBtns.Length; i++)
		{
			zoomBtns[i].GetComponent<PressGesture>().Pressed += buttonPressedHandler;
			zoomBtns[i].GetComponent<ReleaseGesture>().Released += buttonReleasedHandler;
		}

		targetBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		targetBtn.GetComponent<ReleaseGesture>().Released += buttonReleasedHandler;
		originalTargetBtnPosition = targetBtn.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 camPos = topCamera.transform.position;

		if(followTarget != null)
		{
			camPos.x = followTarget.transform.position.x;
			camPos.y = followTarget.transform.position.y;
		}
		else
		{
			if(panBtnsPressed[0])
			{
				camPos.y += panSpeed * Time.deltaTime;
			}
			if(panBtnsPressed[1])
			{
				camPos.x += panSpeed * Time.deltaTime;
			}
			if(panBtnsPressed[2])
			{
				camPos.y -= panSpeed * Time.deltaTime;
			}
			if(panBtnsPressed[3])
			{
				camPos.x -= panSpeed * Time.deltaTime;
			}
		}
			
		topCamera.transform.position = camPos;

		Camera cam = topCamera.GetComponent<Camera>();
		float camSize = cam.orthographicSize;
		if(zoomBtnsPressed[0])
		{
			camSize -= camSize * Time.deltaTime;
		}
		if(zoomBtnsPressed[1])
		{
			camSize += camSize * Time.deltaTime;
		}
		cam.orthographicSize = camSize;

		// Set the outline to the camera's position and size
		Vector3 outlinePos = camPos;
		outlinePos.y -= 10;
		outlinePos.z = 0;
		cameraOutline.transform.position = outlinePos;
		Vector3 scale = new Vector3();
		scale.y = (camSize * 2) / OUTLINE_HEIGHT;
		scale.x = (camSize * 2 * cam.aspect) / OUTLINE_WIDTH;
		cameraOutline.transform.localScale = scale;

		// Set position of target icon if it is being dragged
		if(targetBtnPressed && targetBtnTouchID > -1)
		{
			Vector3 targetPos = new Vector3();
			bool found = false;
			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
			{
				if(touchManager.ActiveTouches[i].Id == targetBtnTouchID)
				{
					targetPos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position);
					found = true;
					break;
				}
			}

			if(found)
			{
				targetPos.z = 0;
				targetBtn.transform.position = targetPos;
			}
		}
	}

	private void buttonPressedHandler(object sender, EventArgs e)
	{
		GameObject s = ((Component) sender).gameObject;
		//print (s.name);
		for(int i = 0; i < panBtns.Length; i++)
		{
			if(s.name.Equals(panBtns[i].name))
			{
				panBtnsPressed[i] = true;
				followTarget = null;
				return;
			}
		}

		for(int i = 0; i < zoomBtns.Length; i++)
		{
			if(s.name.Equals(zoomBtns[i].name))
			{
				zoomBtnsPressed[i] = true;
				return;
			}
		}

		if(s.name.Equals(targetBtn.name))
		{
			targetBtnPressed = true;
			followTarget = null;
			int max = -1;
			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
			{
				if(touchManager.ActiveTouches[i].Id > max)
				{
					max = touchManager.ActiveTouches[i].Id;
				}
			}

			targetBtnTouchID = max;
			return;
		}
	}

	private void buttonReleasedHandler(object sender, EventArgs e)
	{
		GameObject s = ((Component) sender).gameObject;

		for(int i = 0; i < panBtns.Length; i++)
		{
			if(s.name.Equals(panBtns[i].name))
			{
				panBtnsPressed[i] = false;
				return;
			}
		}
		
		for(int i = 0; i < zoomBtns.Length; i++)
		{
			if(s.name.Equals(zoomBtns[i].name))
			{
				zoomBtnsPressed[i] = false;
				return;
			}
		}
		
		if(s.name.Equals(targetBtn.name))
		{
			targetBtnPressed = false;
			targetBtnTouchID = -1;
			
			if(levelManager.isObjectAtPosition(targetBtn.transform.position))
			{
				followTarget = levelManager.getObjectAtPosition(targetBtn.transform.position).first;
			}
			else
			{
				followTarget = null;
			}

			targetBtn.transform.position = originalTargetBtnPosition;
			return;
		}
	}
}
