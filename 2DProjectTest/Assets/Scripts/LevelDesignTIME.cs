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

	public GameObject pathBtn, replaceBtn, removeBtn, resetBtn, exitBtn, sliderGroup, sliderTab;
	
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

	private int sliderTouchID = -1;
	private const float SLIDER_MAX_X = 1.6455f;
	private const float SLIDER_MIN_X = -1.6455f;

	private Pair<GameObject, GameObject> lastObjectSelected = null;

	private string pendingKey = "";

	// Use this for initialization
	void Start () 
    {
		// Add things to the data base
        // Key will be replaced with rfid values
        // Instantiated them for displaying purposes only
		// Enemy
//		Pair<GameObject, GameObject> p = new Pair<GameObject, GameObject>(Instantiate(prefabs[0]), Instantiate(staticPrefabs[0]));
//		p.first.SetActive(false);
//		p.second.SetActive(false);
//		database.Add("4d004aef91", p);
//		// Soda
//		p = new Pair<GameObject, GameObject>(Instantiate(prefabs[1]), Instantiate(staticPrefabs[1]));
//		p.first.SetActive(false);
//		p.second.SetActive(false);
//		database.Add("4d004ab4ee", p);
//		// Wall
//		p = new Pair<GameObject, GameObject>(Instantiate(prefabs[2]), Instantiate(staticPrefabs[2]));
//		p.first.SetActive(false);
//		p.second.SetActive(false);
//		database.Add("4d004aa4ee", p);
//		// Player
//		p = new Pair<GameObject, GameObject>(Instantiate(prefabs[3]), Instantiate(staticPrefabs[3]));
//		p.first.SetActive(false);
//		p.second.SetActive(false);
//		database.Add("0a00ec698c", p);

        grid = Completed.GameManager.instance.GetBoardScript();

		touchManager = TouchManager.Instance;

		//activeKey = "0a00ec698c";
        //database[activeKey].SetActive(true);
        //PlacementUI.SetActive(false);

        if (mouseMode)
            MouseModeActiveText.SetActive(true);
        else
            MouseModeActiveText.SetActive(false);
		

		pathBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		replaceBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		removeBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		resetBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		exitBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		sliderTab.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
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
            PlacementUI.transform.Find("ReplaceRemove").gameObject.SetActive(false);
            PlacementUI.transform.Find("ReplaceRemove/PathBtn").gameObject.SetActive(false);
        }
        else
        {
            PlacementUI.transform.Find("ReplaceRemove").gameObject.SetActive(true);
            PlacementUI.transform.Find("ReplaceRemove/PathBtn").gameObject.SetActive(false);
			sliderGroup.SetActive(false);
			
            for (int i = 0; i < grid.PlacedTiles.Count; i++)
            {
                if (grid.PlacedTiles[i].tileIdx == gridIdx)
                {
                    PathFollowing p = grid.StaticPlacedTiles[i].go.GetComponent<PathFollowing>();
                    if (p != null)
                    {
                        p.startDrawingPath();
                    }

					p = grid.PlacedTiles[i].go.GetComponent<PathFollowing>();
					if(p != null)
					{
           				PlacementUI.transform.Find("ReplaceRemove/PathBtn").gameObject.SetActive(true);

						if(p.currentState == PathFollowing.PathState.Playing)
						{
							sliderGroup.SetActive(true);
						}
						else if(p.currentState == PathFollowing.PathState.Idle)
						{
							p.setStateToPlaying();
							lastObjectSelected = new Pair<GameObject, GameObject>(grid.PlacedTiles[i].go, grid.StaticPlacedTiles[i].go);

							// Set slider tab to correct position
							float xpos = ((1f - (p.pathPlaybackTimeInSeconds - 1f) / 8f) * 5.6f) - 2.8f;
							sliderTab.transform.localPosition = new Vector3(xpos, sliderTab.transform.localPosition.y, 0f);
						}
					}

                    break;
                }
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
		int gridIdx;

		if(touchManager.ActiveTouches.Count > 0)
		{
			gridIdx = grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, true);
			if(gridIdx < 0)
				return;

			if(sliderTouchID > -1)
			{
				bool found = false;

				for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
				{
					if(touchManager.ActiveTouches[i].Id == sliderTouchID)
					{
						found = true;

						//print("FOUND THE TOUCH!!");

						float xpos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position).x;
						xpos = Mathf.Max(Mathf.Min(xpos, sliderGroup.transform.position.x + SLIDER_MAX_X), sliderGroup.transform.position.x + SLIDER_MIN_X);

						sliderTab.transform.position = new Vector3(xpos, sliderTab.transform.position.y, 0f);

						float newPlaybackTimeSec = (1 - (sliderTab.transform.localPosition.x + 2.8f) / 5.6f) * 8f + 1f;
						for (int j = 0; j < grid.StaticPlacedTiles.Count; j++)
						{
							if (grid.StaticPlacedTiles[j].tileIdx == gridIdx)
							{
								PathFollowing p = grid.PlacedTiles[j].go.GetComponent<PathFollowing>();
								if (p != null)
								{
									p.pathPlaybackTimeInSeconds = newPlaybackTimeSec;
									p.currentPathPlayBackTime = 0f;
									break;
								}
							}
						}
						
						break;
					}
				}
				
				if(!found)
				{
					sliderTouchID = -1;
				}
			}

			if(!removed)
			{
				lastTouchPosition = touchManager.ActiveTouches[0].Position;
				if(activeKey != "" && !database[activeKey].first.activeSelf)
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
							lastObjectSelected = null;
							dragging = false;
						}
					}

					if(activeKey != "")
					{
						Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
						database[activeKey].first.transform.position = new Vector3(wsTilePos.x, wsTilePos.y + 10, 1.0f);
						database[activeKey].second.transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
						UpdatePlacementUI(gridIdx);
					}
				}
			}
		}
		else
		{
			if(activeKey != "" && database[activeKey].first.activeSelf)
			{
				PlaceObject(lastTouchPosition);
				database[activeKey].first.SetActive(false);
				database[activeKey].second.SetActive(false);
				PlacementUI.SetActive(false);

				if(lastObjectSelected != null)
				{
					lastObjectSelected.first.transform.position = lastObjectSelected.second.transform.position + (new Vector3(0f, 10f, 0f));

					PathFollowing p = lastObjectSelected.first.GetComponent<PathFollowing>();
					if (p != null)
					{
						p.setStateToIdle();
					}

					lastObjectSelected = null;
				}
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
			print("rfidFound calling...");
			rfidFound("4d004aef91");
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
            print("Not free");
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
		if (gridIdx > -1 && grid.IsTileFreeAtIndex(gridIdx))
		{
			grid.PlaceObjectAtIndex(gridIdx, database[activeKey].first, database[activeKey].second, this.transform);
		}
		else
		{
			// Ask user if they want to replace
			print("Not free");
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
		PlacementUI.transform.Find("ReplaceRemove").gameObject.SetActive(false);
		lastObjectSelected = null;
		
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
		lastObjectSelected = null;
    }

    public void rfidFound(string key)
    {
		if(database.ContainsKey(key))
		{
			if(activeKey != "")
			{
				database[activeKey].first.SetActive(false);
				database[activeKey].second.SetActive(false);
			}
			activeKey = key;
		}
		else
		{
			pendingKey = key;

			string url = "http://localhost/playtime/getComponents.php?rfidKey=" + pendingKey;
			
			print("fetching key " + pendingKey);
			
			StartCoroutine(pollDatabase(url));
		}
    }

	private IEnumerator pollDatabase(string url)
	{
		WWW www = new WWW(url);
		WWWForm form = new WWWForm();
		
		yield return www;
		
		string[] delimiters = {"<br>"};
		string[] results = www.text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
		if(results.Length == 0)
		{
			print ("No components were found for RFID key " + pendingKey + " in the database.");
		}
		else
		{
			if(activeKey != "")
			{
				database[activeKey].first.SetActive(false);
				database[activeKey].second.SetActive(false);
			}
			activeKey = pendingKey;

			Pair<GameObject, GameObject> p = new Pair<GameObject, GameObject>();
			
			for(int i = 0; i < results.Length; i++)
			{
				print(results[i]);
				string[] delim = {","};
				string[] vals = results[i].Split(delim, StringSplitOptions.None);
				addComponentByName(p.first, vals[0], false, vals[1], vals[2]);
				addComponentByName(p.second, vals[0], true, vals[1], vals[2]);
			}

			p.first.SetActive(false);
			p.second.SetActive(false);
			database.Add(activeKey, p);
		}

		pendingKey = "";
	}

	private void addComponentByName(GameObject go, string name, bool isStatic, string data1, string data2)
	{
		switch(name)
		{
		case "PathFollowing":
			go.AddComponent<PathFollowing>().initDrawing(0, !isStatic).setStateToIdle();
			break;
		case "SpriteRenderer":
			go.AddComponent<SpriteRenderer>();
			SpriteRenderer r = go.GetComponent<SpriteRenderer>();
			if(data1.Length > 0)
			{
				r.sprite = Resources.Load<Sprite>(data1);
			}
			if(data2.Length > 0)
			{
				r.sortingLayerName = data2;
			}
			break;
		case "Animator":
			go.AddComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(data1);
			break;
		default:
			break;
		}
	}

	private void buttonPressedHandler(object sender, EventArgs e)
	{
		//print ("button pressed - " + sender);
		GameObject s = ((Component) sender).gameObject;
		if(s.name.Equals(pathBtn.name))
		{
			//print("Placing object");
			//PlaceObject();
			// 

            // Get GameObject at touch location and add a PathFollowing component to it
            int gridIdx = (grid.GetTileIndexInGridAtPoint(touchManager.ActiveTouches[0].Position, false));
            for (int i = 0; i < grid.PlacedTiles.Count; i++)
            {
                if (grid.PlacedTiles[i].tileIdx == gridIdx)
                {
					PathFollowing p = grid.StaticPlacedTiles[i].go.GetComponent<PathFollowing>();
					if(p != null)
					{
						DestroyImmediate(p);
						p = null;

						p = grid.PlacedTiles[i].go.GetComponent<PathFollowing>();
						DestroyImmediate(p);
					}

					grid.PlacedTiles[i].go.AddComponent<PathFollowing>().initDrawing(touchManager.ActiveTouches[touchManager.ActiveTouches.Count-1].Id, true);
					grid.StaticPlacedTiles[i].go.AddComponent<PathFollowing>().initDrawing(touchManager.ActiveTouches[touchManager.ActiveTouches.Count-1].Id, false);

					break;
				}
            }

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
		else if(s.name.Equals(sliderTab.name))
		{
			sliderTouchID = touchManager.ActiveTouches[touchManager.ActiveTouches.Count - 1].Id;
		}
		else if(s.name.Equals(resetBtn.name))
		{
			//print("Resetting game");
			grid.Revert();
		}
		else if(s.name.Equals(exitBtn.name))
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
