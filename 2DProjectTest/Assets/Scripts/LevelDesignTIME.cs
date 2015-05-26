﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;

public class LevelDesignTIME : MonoBehaviour 
{
    // Fake database
    // string is the key (rfid), GameObject is the data (prefab)
	public Dictionary<string, Pair<GameObject, GameObject>> database = new Dictionary<string, Pair<GameObject, GameObject>>();
	public GameObject[] prefabs;
    public GameObject[] staticPrefabs;

    public GameObject PlacementUI;
    public GameObject MouseModeActiveText;
    public Vector2 PlacementUIOffsetInPixels;

	public GameObject placeBtn, replaceBtn, removeBtn, resetBtn;
	
	// Current or latest rfid tag read.
	// Empty string means none active
	public string activeKey = "";

	private ITouchManager touchManager;

    private Vector3 LastMousePosition;

    // Reference to the board (Grid)
    private Completed.BoardManager grid;

    // Prevent UI from moving when using mouse as input
    public bool mouseMode = false;
    private bool lockPositions = false;
    private int lockedPosition;

    private bool previewMode = false;
	
	private List<GameObject> objects;
	private List<GameObject> staticObjects;

	// Use this for initialization
	void Start () 
    {
		// Add things to the data base
        // Key will be replaced with rfid values
        // Instantiated them for displaying purposes only
		// Enemy
		Pair<GameObject, GameObject> p = new Pair<GameObject, GameObject>(Instantiate(prefabs[0]), Instantiate(staticPrefabs[0]));
		p.first.SetActive(false);
		p.second.SetActive(false);
        database.Add("4d004aef91", p);
		// Soda
		p = new Pair<GameObject, GameObject>(Instantiate(prefabs[1]), Instantiate(staticPrefabs[1]));
		p.first.SetActive(false);
		p.second.SetActive(false);
		database.Add("4d004ab4ee", p);
		// Wall
		p = new Pair<GameObject, GameObject>(Instantiate(prefabs[2]), Instantiate(staticPrefabs[2]));
		p.first.SetActive(false);
		p.second.SetActive(false);
		database.Add("4d004aa4ee", p);
		// Player
		p = new Pair<GameObject, GameObject>(Instantiate(prefabs[3]), Instantiate(staticPrefabs[3]));
		p.first.SetActive(false);
		p.second.SetActive(false);
		database.Add("0a00ec698c", p);

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
		resetBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;

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
		int gridIdx;

		database[activeKey].first.SetActive(touchManager.ActiveTouches.Count > 0);
		database[activeKey].second.SetActive(touchManager.ActiveTouches.Count > 0);
		PlacementUI.SetActive(touchManager.ActiveTouches.Count > 0);
		
		if(database[activeKey].first.activeSelf)
		{
			//print (touchManager.ActiveTouches[0].Id + " - " + touchManager.ActiveTouches[0].Position);
			gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, true);
			//print (gridIdx);
			Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
			database[activeKey].first.transform.position = new Vector3(wsTilePos.x, wsTilePos.y + 10, 1.0f);
			database[activeKey].second.transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
			UpdatePlacementUI(gridIdx);
		}

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

        // Cycle through the prefabs 
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
			rfidFound("0a00ec698c");
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
			grid.Revert();
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
			grid.PlaceObjectAtIndex(gridIdx, database[activeKey].first, database[activeKey].second, this.transform);
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

		grid.ReplaceObjectAtIndex(gridIdx, database[activeKey].first, database[activeKey].second, this.transform);
    }

    public void rfidFound(string key)
    {
		database[activeKey].first.SetActive(false);
		database[activeKey].second.SetActive(false);
		activeKey = key;
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
		else if(s.name.Equals(resetBtn.name))
		{
			print("Resetting game");
			grid.Revert();
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
