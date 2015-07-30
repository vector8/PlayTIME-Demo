using UnityEngine;
using System.Collections;

public class JustSpawned : MonoBehaviour 
{
	private const float SPAWNING_TIME = 0.5f;
	private float spawningTimer = 0f;

	// Update is called once per frame
	void Update () 
	{
		if(spawningTimer < SPAWNING_TIME)
		{
			spawningTimer += Time.deltaTime;

			if(spawningTimer >= SPAWNING_TIME)
			{
				enabled = false;
			}
		}
	}
}
