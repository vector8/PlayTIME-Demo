using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDesignTIME : MonoBehaviour 
{
    // Fake database
    // int is the key (rfid), GameObject is the data (prefab)
    public Dictionary<int, GameObject> database = new Dictionary<int, GameObject>();
    public GameObject[] prefabs;

    // Reference to the board (Grid)
    public Completed.BoardManager grid;

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
	}
	
	// Update is called once per frame
	void Update ()
    {
        int gridIdx = grid.GetTileIndexInGridAtPoint(Input.mousePosition, true);

        if (gridIdx > 0)
        {
            database[activeKey].SetActive(true);
            Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
            database[activeKey].transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);

            if (Input.GetMouseButtonUp(0))
            {
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
        }

        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            database[activeKey].SetActive(false);
            activeKey++;
            activeKey = activeKey % 3;
            database[activeKey].SetActive(true);
        }
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
