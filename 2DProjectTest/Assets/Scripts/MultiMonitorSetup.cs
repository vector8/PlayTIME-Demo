using UnityEngine;
using System.Collections;

public class MultiMonitorSetup : MonoBehaviour 
{
	public Vector2 topScreenResolution;
	public Vector2 bottomScreenResolution;
	public Camera topCam;
	
	private Camera bottomCam;
	private Vector3 pos;
	
	// Use this for initialization
	void Start () 
	{
		float totalYResolution = topScreenResolution.y + bottomScreenResolution.y;
		Screen.SetResolution((int)Mathf.Min(topScreenResolution.x, bottomScreenResolution.x), 
		                     (int)totalYResolution, 
		                     false);
		
		bottomCam = GetComponent<Camera>();

		Rect rect = bottomCam.rect;
		rect.height = bottomScreenResolution.y / totalYResolution;
		bottomCam.rect = rect;

		rect = topCam.rect;
		rect.height = topScreenResolution.y / totalYResolution;
		rect.y = 1f - rect.height;
		topCam.rect = rect;

//		if(bottomScreenResolution.y > 0)
//		{
//			cam.orthographicSize = 5 * (topScreenResolution.y / bottomScreenResolution.y + 1f);
//		}
//		pos = gameObject.transform.position;
//		pos.y = -1.5f + cam.orthographicSize;
//		gameObject.transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
