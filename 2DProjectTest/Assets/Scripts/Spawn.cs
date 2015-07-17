using UnityEngine;
using System.Collections;

public class Spawn : CustomAction
{
	public GameObject toSpawn = null;

	public int maxSpawnCount;
	public int currentSpawnCount;

	public override void run(GameObject other, int id)
	{
		if(toSpawn != null && currentSpawnCount > 0 && isValidTag(other.tag))
		{
			LevelManager.instance.placeSpawnedObject(gameObject.transform.position + new Vector3(0f, 1f), toSpawn, gameObject.transform);
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
