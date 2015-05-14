using UnityEngine;
using System.Collections;

public class UIControlScript : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void goToNextScreen(GameObject nextScreen)
	{
		nextScreen.SetActive (true);
		gameObject.SetActive (false);
	}

	public void instantiateScreen(GameObject prefab)
	{
		GameObject screenClone = (GameObject)Instantiate (prefab);
		screenClone.SetActive (true);
		gameObject.SetActive (false);
	}

	public void destroySelf()
	{
		Destroy (gameObject);
	}
}
