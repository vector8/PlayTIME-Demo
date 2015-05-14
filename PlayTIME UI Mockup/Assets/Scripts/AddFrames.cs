using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddFrames : MonoBehaviour 
{
	private int currentFrame = 0;
	private Vector2 originalPos = new Vector3(0, 0);
	public GameObject[] frames;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void onClick(GameObject button)
	{
		if(originalPos.x == 0)
		{
			originalPos = button.transform.position;
		}

		if(currentFrame < frames.Length)
		{
			frames[currentFrame].SetActive(true);
			if(currentFrame < frames.Length - 1)
			{
				button.transform.position = new Vector3(button.transform.position.x + 166, button.transform.position.y);
			}
			else
			{
				button.SetActive(false);
			}
			currentFrame++;
		}
	}

	public void reset(GameObject button)
	{
		for (int i = 0; i < frames.Length; i++) 
		{
			frames[i].SetActive(false);
		}

		currentFrame = 0;
		button.transform.position = originalPos;
		button.SetActive (true);
	}
}
