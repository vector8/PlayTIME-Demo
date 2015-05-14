using UnityEngine;
using System.Collections;

public class SelectionScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void buttonClicked(GameObject button)
	{
		if(button.transform.position.y < 345)
		{
			button.transform.Translate(new Vector3(0, 265, 0));
		}
		else
		{
			button.transform.Translate(new Vector3(0, -265, 0));
		}
	}
}
