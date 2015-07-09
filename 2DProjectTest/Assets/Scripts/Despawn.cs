using UnityEngine;
using System.Collections;

public class Despawn : CustomAction
{
	public override void run(GameObject other)
	{
		gameObject.SetActive(false);
	}

	public override void reset()
	{
		gameObject.SetActive(true);
	}
}
