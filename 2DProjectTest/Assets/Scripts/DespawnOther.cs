using UnityEngine;
using System.Collections;

public class DespawnOther : CustomAction
{
	public override void run(GameObject other = null, int id = 0)
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
