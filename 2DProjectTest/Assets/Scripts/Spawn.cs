using UnityEngine;
using System.Collections;

public class Spawn : CustomAction
{
	public GameObject toSpawn = null;

	public int maxSpawnCount;
	public int currentSpawnCount;

	public override void run(GameObject other)
	{
		if(toSpawn != null && currentSpawnCount > 0)
		{
			LevelManager.instance.placeSpawnedObject(gameObject.transform.position + new Vector3(0f, 1f), toSpawn, gameObject.transform.parent);
			currentSpawnCount--;
		}
	}

	public override void reset()
	{
		currentSpawnCount = maxSpawnCount;
	}

	public void setMaxSpawnCount(int c)
	{
		maxSpawnCount = c;
		currentSpawnCount = maxSpawnCount;
	}
}
