﻿using UnityEngine;
using System.Collections;

public class Despawn : CustomAction
{
	public override void run(GameObject other, int id)
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
