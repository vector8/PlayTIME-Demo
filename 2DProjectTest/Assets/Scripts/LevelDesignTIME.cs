using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;

public class LevelDesignTIME : MonoBehaviour 
{
	// Real database
	public string databaseAddress;

    // Local database to cache objects
    // string is the key (rfid), GameObject is the data (prefab)
	public Dictionary<string, Pair<GameObject, GameObject>> database = new Dictionary<string, Pair<GameObject, GameObject>>();
	public GameObject[] prefabs;
    public GameObject[] staticPrefabs;

    public GameObject PlacementUI;
    public Vector2 PlacementUIOffsetInPixels;

	public GameObject pathBtn, replaceBtn, removeBtn, resetBtn, exitBtn, sliderGroup, sliderTab, cameraBtn, cameraPanel, cameraOutline;
	public GameObject horizontalScrollbar, verticalScrollbar;
	
	// Current or latest rfid tag read.
	// Empty string means none active
	public string activeKey = "";

	private ITouchManager touchManager;
	private LevelManager levelManager;

    private Vector3 lastTouchPosition;
	private Vector3 dragStartPosition;

	private bool removed = false;
	private bool dragging = true;
	private Pair<GameObject, GameObject> draggingObject = null;
	//private int lastGridIdx;
	private string keyToReset = "";
	private bool shouldResetKey = false;

    private bool previewMode = false;
	
	private int sliderTouchID = -1;
	private const float SLIDER_MAX_X = 1.6455f;
	private const float SLIDER_MIN_X = -1.6455f;

	private Pair<GameObject, GameObject> lastObjectSelected = null;

	private string pendingKey = "";

	private string[] testRFIDKeys = {"4d004aef91", "4d004ab4ee", "4d004aa4ee", "0a00ec698c"};
	private int testIndex = -1;

	void Awake()
	{
		gameObject.AddComponent<LevelManager>();
		levelManager = LevelManager.instance;
	}

	// Use this for initialization
	void Start () 
    {
		touchManager = TouchManager.Instance;
		
		pathBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		replaceBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		removeBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		resetBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		exitBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		sliderTab.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		cameraBtn.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		horizontalScrollbar.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
		verticalScrollbar.GetComponent<PressGesture>().Pressed += buttonPressedHandler;
	}

    // Description:
    // Updates the UI position and displays the correct
    // menu for the input index in the grid.
    private void UpdatePlacementUI(Vector2 touchPosition, bool updatePosition = true)
    {
        // Update position
        if (updatePosition)
        {
			PlacementUI.transform.position = new Vector3(touchPosition.x, touchPosition.y, -1.0f);
        }

        // Display correct menu
		if(!levelManager.isObjectAtPosition(touchPosition))
        {
            PlacementUI.transform.Find("ReplaceRemove").gameObject.SetActive(false);
            PlacementUI.transform.Find("ReplaceRemove/PathBtn").gameObject.SetActive(false);
        }
        else
        {
            PlacementUI.transform.Find("ReplaceRemove").gameObject.SetActive(true);
            PlacementUI.transform.Find("ReplaceRemove/PathBtn").gameObject.SetActive(false);
			PlacementUI.transform.Find("ReplaceRemove/ReplaceBtn").gameObject.SetActive(activeKey != "");
			sliderGroup.SetActive(false);
			
			Pair<GameObject, GameObject> selectedObject = levelManager.getObjectAtPosition(touchPosition);
			PathFollowing p = selectedObject.second.GetComponent<PathFollowing>();
            if (p != null)
            {
                p.startDrawingPath();
            }

			p = selectedObject.first.GetComponent<PathFollowing>();
			if(p != null)
			{
   				PlacementUI.transform.Find("ReplaceRemove/PathBtn").gameObject.SetActive(true);

				if(!p.isEmpty())
				{
					sliderGroup.SetActive(true);
				}

				if(p.currentState == PathFollowing.PathState.Idle)
				{
					p.setStateToPlaying();
					lastObjectSelected = selectedObject;

					// Set slider tab to correct position
					float xpos = ((1f - (p.pathPlaybackTimeInSeconds - 1f) / 8f) * 5.6f) - 2.8f;
					sliderTab.transform.localPosition = new Vector3(xpos, sliderTab.transform.localPosition.y, 0f);
				}
			}
        }
    }

