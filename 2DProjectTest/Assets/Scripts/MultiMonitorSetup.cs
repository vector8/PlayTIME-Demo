using UnityEngine;
using System.Collections;

public class MultiMonitorSetup : MonoBehaviour 
{
	public Vector2 topScreenResolution;
	public Vector2 bottomScreenResolution;

	private Camera cam;
	private Vector3 pos;
	
	// Use this for initialization
	void Start () 
	{
		Screen.SetResolution((int)Mathf.Min(topScreenResolution.x, bottomScreenResolution.x), 
		                     (int)topScreenResolution.y + (int)bottomScreenResolution.y, 
		                     false);

		cam = GetComponent<Camera>();
		if(bottomScreenResolution.y > 0)
		{
			cam.orthographicSize = 5 * (topScreenResolution.y / bottomScreenResolution.y + 1f);
		}
		pos = gameObject.transform.position;
		pos.y = -1.5f + cam.orthographicSize;
		gameObject.transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
