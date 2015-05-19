using UnityEngine;
using System.Collections;

public class ResetScene : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnGUI()
	{
		if(Event.current.Equals(Event.KeyboardEvent("r")))
		{
			Application.LoadLevel("ActionStation");
		}
	}
}