	// Update is called once per frame
	void Update ()
    {
		if(cameraPanel.activeSelf)
			return;

		if(touchManager.ActiveTouches.Count > 0)
		{
			Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[0].Position);

			if(sliderTouchID > -1)
			{
				bool found = false;

				for(int i = 0; i < touchManager.ActiveTouches.Count; i++)
				{
					if(touchManager.ActiveTouches[i].Id == sliderTouchID)
					{
						found = true;

						float xpos = Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[i].Position).x;
						xpos = Mathf.Max(Mathf.Min(xpos, sliderGroup.transform.position.x + SLIDER_MAX_X), sliderGroup.transform.position.x + SLIDER_MIN_X);

						sliderTab.transform.position = new Vector3(xpos, sliderTab.transform.position.y, 0f);

						float newPlaybackTimeSec = (1 - (sliderTab.transform.localPosition.x + 2.8f) / 5.6f) * 8f + 1f;
						Pair<GameObject, GameObject> selectedObject = levelManager.getObjectAtPosition(touchPosition);
						if(selectedObject != null)
						{
							PathFollowing p = selectedObject.first.GetComponent<PathFollowing>();
							if (p != null)
							{
								p.pathPlaybackTimeInSeconds = newPlaybackTimeSec;
								p.currentPathPlayBackTime = 0f;
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
				lastTouchPosition = touchPosition;
				if(activeKey == "" && !PlacementUI.activeSelf || activeKey != "" && !database[activeKey].first.activeSelf)
				{
					PlacementUI.SetActive(true);

					if(activeKey != "")
					{
						database[activeKey].first.SetActive(true);
						database[activeKey].second.SetActive(true);
					}

					if(levelManager.isObjectAtPosition(touchPosition))
					{
						dragging = true;
						dragStartPosition = touchPosition;
					}
				}
				else
				{
					if(dragging)
					{
						print (Vector2.Distance(touchPosition, dragStartPosition));
						if(Vector2.Distance(touchPosition, dragStartPosition) > 0.1f)
						{
							if(activeKey != "")
							{
								database[activeKey].first.SetActive(false);
								database[activeKey].second.SetActive(false);
							}

							draggingObject = levelManager.getObjectAtPosition(dragStartPosition);

							keyToReset = activeKey;
							activeKey = "";
							shouldResetKey = true;
							dragging = false;
						}
					}

					if(draggingObject != null)
					{
						draggingObject.first.transform.position = new Vector3(touchPosition.x, touchPosition.y + LevelManager.SCREEN_GAP, 1.0f);
						draggingObject.second.transform.position = new Vector3(touchPosition.x, touchPosition.y, 1.0f);
					}
					else if(activeKey != "")
					{
						database[activeKey].first.transform.position = new Vector3(touchPosition.x, touchPosition.y + LevelManager.SCREEN_GAP, 1.0f);
						database[activeKey].second.transform.position = new Vector3(touchPosition.x, touchPosition.y, 1.0f);
					}
					UpdatePlacementUI(touchPosition);
				}
			}
		}
		else
		{
			PlacementUI.SetActive(false);
			
			if(activeKey != "" && database[activeKey].first.activeSelf)
			{
				PlaceObject(lastTouchPosition);
				database[activeKey].first.SetActive(false);
				database[activeKey].second.SetActive(false);
			}

			if(lastObjectSelected != null)
			{
				lastObjectSelected.first.transform.position = lastObjectSelected.second.transform.position + (new Vector3(0f, LevelManager.SCREEN_GAP, 0f));

				PathFollowing p = lastObjectSelected.first.GetComponent<PathFollowing>();
				if (p != null)
				{
					p.setStateToIdle();
				}

				lastObjectSelected = null;
			}
			
			if(shouldResetKey)
			{
				activeKey = keyToReset;
				keyToReset = "";
				shouldResetKey = false;
			}

			draggingObject = null;
			removed = false;
			dragging = false;
		}

        // Cycle through the prefabs 
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
			testIndex++;
			if(testIndex >= testRFIDKeys.Length)
			{
				testIndex = 0;
			}
			rfidFound(testRFIDKeys[testIndex]);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
			levelManager.revert();
        }
	}

	public void PlaceObject(Vector2 position)
	{
		// Check if spot is free
		if (!levelManager.isObjectAtPosition(position))
		{
			levelManager.placeObject(position, database[activeKey].first, database[activeKey].second, this.transform);
		}
		else
		{
			// Ask user if they want to replace
			print("Not free");
		}
	}
	
	public void RemoveObject()
	{
		levelManager.removeObject(Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[0].Position));
		removePlacementUI();
    }

