using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionScript : MonoBehaviour
{
	private ArrayList selectedActions = new ArrayList();
	private ArrayList deactivatedButtons = new ArrayList();
	
	public Button currentTab;
	public GameObject currentPanel;
	public GameObject selectedActionsPanel;

	public int xspacing;
	public int xoffset;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void tabSelected(Button newTab)
	{
		currentTab.interactable = true;
		newTab.interactable = false;
		
		currentTab = newTab;
	}

	public void tabSelected(GameObject newPanel)
	{
		currentPanel.SetActive(false);
		newPanel.SetActive(true);

		currentPanel = newPanel;

		for(int i = 0; i < selectedActions.Count; i++)
		{
			((GameObject)selectedActions[i]).SetActive(true);
		}
	}

	public void buttonClicked(GameObject button)
	{
		if(selectedActions.Contains(button))
		{
			for(int i = 0; i < deactivatedButtons.Count; i++)
			{
				if(button.tag.Equals(((GameObject)deactivatedButtons[i]).tag))
				{
					((GameObject)deactivatedButtons[i]).SetActive(true);
					deactivatedButtons.RemoveAt(i);
					break;
				}
			}

			selectedActions.Remove(button);

			for(int i = 0; i < selectedActions.Count; i++)
			{
				((GameObject)selectedActions[i]).transform.position = getNewActionPosition(i);
			}
			
			Destroy(button);
		}
		else
		{
			GameObject instance = (GameObject)Instantiate(button);
			instance.transform.SetParent(selectedActionsPanel.transform);
			instance.transform.position = getNewActionPosition(selectedActions.Count);
			selectedActions.Add(instance);
			button.SetActive(false);
			deactivatedButtons.Add(button);
		}
		//print (button.transform.position.y);

//		if(button.transform.position.y < 345)
//		{
//			button.transform.Translate(new Vector3(0, 265, 0));
//		}
//		else
//		{
//			button.transform.Translate(new Vector3(0, -265, 0));
//		}
	}

	private Vector3 getNewActionPosition(int count)
	{
		return new Vector3(xoffset + count%6 * xspacing, 616 - count/6 * 69);
	}
}
