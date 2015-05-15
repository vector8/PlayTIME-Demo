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
            database.Add(i, prefabs[i]);
            prefabs[i] = Instantiate(prefabs[i]);
            prefabs[i].SetActive(false);
        }

        grid = Completed.GameManager.instance.GetBoardScript();

        activeKey = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        int gridIdx = grid.GetTileIndexInGridAtPoint(Input.mousePosition, true);
        Debug.Log(gridIdx);

        if (gridIdx > 0)
        {
            prefabs[activeKey].SetActive(true);
            Vector2 wsTilePos = grid.GetPositionFromIndex(gridIdx);
            prefabs[activeKey].transform.position = new Vector3(wsTilePos.x, wsTilePos.y, 1.0f);

            if (Input.GetMouseButtonUp(0))
            {
                GameObject g = GameObject.Instantiate(database[activeKey]);
                g.transform.position = prefabs[activeKey].transform.position;
            }
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            prefabs[activeKey].SetActive(false);
            activeKey++;
            activeKey = activeKey % 3;
            prefabs[activeKey].SetActive(true);
        }
        Debug.Log(activeKey);
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
