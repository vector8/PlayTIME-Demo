using UnityEngine;
using System.Collections;

public class AnimationAdded : MonoBehaviour 
{
	public GameObject plusButton;
	public GameObject hunterWalkPanel;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void onAddAnimation()
	{
		plusButton.transform.position = new Vector3(plusButton.transform.position.x + 166, plusButton.transform.position.y);
		hunterWalkPanel.SetActive (true);
	}
}
