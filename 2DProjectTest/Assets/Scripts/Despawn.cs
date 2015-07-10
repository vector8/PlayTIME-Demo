using UnityEngine;
using System.Collections;

public class Despawn : CustomAction
{
	public string targetTag;

	public override void run(GameObject other)
	{
		if(other.tag.Equals(targetTag))
		{
			gameObject.SetActive(false);
		}
	}

	public override void reset()
	{
		gameObject.SetActive(true);
	}
}
