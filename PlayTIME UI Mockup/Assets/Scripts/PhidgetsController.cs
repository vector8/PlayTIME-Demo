using UnityEngine;
using System.Collections;
using Phidgets;

public class PhidgetsController : MonoBehaviour 
{
	private RFID reader;

	public GameObject startScreen;
	public GameObject actionSelector;
	public GameObject characteristicSelector;

	// Use this for initialization
	void Start () 
	{
		reader = new RFID();
		reader.open();
		reader.waitForAttachment(1000);

		reader.Tag += tagAdded;
		reader.TagLost += tagRemoved;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnGUI()
	{
		if(Event.current.Equals(Event.KeyboardEvent("r")))
		{
			reader.close();
			Application.LoadLevel("ActionStation");
		}
	}

	private void tagAdded(object sender, Phidgets.Events.TagEventArgs e)
	{
		startScreen.SetActive(false);
		actionSelector.SetActive(true);
		characteristicSelector.SetActive(false);
	}

	private void tagRemoved(object sender, Phidgets.Events.TagEventArgs e)
	{
		startScreen.SetActive(true);
		actionSelector.SetActive(false);
		characteristicSelector.SetActive(false);
	}
}