	private void removePlacementUI()
	{
		
		if(activeKey != "")
		{
			database[activeKey].first.SetActive(false);
			database[activeKey].second.SetActive(false);
		}
		
		PlacementUI.transform.Find("ReplaceRemove").gameObject.SetActive(false);
		lastObjectSelected = null;
		removed = true;
	}

    public void ReplaceObject()
    {
		levelManager.replaceObject(Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[0].Position), database[activeKey].first, database[activeKey].second, this.transform);
		lastObjectSelected = null;
    }

    public void rfidFound(string key)
    {
		if(activeKey != "")
		{
			database[activeKey].first.SetActive(false);
			database[activeKey].second.SetActive(false);
			activeKey = "";
		}

		if(database.ContainsKey(key))
		{
			activeKey = key;
		}
		else
		{
			pendingKey = key;

			string url = "http://" + databaseAddress + "/playtime/getComponents.php";
			
			print("fetching key " + pendingKey);
			
			StartCoroutine(pollDatabase(url));
		}
    }

	private IEnumerator pollDatabase(string url)
	{
		WWWForm form = new WWWForm();

		form.AddField("rfidKey", pendingKey);
		
		WWW www = new WWW(url, form);

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
				p.first.name = vals[0];
				p.second.name = vals[0];
				addComponentByName(p.first, vals[1], false, vals[2], vals[3], vals[4], vals[5]);
				addComponentByName(p.second, vals[1], true, vals[2], vals[3], vals[4], vals[5]);
			}

			p.first.SetActive(false);
			p.second.SetActive(false);
			database.Add(activeKey, p);
		}

		pendingKey = "";
	}

	private void addComponentByName(GameObject go, string name, bool isStatic, string data1, string data2, string data3, string data4)
	{
		go.layer = LayerMask.NameToLayer("BlockingLayer");
		switch(name)
		{
		case "PathFollowing":
			go.AddComponent<PathFollowing>().initDrawing(0, !isStatic).setStateToIdle();
			break;
		case "SpriteRenderer":
		{
			SpriteRenderer r = go.AddComponent<SpriteRenderer>();
			if(data1.Length > 0)
			{
				int index = data1.LastIndexOf('_');
				if(index > 0)
				{
					int spriteIndex;
					if(Int32.TryParse(data1.Substring(index + 1), out spriteIndex))
					{
						string path = data1.Substring(0, index);
						Sprite[] sprites = Resources.LoadAll<Sprite>(path);
						if(spriteIndex < sprites.Length)
						{
							r.sprite = sprites[spriteIndex];
						}
					}
				}
				else
				{
					r.sprite = Resources.Load<Sprite>(data1);
				}
			}
			if(data2.Length > 0)
			{
				r.sortingLayerName = data2;
			}
		}
			break;
		case "Animator":
			go.AddComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(data1);
			break;
		case "BoxCollider2D":
		{
			BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
			bool trigger;
			if(Boolean.TryParse(data1, out trigger))
			{
				bc.isTrigger = trigger;
			}
		}
			break;
		case "RigidBody2D":
		{
			Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
			bool kinematic;
			if(Boolean.TryParse(data1, out kinematic))
			{
				rb.isKinematic = kinematic;
			}
		}
			break;
		case "Tag":
			if(!isStatic)
			{
				go.tag = data1;
			}
			break;
		case "CustomScript":
			if(!isStatic)
			{
				go.AddComponent(Type.GetType(data1));
			}
			break;
		case "Move":
			if(!isStatic)
			{
				Move mc = go.AddComponent<Move>();
				mc.maxSpeed[0] = float.Parse(data1);
				mc.maxSpeed[1] = float.Parse(data2);
				mc.maxSpeed[2] = float.Parse(data3);
				mc.maxSpeed[3] = float.Parse(data4);
			}
			break;
		case "Jump":
			if(!isStatic)
			{
				go.AddComponent<Jump>();

				Rigidbody2D rb = null;
				rb = go.GetComponent<Rigidbody2D>();

				if(rb == null)
				{
					rb = go.AddComponent<Rigidbody2D>();
				}

				rb.isKinematic = false;
				rb.fixedAngle = true;
			}
			break;
		default:
			print ("Component " + name + " is undefined.");
			break;
		}
	}

	private void buttonPressedHandler(object sender, EventArgs e)
	{
		PressGesture gesture = (PressGesture) sender;
		GameObject s = gesture.gameObject;
		if(s.name.Equals(pathBtn.name))
		{
            // Get GameObject at touch location and add a PathFollowing component to it
			Pair<GameObject, GameObject> selectedObject = levelManager.getObjectAtPosition(Camera.main.ScreenToWorldPoint(touchManager.ActiveTouches[0].Position));
			if(selectedObject != null)
			{
				PathFollowing p = selectedObject.second.GetComponent<PathFollowing>();
				if(p != null)
				{
					DestroyImmediate(p);
					p = null;
					
					p = selectedObject.first.GetComponent<PathFollowing>();
					DestroyImmediate(p);
				}
				
				selectedObject.first.AddComponent<PathFollowing>().initDrawing(touchManager.ActiveTouches[touchManager.ActiveTouches.Count-1].Id, true);
				selectedObject.second.AddComponent<PathFollowing>().initDrawing(touchManager.ActiveTouches[touchManager.ActiveTouches.Count-1].Id, false);
			}
		}
		else if(s.name.Equals(replaceBtn.name))
		{
			ReplaceObject();
		}
		else if(s.name.Equals(removeBtn.name))
		{
			RemoveObject();
		}
		else if(s.name.Equals(sliderTab.name))
		{
			sliderTouchID = touchManager.ActiveTouches[touchManager.ActiveTouches.Count - 1].Id;
		}
		else if(s.name.Equals(resetBtn.name))
		{
			if(gesture.ActiveTouches[0].Id == touchManager.ActiveTouches[0].Id)
			{
				removePlacementUI();
			}
			levelManager.revert();
		}
		else if(s.name.Equals(exitBtn.name))
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
		else if(s.name.Equals(cameraBtn.name))
		{
			cameraOutline.SetActive(!cameraOutline.activeSelf);
			cameraPanel.SetActive(!cameraPanel.activeSelf);
			if(gesture.ActiveTouches[0].Id == touchManager.ActiveTouches[0].Id)
			{
				removePlacementUI();
			}
		}
		else if(s.name.Equals(horizontalScrollbar.name) || s.name.Equals(verticalScrollbar.name))
		{
			if(gesture.ActiveTouches[0].Id == touchManager.ActiveTouches[0].Id)
			{
				removePlacementUI();
			}
		}
	}
}
