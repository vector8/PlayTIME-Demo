using UnityEngine;
using System.Collections;

public class ActionSelectionScript : MonoBehaviour 
{
	private GameObject draggingAction;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(draggingAction != null)
		{
			draggingAction.transform.position = Input.mousePosition;
		}
	}

	public void onPointerDown (GameObject source)
	{
		draggingAction = source;
	}

	public void onPointerUp()
	{
		draggingAction = null;
	}
}
