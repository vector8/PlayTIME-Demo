using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;

public class LevelDesignTIME : MonoBehaviour 
{
    // Fake database
    // string is the key (rfid), GameObject is the data (prefab)
    public Dictionary<string, GameObject> database = new Dictionary<string, GameObject>();
    public GameObject[] prefabs;

    public GameObject PlacementUI;
    public GameObject MouseModeActiveText;
    public Vector2 PlacementUIOffsetInPixels;

	public GameObject placeBtn, replaceBtn, removeBtn;

	private ITouchManager touchManager;

    private Vector3 LastMousePosition;

    // Reference to the board (Grid)
    private Completed.BoardManager grid;

    // Prevent UI from moving when using mouse as input
    public bool mouseMode = false;
    private bool lockPositions = false;
    private int lockedPosition;

    private bool previewMode = false;

    // Current or latest rfid tag read.
    // Empty string means none active
    public string activeKey = "";

	// Use this for initialization
	void Start () 
    {
		// Add things to the data base
        // Key will be replaced with rfid values
        // Instantiated them for displaying purposes only
		// Enemy
        GameObject go = Instantiate(prefabs[0]);
        database.Add("4d004aef91", go);
        go.SetActive(false);
		// Soda
		go = Instantiate(prefabs[1]);
		database.Add("4d004ab4ee", go);
		go.SetActive(false);
		// Wall
		go = Instantiate(prefabs[2]);
		database.Add("4d004aa4ee", go);
		go.SetActive(false);

        grid = Completed.GameManager.instance.GetBoardScript();

		touchManager = TouchManager.Instance;

		activeKey = "4d004aef91";
        //database[activeKey].SetActive(true);
        //PlacementUI.SetActive(false);
        LastMousePosition = Input.mousePosition;

        if (mouseMode)
            MouseModeActiveText.SetActive(true);
        else
            MouseModeActiveText.SetActive(false);
		

		placeBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		replaceBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		removeBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;

//		GetComponent<PressGesture>().Pressed += pressedHandler;
//		GetComponent<ReleaseGesture>().Released += releasedHandler;
	}

    // Description:
    // Updates the UI position and displays the correct
    // menu for the input index in the grid.
    private void UpdatePlacementUI(int gridIdx, bool updatePosition = true)
    {
        // Update position
        if (updatePosition)
        {
            Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
            //PlacementUI.transform.position = Camera.main.WorldToScreenPoint(new Vector3(wsTilePos.x, wsTilePos.y, 1.0f)) + new Vector3(PlacementUIOffsetInPixels.x, PlacementUIOffsetInPixels.y, 0.0f);
			PlacementUI.transform.position = new Vector3(wsTilePos.x, wsTilePos.y, -1.0f);
        }

        // Display correct menu
        if (grid.IsTileFreeAtIndex(gridIdx))
        {
            PlacementUI.transform.FindChild("Place").gameObject.SetActive(true);
            PlacementUI.transform.FindChild("ReplaceRemove").gameObject.SetActive(false);
        }
        else
        {
            PlacementUI.transform.FindChild("Place").gameObject.SetActive(false);
            PlacementUI.transform.FindChild("ReplaceRemove").gameObject.SetActive(true);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        int gridIdx = grid.GetTileIndexInGridAtPoint(Input.mousePosition, true);

//        if (gridIdx > 0)
//        {
//            if (mouseMode)
//            {
//                // Lock the ui position so it doesnt move when we try to click it
//                if (Input.GetMouseButtonUp(0))
//                {
//                    lockPositions = !lockPositions;
//                    PlacementUI.SetActive(lockPositions);
//                    UpdatePlacementUI(gridIdx, lockPositions);
//                    lockedPosition = gridIdx;
//                }
//
//                if (!lockPositions)
//                {
//                    Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
//                    database[activeKey].transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
//                }
//            }
//            else // Touch mode
//            {

				database[activeKey].SetActive(touchManager.ActiveTouches.Count > 0);
				PlacementUI.SetActive(touchManager.ActiveTouches.Count > 0);
				
				if(database[activeKey].activeSelf)
				{
					//print (touchManager.ActiveTouches[0].Id + " - " + touchManager.ActiveTouches[0].Position);
       				gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, true);
					//print (gridIdx);
					Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
					database[activeKey].transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
					UpdatePlacementUI(gridIdx);
				}
           // }

            if (Input.GetKeyUp(KeyCode.C))
            {
                PlaceObject();
            }

            if (Input.GetKeyUp(KeyCode.V))
            {
                ReplaceObject();
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                RemoveObject();
            }


            // Toggle mouse mode
            if (Input.GetKeyUp(KeyCode.M))
            {
                mouseMode = !mouseMode;
                MouseModeActiveText.SetActive(mouseMode);
            }
       // }

        // Cycle through the prefabs 
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            database[activeKey].SetActive(false);
//            activeKey++;
//            activeKey = activeKey % prefabs.Length;
            database[activeKey].SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            previewMode = !previewMode;
            if (previewMode)
            {
                grid.SetRevert();
            }
            else
            {
                grid.Revert();
            }
        }

