using UnityEngine;
using System.Collections.Generic;

public class LevelManager
{
	private static LevelManager _instance = null;
	private List<ICanReset> resetListeners = new List<ICanReset>();

	public static LevelManager instance 
	{
		get
		{
			if(_instance == null)
			{
				_instance = new LevelManager();
			}

			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	public const float SCREEN_GAP = 10000f;

	public List<GameObject> placedObjects = new List<GameObject>();
	public List<GameObject> staticPlacedObjects = new List<GameObject>();
    public List<GameObject> backgroundPlacedObjects = new List<GameObject>();
    public List<GameObject> staticBackgroundPlacedObjects = new List<GameObject>();

	private List<GameObject> spawnedObjects = new List<GameObject>();

	private void specialPlacementLogic(GameObject g, bool spawned, GameObject sg = null, GameObject oldG = null)
	{
		if(sg != null)
		{
			// Special behaviour for Resize component
			Resize r = sg.GetComponent<Resize>();
			if(r != null)
			{
				r.nonStatic = g;
			}
		}
			
		// Special behaviour for CollideTrigger component
		CollideTrigger ct = g.GetComponent<CollideTrigger>();
		if(ct != null)
		{
			ct.initialize();
		}

		if(oldG != null)
		{
			Transfigure oldT = oldG.GetComponent<Transfigure>();
			if(oldT != null)
			{
				Transfigure t = g.GetComponent<Transfigure>();
				t.targetAndTags = new List<TargetAnimControllerAndTags>(oldT.targetAndTags);
			}
		}

		TimeTrigger tt = g.GetComponent<TimeTrigger>();
		if(tt != null)
		{
			tt.initialize();
		}

		if(!spawned)
		{
			MoveHorizontalUntilCollision mh = g.GetComponent<MoveHorizontalUntilCollision>();
			if(mh != null)
			{
				mh.run();
			}
		}

		KoopaTroopa kt = g.GetComponent<KoopaTroopa>();
		if(kt != null)
		{
			addResetListener(kt);
		}
	}

	private void specialRevertLogic(GameObject g, GameObject sg = null)
	{
		if(sg != null)
		{
		}
		
		PathFollowing p = g.GetComponent<PathFollowing>();
		if(p != null)
		{
			p.setStateToIdle();
		}

		CollideTrigger ct = g.GetComponent<CollideTrigger>();
		if(ct != null)
		{
			ct.reset();
		}
		
		TimeTrigger tt = g.GetComponent<TimeTrigger>();
		if(tt != null)
		{
			tt.reset();
		}
		
		DeathTrigger dt = g.GetComponent<DeathTrigger>();
		if(dt != null)
		{
			dt.reset();
		}

		Health h = g.GetComponent<Health>();
		if(h != null)
		{
			h.hp = h.startHP;
		}

		StarPower sp = g.GetComponent<StarPower>();
		if(sp != null)
		{
			sp.reset();
		}
	}

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

        g.SetActive(true);
        sg.SetActive(true);

		specialPlacementLogic(g, false, sg, toSpawn);

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

	public void placeSpawnedObject(Vector2 position, GameObject toSpawn, Transform parent)
	{
		GameObject g = GameObject.Instantiate(toSpawn);
		g.transform.position = position;
		g.transform.parent = parent;
		g.SetActive(true);
		g.AddComponent<JustSpawned>();
		specialPlacementLogic(g, true);
		spawnedObjects.Add(g);
	}

	public void removeObject(Vector2 position, bool backgroundOnly)
	{
		if(!backgroundOnly)
		{
			for(int i = 0; i < staticPlacedObjects.Count; i++)
			{
				if(staticPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
				{
					GameObject.DestroyImmediate(placedObjects[i]);
					placedObjects.RemoveAt(i);
					GameObject.DestroyImmediate(staticPlacedObjects[i]);
					staticPlacedObjects.RemoveAt(i);
					return;
				}
			}
		}
			
		for(int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
		{
			if(staticBackgroundPlacedObjects[i].GetComponent<Collider2D>().OverlapPoint(position))
			{
				GameObject.DestroyImmediate(backgroundPlacedObjects[i]);
				backgroundPlacedObjects.RemoveAt(i);
				GameObject.DestroyImmediate(staticBackgroundPlacedObjects[i]);
				staticBackgroundPlacedObjects.RemoveAt(i);
			}
		}
	}

	public void removeObject(GameObject go)
	{
		for(int i = 0; i < staticPlacedObjects.Count; i++)
		{
			if(placedObjects[i] == go)
			{
				GameObject.DestroyImmediate(placedObjects[i]);
				placedObjects.RemoveAt(i);
				GameObject.DestroyImmediate(staticPlacedObjects[i]);
				staticPlacedObjects.RemoveAt(i);
				return;
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
					
					GameObject.DestroyImmediate(backgroundPlacedObjects[i]);
					backgroundPlacedObjects.RemoveAt(i);
					GameObject.DestroyImmediate(staticBackgroundPlacedObjects[i]);
					staticBackgroundPlacedObjects.RemoveAt(i);

					specialPlacementLogic(g, false, sg);
					
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

					GameObject.DestroyImmediate(placedObjects[i]);
					placedObjects.RemoveAt(i);
					GameObject.DestroyImmediate(staticPlacedObjects[i]);
					staticPlacedObjects.RemoveAt(i);

					specialPlacementLogic(g, false, sg);

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
			placedObjects[i].SetActive(true);
			placedObjects[i].transform.position = staticPlacedObjects[i].transform.position + (new Vector3(0f, LevelManager.SCREEN_GAP, 0f));
			specialRevertLogic(placedObjects[i], staticPlacedObjects[i]);
		}

		for (int i = 0; i < backgroundPlacedObjects.Count; i++)
		{
			backgroundPlacedObjects[i].SetActive(true);
			backgroundPlacedObjects[i].transform.position = staticBackgroundPlacedObjects[i].transform.position + (new Vector3(0f, LevelManager.SCREEN_GAP, 0f));
			specialRevertLogic(backgroundPlacedObjects[i], staticBackgroundPlacedObjects[i]);
		}

		for(int i = 0; i < spawnedObjects.Count; i++)
		{
			GameObject.DestroyImmediate(spawnedObjects[i]);
		}

		foreach(ICanReset r in resetListeners)
		{
			r.reset();
		}
	}

	public void revertObject(GameObject go)
	{
		for(int i = 0; i < placedObjects.Count; i++)
		{
			if(placedObjects[i] == go)
			{
				placedObjects[i].transform.position = staticPlacedObjects[i].transform.position + (new Vector3(0f, LevelManager.SCREEN_GAP, 0f));
				specialRevertLogic(placedObjects[i]);
				return;
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

    public Pair<GameObject, GameObject> getObjectExactlyAtPosition(Vector2 position)
    {
        for (int i = 0; i < staticPlacedObjects.Count; i++)
        {
            Vector2 checkPos = (Vector2)staticPlacedObjects[i].transform.position;
            if (checkPos == position)
            {
                return new Pair<GameObject, GameObject>(placedObjects[i], staticPlacedObjects[i]);
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

    public Pair<GameObject, GameObject> getBackgroundObjectExactlyAtPosition(Vector2 position)
    {
        for (int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
        {
            if ((Vector2)staticBackgroundPlacedObjects[i].transform.position == position)
            {
                return new Pair<GameObject, GameObject>(backgroundPlacedObjects[i], staticBackgroundPlacedObjects[i]);
            }
        }

        return null;
    }

	public void addResetListener(ICanReset r)
	{
		resetListeners.Add(r);
	}

    public void wipeLevel()
    {
        for(int i = 0; i < staticPlacedObjects.Count; i++)
        {
            GameObject.DestroyImmediate(staticPlacedObjects[i]);
            GameObject.DestroyImmediate(placedObjects[i]);
        }
        placedObjects.Clear();
        staticPlacedObjects.Clear();

        for (int i = 0; i < staticBackgroundPlacedObjects.Count; i++)
        {
            GameObject.DestroyImmediate(staticBackgroundPlacedObjects[i]);
            GameObject.DestroyImmediate(backgroundPlacedObjects[i]);
        }
        staticBackgroundPlacedObjects.Clear();
        backgroundPlacedObjects.Clear();

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            GameObject.DestroyImmediate(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
    }
}
