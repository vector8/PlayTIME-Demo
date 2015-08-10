using UnityEngine;
using System.Collections.Generic;

public struct ActionQueueItem
{
    public CustomAction action;
    public GameObject target;
    public int id;
}

public class LevelManager
{
    public const float SCREEN_GAP = 10000f;

    public List<GameObject> placedObjects = new List<GameObject>();
    public List<GameObject> staticPlacedObjects = new List<GameObject>();
    public List<GameObject> backgroundPlacedObjects = new List<GameObject>();
    public List<GameObject> staticBackgroundPlacedObjects = new List<GameObject>();
    public bool paused = true;
    public bool loading = false;

	private static LevelManager _instance = null;
	private List<ICanReset> resetListeners = new List<ICanReset>();

    private List<ActionQueueItem> actionQueue = new List<ActionQueueItem>();

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

	private List<GameObject> spawnedObjects = new List<GameObject>();

	private void specialPlacementLogic(GameObject g, GameObject sg = null, GameObject oldG = null)
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
                t.targetAndTags.Clear();
                foreach(TargetAnimControllerAndTags tar in oldT.targetAndTags)
                {
                    TargetAnimControllerAndTags newTar = new TargetAnimControllerAndTags();
                    newTar.targetAnimController = tar.targetAnimController;
                    newTar.includedTags = new List<string>(tar.includedTags);
                    newTar.excludedTags = new List<string>(tar.excludedTags);
                    newTar.reversible = tar.reversible;
                    newTar.id = tar.id;
                    newTar.done = false;
                    t.targetAndTags.Add(newTar);
                }
			}
		}

		TimeTrigger tt = g.GetComponent<TimeTrigger>();
		if(tt != null)
		{
			tt.initialize();
		}

		MoveHorizontalUntilCollision mh = g.GetComponent<MoveHorizontalUntilCollision>();
		if(mh != null)
		{
			mh.run();
		}

		KoopaTroopa kt = g.GetComponent<KoopaTroopa>();
		if(kt != null)
		{
			addResetListener(kt);
		}

        FirePower fp = g.GetComponent<FirePower>();
        if(fp != null)
        {
            addResetListener(fp);
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

		specialPlacementLogic(g, sg, toSpawn);

		// Store placed object
		if(g.tag == "PaintableBackground")
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

	public GameObject placeSpawnedObject(Vector2 position, GameObject toSpawn, Transform parent)
	{
		GameObject g = GameObject.Instantiate(toSpawn);
		g.transform.position = position;
		g.transform.parent = parent;
		g.SetActive(true);
		g.AddComponent<JustSpawned>();
		specialPlacementLogic(g, null, toSpawn);
		spawnedObjects.Add(g);
        return g;
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

    public void removeSpawnedObject(GameObject go)
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == go)
            {
                GameObject.Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
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

		if(g.tag == "PaintableBackground")
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

					specialPlacementLogic(g, sg, toReplace);
					
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

					specialPlacementLogic(g, sg, toReplace);

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
        spawnedObjects.Clear();

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

        resetListeners.Clear();
    }

    public void addActionQueueItem(CustomAction a, GameObject t = null, int id = 0)
    {
        ActionQueueItem aqi = new ActionQueueItem();
        aqi.action = a;
        aqi.target = t;
        aqi.id = id;

        this.actionQueue.Add(aqi);
    }

    public void executeActionQueue()
    {
        for (int i = 0; i < actionQueue.Count; i++)
        {
            actionQueue[i].action.run(actionQueue[i].target, actionQueue[i].id);
        }

        actionQueue.Clear();
    }
}
