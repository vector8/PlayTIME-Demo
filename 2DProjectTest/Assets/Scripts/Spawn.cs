using UnityEngine;
using System.Collections;

public class Spawn : CustomAction
{
	public GameObject toSpawn = null;

	public int maxSpawnCount;
	public int currentSpawnCount;

	public bool spawnUnderParent = false;

	public override void run(GameObject other = null, int id = 0)
	{
		if(toSpawn != null && currentSpawnCount > 0 && (other == null || isValidTag(other.tag)))
		{
			if(spawnUnderParent)
			{
				LevelManager.instance.placeSpawnedObject(gameObject.transform.position, toSpawn, gameObject.transform.parent);
			}
			else
			{
				LevelManager.instance.placeSpawnedObject(gameObject.transform.position + new Vector3(0f, 1f), toSpawn, gameObject.transform);
			}
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
