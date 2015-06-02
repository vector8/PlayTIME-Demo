using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Phidgets;

public class PhidgetsController : MonoBehaviour 
{
	private RFID reader;
	private bool tagFound = false;
	private bool tagPresent = false;
    private Dictionary<string, GameObject> database = new Dictionary<string, GameObject>();
	private GameObject currentSelectors;

	public GameObject startScreen;
	public GameObject selectorsPrefab;

	// Use this for initialization
	void Start () 
	{
		reader = new RFID();
		reader.Tag += tagAdded;
		reader.TagLost += tagRemoved;
		reader.Attach += rfidAttached;
		reader.Detach += rfidDetached;
		reader.Error += rfidError;
		reader.open();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(tagFound && !tagPresent)
		{
			tagPresent = true;
			startScreen.SetActive(false);
			if(database.ContainsKey(reader.LastTag))
			{
				database.TryGetValue(reader.LastTag, out currentSelectors);
				currentSelectors.SetActive(true);
			}
			else
			{
				currentSelectors = Instantiate<GameObject>(selectorsPrefab);
				database.Add(reader.LastTag, currentSelectors);
			}
			print(reader.LastTag);
		}
		else if(!tagFound && tagPresent)
		{
			tagPresent = false;
			startScreen.SetActive(true);
			currentSelectors.SetActive(false);
		}
	}

	public void clickThroughToNextScreen()
	{
		startScreen.SetActive(false);
		currentSelectors = Instantiate<GameObject>(selectorsPrefab);
	}
	
	void OnGUI()
	{
//		if(Event.current.Equals(Event.KeyboardEvent("r")))
//		{
//			reader.close();
//			Application.LoadLevel("ActionStation");
//		}
	}

	private void rfidError(object sender, Phidgets.Events.ErrorEventArgs e)
	{
		print("ERROR: " + e.Description);
	}

	private void rfidAttached(object sender, Phidgets.Events.AttachEventArgs e)
	{
		print ("RFID reader attached.");
		print("Name: " + reader.Name);

		print(reader.Antenna);
	}
	
	private void rfidDetached(object sender, Phidgets.Events.DetachEventArgs e)
	{
		print("RFID reader detached.");	
	}

	private void tagAdded(object sender, Phidgets.Events.TagEventArgs e)
	{
		print("Tag found!");
		print("Tag: " + e.Tag);

		tagFound = true;
	}

	private void tagRemoved(object sender, Phidgets.Events.TagEventArgs e)
	{
		print("Tag removed!");

		tagFound = false;
	}
}
