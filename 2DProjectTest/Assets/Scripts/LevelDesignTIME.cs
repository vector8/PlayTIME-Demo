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

    private Vector3 lastTouchPosition;

    // Reference to the board (Grid)
    private Completed.BoardManager grid;

    // Prevent UI from moving when using mouse as input
    public bool mouseMode = false;
    private bool lockPositions = false;
    private int lockedPosition;

	private bool removed = false;
	private bool dragging = true;
	private int lastGridIdx;
	private string keyToReset = "";

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

        if (mouseMode)
            MouseModeActiveText.SetActive(true);
        else
            MouseModeActiveText.SetActive(false);
		

		placeBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		replaceBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		removeBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		resetBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
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
            //PlacementUI.transform.FindChild("Place").gameObject.SetActive(true);
            PlacementUI.transform.FindChild("ReplaceRemove").gameObject.SetActive(false);
        }
        else
        {
            //PlacementUI.transform.FindChild("Place").gameObject.SetActive(false);
            PlacementUI.transform.FindChild("ReplaceRemove").gameObject.SetActive(true);
        }
    }

	// Update is called once per frame
	void Update ()
    {
		int gridIdx;

		if(touchManager.ActiveTouches.Count > 0)
		{
			if(!removed)
			{
				lastTouchPosition = touchManager.ActiveTouches[0].Position;
				if(!database[activeKey].first.activeSelf)
				{
					PlacementUI.SetActive(true);
					database[activeKey].first.SetActive(true);
					database[activeKey].second.SetActive(true);
					if(!grid.IsTileFreeAtIndex(grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, true)))
					{
						dragging = true;
						lastGridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, true);
					}
				}
				else
				{
					gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, true);
					if(dragging)
					{
						if(lastGridIdx != gridIdx)
						{
							database[activeKey].first.SetActive(false);
							database[activeKey].second.SetActive(false);
							string keyToActivate = "";
							string objectName = "";
							Dictionary<string, Pair<GameObject, GameObject>>.KeyCollection keys = database.Keys;
							for (int i = 0; i < grid.StaticPlacedTiles.Count; i++)
							{
								if (grid.StaticPlacedTiles[i].tileIdx == lastGridIdx)
								{
									objectName = grid.StaticPlacedTiles[i].go.name;

									foreach(KeyValuePair<string, Pair<GameObject, GameObject>> entry in database)
									{
										if(objectName == string.Format("{0}(Clone)", entry.Value.second.name))
										{
											keyToActivate = entry.Key;
											break;
										}
									}

									break;
								}
							}

							if(keyToActivate.Length > 0)
							{
								keyToReset = activeKey;
								activeKey = keyToActivate;
								database[activeKey].first.SetActive(true);
								database[activeKey].second.SetActive(true);
							}
							else
							{
								print("ERROR: could not find object " + objectName + " in database.");
							}

							grid.RemoveObjectAtIndex(lastGridIdx);
							dragging = false;
						}
					}
					
					Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
					database[activeKey].first.transform.position = new Vector3(wsTilePos.x, wsTilePos.y + 10, 1.0f);
					database[activeKey].second.transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
					UpdatePlacementUI(gridIdx);
				}
			}
		}
		else
		{
			if(database[activeKey].first.activeSelf)
			{
				PlaceObject(lastTouchPosition);
				database[activeKey].first.SetActive(false);
				database[activeKey].second.SetActive(false);
				PlacementUI.SetActive(false);
			}

			if(keyToReset.Length > 0)
			{
				activeKey = keyToReset;
				keyToReset = "";
			}

			removed = false;
			dragging = false;
		}

//        if (Input.GetKeyUp(KeyCode.C))
//        {
//            PlaceObject();
//        }
//
//        if (Input.GetKeyUp(KeyCode.V))
//        {
//            ReplaceObject();
//        }
//
//        if (Input.GetKeyUp(KeyCode.B))
//        {
//            RemoveObject();
//        }
//
//
//        // Toggle mouse mode
//        if (Input.GetKeyUp(KeyCode.M))
//        {
//            mouseMode = !mouseMode;
//            MouseModeActiveText.SetActive(mouseMode);
//        }

        // Cycle through the prefabs 
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
			rfidFound("0a00ec698c");
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
			grid.Revert();
        }
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

	public void PlaceObject(Vector2 position)
	{
		int gridIdx = 0;
		
		if (!mouseMode)
			gridIdx = grid.GetTileIndexInGridAtPoint(position, false);
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

		database[activeKey].first.SetActive(false);
		database[activeKey].second.SetActive(false);
		PlacementUI.transform.FindChild("ReplaceRemove").gameObject.SetActive(false);

		removed = true;
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
		//print ("button pressed - " + sender);
		GameObject s = ((Component) sender).gameObject;
		if(s.name.Equals(placeBtn.name))
		{
			//print("Placing object");
			PlaceObject();
		}
		else if(s.name.Equals(replaceBtn.name))
		{
			//print("Replacing object");
			ReplaceObject();
		}
		else if(s.name.Equals(removeBtn.name))
		{
			//print("Removing object");
			RemoveObject();
		}
		else if(s.name.Equals(resetBtn.name))
		{
			//print("Resetting game");
			grid.Revert();
		}
	}
}
