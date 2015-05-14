using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationSelection : MonoBehaviour 
{
	public Collider2D trashcan;
	public GameObject plusSign;
	public GameObject hunterWalkPanel;
	private GameObject selectedAnimation;
	private Vector3 startingPosition;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(selectedAnimation != null)
		{
			selectedAnimation.transform.position = Input.mousePosition;
		}
	}

	public void onPointerDown (GameObject source)
	{
		selectedAnimation = source;
		startingPosition = source.transform.position;
	}

	public void onPointerUp()
	{
		if (trashcan.bounds.Intersects(selectedAnimation.GetComponent<Collider2D>().bounds))
		{
			selectedAnimation.SetActive (false);
			plusSign.transform.position = new Vector3(plusSign.transform.position.x - 166, plusSign.transform.position.y);
			hunterWalkPanel.transform.position = new Vector3(hunterWalkPanel.transform.position.x - 166, hunterWalkPanel.transform.position.y);
		} 
		else 
		{
			print(trashcan.bounds.ToString());
			print(selectedAnimation.GetComponent<Collider2D>().bounds.ToString());
			selectedAnimation.transform.position = startingPosition;
		}

		selectedAnimation = null;
	}
}
