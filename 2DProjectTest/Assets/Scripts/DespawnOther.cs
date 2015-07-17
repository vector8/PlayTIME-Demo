using UnityEngine;
using System.Collections;

public class DespawnOther : CustomAction
{
	public override void run(GameObject other, int id)
	{
		if(isValidTag(other.tag))
		{
			other.SetActive(false);
		}
	}

	public override void reset()
	{
	}
}
