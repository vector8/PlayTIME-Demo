using UnityEngine;
using System.Collections;

public class Despawn : CustomAction
{
	public override void run(GameObject other = null, int id = 0)
	{
		if(other == null || isValidTag(other.tag))
		{
			gameObject.SetActive(false);
		}
	}

	public override void reset()
	{
		gameObject.SetActive(true);
	}
}
