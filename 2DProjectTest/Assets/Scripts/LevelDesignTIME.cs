using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDesignTIME : MonoBehaviour 
{
    // Fake database
    // int is the key (rfid), GameObject is the data (prefab)
    public Dictionary<int, GameObject> database = new Dictionary<int, GameObject>();
    public GameObject[] prefabs;

    public GameObject PlacementUI;
    public GameObject MouseModeActiveText;
    public Vector2 PlacementUIOffsetInPixels;

    private Vector3 LastMousePosition;

    // Reference to the board (Grid)
    private Completed.BoardManager grid;

    // Prevent UI from moving when using mouse as input
    public bool mouseMode = false;
    private bool lockPositions = false;
    private int lockedPosition;

    // Current or latest rfid tag read.
    // Value of -1 means none active
    public int activeKey = -1;

	// Use this for initialization
	void Start () 
    {
	    // Add things to the data base
        // Key will be replaced with rfid values
        for (int i = 0; i < prefabs.Length; i++)
        {
            // Instantiated them for displaying purposes only
            GameObject go = Instantiate(prefabs[i]);
            database.Add(i, go);
            go.SetActive(false);
        }

        grid = Completed.GameManager.instance.GetBoardScript();

        activeKey = 0;
        database[activeKey].SetActive(true);
        PlacementUI.SetActive(false);
        LastMousePosition = Input.mousePosition;

        if (mouseMode)
            MouseModeActiveText.SetActive(true);
        else
            MouseModeActiveText.SetActive(false);
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
            PlacementUI.transform.position = Camera.main.WorldToScreenPoint(new Vector3(wsTilePos.x, wsTilePos.y, 1.0f)) + new Vector3(PlacementUIOffsetInPixels.x, PlacementUIOffsetInPixels.y, 0.0f);
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

        if (gridIdx > 0)
        {
            if (mouseMode)
            {
                // Lock the ui position so it doesnt move when we try to click it
                if (Input.GetMouseButtonUp(0))
                {
                    lockPositions = !lockPositions;
                    PlacementUI.SetActive(lockPositions);
                    UpdatePlacementUI(gridIdx, lockPositions);
                    lockedPosition = gridIdx;
                }

                if (!lockPositions)
                {
                    Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
                    database[activeKey].transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
                }
            }
            else // Touch mode
            {
                // If mouse is moving, hide UI
                if (Vector3.Magnitude(Input.mousePosition - LastMousePosition) > 0.0f)
                {
                    PlacementUI.SetActive(false);
                    Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
                    database[activeKey].transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);
                }
                else
                {
                    PlacementUI.SetActive(true);
                    UpdatePlacementUI(gridIdx);
                } 
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
        }

        // Cycle through the prefabs 
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            database[activeKey].SetActive(false);
            activeKey++;
            activeKey = activeKey % prefabs.Length;
            database[activeKey].SetActive(true);
        }

        LastMousePosition = Input.mousePosition;
	}

    public void PlaceObject()
    {
        int gridIdx = 0;

        if (!mouseMode)
            gridIdx = grid.GetTileIndexInGridAtPoint(Input.mousePosition, false);
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
            gridIdx = grid.GetTileIndexInGridAtPoint(Input.mousePosition, false);
        else
            gridIdx = lockedPosition;

        grid.RemoveObjectAtIndex(gridIdx);
    }

    public void ReplaceObject()
    {
        int gridIdx = 0;

        if (!mouseMode)
            gridIdx = grid.GetTileIndexInGridAtPoint(Input.mousePosition, false);
        else
            gridIdx = lockedPosition;

        grid.ReplaceObjectAtIndex(gridIdx, database[activeKey], this.transform);
    }

    // Description:
    // Obtains data represented by RFID Figurine from "database".
    // Arguments:
    // int rfid - The id to look up
    // Returns:
    // ?
    void LookupRFID()
    {
        
    }
}
