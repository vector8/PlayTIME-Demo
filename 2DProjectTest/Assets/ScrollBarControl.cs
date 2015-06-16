using UnityEngine;
using System;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;


public class ScrollBarControl : MonoBehaviour 
{
	public GameObject horizontalScrollbar;
	public GameObject verticalScrollbar;
	public GameObject resetBtn;
	public GameObject exitBtn;
	public GameObject cameraBtn;
	public GameObject cameraControlPanel;
	public float hScrollbarWidth;
	public float vScrollbarWidth;
	public float resetBtnOffset;
	public float exitBtnOffset;
	public float verticalBtnOffset;
	public Vector2 camBtnOffset;
	public Vector2 camPanelOffset;

	private Camera bottomCam;

	// Use this for initialization
	void Start () 
	{
		bottomCam = gameObject.GetComponent<Camera>();

		horizontalScrollbar.GetComponent<PanGesture>().Panned += scrollbarPannedHandler;
		verticalScrollbar.GetComponent<PanGesture>().Panned += scrollbarPannedHandler;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = bottomCam.transform.position;
		pos.z = 0;
		pos.x += (bottomCam.orthographicSize * bottomCam.aspect) - vScrollbarWidth / 2f;
		verticalScrollbar.transform.position = pos;
		Vector3 scale = verticalScrollbar.transform.localScale;
		scale.x = bottomCam.orthographicSize;
		verticalScrollbar.transform.localScale = scale;
		
		pos = bottomCam.transform.position;
		pos.z = 0;
		pos.y -= bottomCam.orthographicSize;
		pos.y += hScrollbarWidth / 2f;
		horizontalScrollbar.transform.position = pos;
		scale = horizontalScrollbar.transform.localScale;
		scale.x = bottomCam.orthographicSize * bottomCam.aspect;
		horizontalScrollbar.transform.localScale = scale;
		
		pos = bottomCam.transform.position;
		pos.z = 0;
		pos.x -= (bottomCam.orthographicSize * bottomCam.aspect) - resetBtnOffset;
		pos.y -= bottomCam.orthographicSize - verticalBtnOffset;
		resetBtn.transform.position = pos;
		
		pos = bottomCam.transform.position;
		pos.z = 0;
		pos.x += (bottomCam.orthographicSize * bottomCam.aspect) - exitBtnOffset;
		pos.y -= bottomCam.orthographicSize - verticalBtnOffset;
		exitBtn.transform.position = pos;

		pos = bottomCam.transform.position;
		pos.z = 0;
		pos.x += (bottomCam.orthographicSize * bottomCam.aspect) - camBtnOffset.x;
		pos.y += bottomCam.orthographicSize - camBtnOffset.y;
		cameraBtn.transform.position = pos;

		pos = bottomCam.transform.position;
		pos.z = 0;
		pos.x += (bottomCam.orthographicSize * bottomCam.aspect) - camPanelOffset.x;
		pos.y += bottomCam.orthographicSize - camPanelOffset.y;
		cameraControlPanel.transform.position = pos;
	}

	private void scrollbarPannedHandler(object sender, EventArgs e)
	{
		PanGesture gesture = (PanGesture) sender;
		GameObject s = gesture.gameObject;

		if(s.name.Equals(horizontalScrollbar.name))
		{
			float dx = gesture.ScreenPosition.x - gesture.PreviousScreenPosition.x;
			dx /= 50f;
			bottomCam.transform.Translate(-dx, 0, 0);
		}
		else if(s.name.Equals(verticalScrollbar.name))
		{
			float dy = gesture.ScreenPosition.y - gesture.PreviousScreenPosition.y;
			dy /= 50f;
			bottomCam.transform.Translate(0, -dy, 0);
		}
	}
}
