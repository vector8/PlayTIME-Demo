using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour 
{
	public static LevelManager instance {get; private set;}

	public const float SCREEN_GAP = 10000f;

	private List<GameObject> placedObjects = new List<GameObject>();
	private List<GameObject> staticPlacedObjects = new List<GameObject>();

	private List<GameObject> backgroundPlacedObjects = new List<GameObject>();
	private List<GameObject> staticBackgroundPlacedObjects = new List<GameObject>();

	void Awake()
	{
		instance = this;
	}

	public void placeObject(Vector2 position, GameObject toSpawn, GameObject staticToSpawn, Transform parent)
	{
		GameObject g = GameObject.Instantiate(toSpawn);
		g.transform.position = (new Vector2(0f, LevelManager.SCREEN_GAP)) + position;
		g.transform.parent = parent;
		GameObject sg = GameObject.Instantiate(staticToSpawn);
		sg.transform.position = position;
		sg.transform.parent = parent;
		// Store placed object
		if(g.tag == "Background")
		{
			backgroundPlacedObjects.Add(g);
			staticBackgroundPlacedObjects.Add(sg);
		}
		else
		{
			placedObjects.Add(g);
			staticPlacedObjects.Add(sg);
		}
	}

	public void removeObject(Vector2 position, bool backgroundOnly)
	{
		if(!backgroundOnly)
		{
			for(int i = 0; i < staticPlacedObjects.Count; i++)
			{
				if(staticPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
				{
					DestroyImmediate(placedObjects[i]);
					placedObjects.RemoveAt(i);
					DestroyImmediate(staticPlacedObjects[i]);
					staticPlacedObjects.RemoveAt(i);
					return;
				}
			}
		}
			
		for(int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
		{
			if(staticBackgroundPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
			{
				DestroyImmediate(backgroundPlacedObjects[i]);
				backgroundPlacedObjects.RemoveAt(i);
				DestroyImmediate(staticBackgroundPlacedObjects[i]);
				staticBackgroundPlacedObjects.RemoveAt(i);
			}
		}
	}

	public void replaceObject(Vector2 position, GameObject toReplace, GameObject staticToReplace, Transform parent)
	{
		GameObject g = GameObject.Instantiate(toReplace);
		g.transform.parent = parent;
		GameObject sg = GameObject.Instantiate(staticToReplace);
		sg.transform.parent = parent;

		if(g.tag == "Background")
		{
			for(int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
			{
				if(staticBackgroundPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
				{
					g.transform.position = backgroundPlacedObjects[i].transform.position;
					sg.transform.position = staticBackgroundPlacedObjects[i].transform.position;
					
					DestroyImmediate(backgroundPlacedObjects[i]);
					backgroundPlacedObjects.RemoveAt(i);
					DestroyImmediate(staticBackgroundPlacedObjects[i]);
					staticBackgroundPlacedObjects.RemoveAt(i);
					
					backgroundPlacedObjects.Add(g);
					staticBackgroundPlacedObjects.Add(sg);
					return;
				}
			}
		}
		else
		{
			for(int i = 0; i < staticPlacedObjects.Count; i++)
			{
				if(staticPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
				{
					g.transform.position = placedObjects[i].transform.position;
					sg.transform.position = staticPlacedObjects[i].transform.position;

					DestroyImmediate(placedObjects[i]);
					placedObjects.RemoveAt(i);
					DestroyImmediate(staticPlacedObjects[i]);
					staticPlacedObjects.RemoveAt(i);

					placedObjects.Add(g);
					staticPlacedObjects.Add(sg);
					return;
				}
			}
		}
	}

	public void revert()
	{
		for (int i = 0; i < placedObjects.Count; i++)
		{
			placedObjects[i].transform.position = staticPlacedObjects[i].transform.position + (new Vector3(0f, LevelManager.SCREEN_GAP, 0f));
			PathFollowing p = placedObjects[i].GetComponent<PathFollowing>();
			if(p != null)
			{
				p.setStateToIdle();
			}
		}

		for (int i = 0; i < backgroundPlacedObjects.Count; i++)
		{
			backgroundPlacedObjects[i].transform.position = staticBackgroundPlacedObjects[i].transform.position + (new Vector3(0f, LevelManager.SCREEN_GAP, 0f));
			PathFollowing p = backgroundPlacedObjects[i].GetComponent<PathFollowing>();
			if(p != null)
			{
				p.setStateToIdle();
			}
		}
	}

	public bool isObjectAtPosition(Vector2 position)
	{
		for(int i = 0; i < staticPlacedObjects.Count; i++)
		{
			if(staticPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
			{
				return true;
			}
		}

		return false;
	}

	public bool isBackgroundObjectAtPosition(Vector2 position)
	{
		for(int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
		{
			if(staticBackgroundPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
			{
				return true;
			}
		}
		
		return false;
	}

	public Pair<GameObject, GameObject> getObjectAtPosition(Vector2 position)
	{
		for(int i = 0; i < staticPlacedObjects.Count; i++)
		{
			if(staticPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
			{
				return new Pair<GameObject,GameObject>(placedObjects[i], staticPlacedObjects[i]);
			}
		}
		
		return null;
	}
	
	public Pair<GameObject, GameObject> getBackgroundObjectAtPosition(Vector2 position)
	{
		for(int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
		{
			if(staticBackgroundPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
			{
				return new Pair<GameObject,GameObject>(backgroundPlacedObjects[i], staticBackgroundPlacedObjects[i]);
			}
		}
		
		return null;
	}
}