        LastMousePosition = Input.mousePosition;
	}

    public void PlaceObject()
    {
        int gridIdx = 0;

        if (!mouseMode)
			gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, false);
        else
            gridIdx = lockedPosition;

        // Check if grid spot is free
        if (grid.IsTileFreeAtIndex(gridIdx))
        {
            grid.PlaceObjectAtIndex(gridIdx, database[activeKey], this.transform);
        }
        else
        {
            // Ask user if they want to replace
            Debug.Log("Not free");
        }
    }

    public void RemoveObject()
    {
        int gridIdx = 0;

        if (!mouseMode)
			gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, false);
        else
            gridIdx = lockedPosition;

        grid.RemoveObjectAtIndex(gridIdx);
    }

    public void ReplaceObject()
    {
        int gridIdx = 0;

        if (!mouseMode)
			gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, false);
        else
            gridIdx = lockedPosition;

        grid.ReplaceObjectAtIndex(gridIdx, database[activeKey], this.transform);
    }

    public void rfidFound(string key)
    {
		database[activeKey].SetActive(false);
		activeKey = key;
		database[activeKey].SetActive(true);
    }

	private void buttonPressedHandler(object sender, EventArgs e)
	{
		print ("button pressed - " + sender);
		GameObject s = ((Component) sender).gameObject;
		if(s.name.Equals(placeBtn.name))
		{
			print("Placing object");
			PlaceObject();
		}
		else if(s.name.Equals(replaceBtn.name))
		{
			print("Replacing object");
			ReplaceObject();
		}
		else if(s.name.Equals(removeBtn.name))
		{
			print("Removing object");
			RemoveObject();
		}
	}

	private void pressedHandler(object sender, EventArgs e)
	{
		print("pressed - " + ((PressGesture)sender).ActiveTouches.Count + " - " + ((PressGesture)sender).ActiveTouches[0].Position);
//		for(int i = 0; i < ((PressGesture)sender).ActiveTouches.Count; i++)
//		{
//			touchManager.ActiveTouches.Add(((PressGesture)sender).ActiveTouches[i]);
//		}
	}
	
	private void releasedHandler(object sender, EventArgs e)
	{
//		print(((ReleaseGesture)sender).ActiveTouches.Count);
//		if(touchManager.ActiveTouches.Count == 0)
//		{
//			touchManager.ActiveTouches.Clear();
//		}
//		else
//		{
//			for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
//			{
//				bool found = false;
//				for(int j = 0; j < touchManager.ActiveTouches.Count; j++)
//				{
//					if(touchManager.ActiveTouches[i].Id == touchManager.ActiveTouches[j].Id)
//					{
//						found = true;
//						break;
//					}
//				}
//				
//				if(!found)
//				{
//					touchManager.ActiveTouches.RemoveAt(i);
//				}
//			}
//		}
	}
}
