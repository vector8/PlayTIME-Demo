using UnityEngine;
using System.Collections;

public class RespawnOther : CustomAction 
{
	public override void run(GameObject other = null, int id = 0)
	{
		if(isValidTag(other.tag))
		{
			LevelManager.instance.revertObject(other);
		}
	}
	
	public override void reset()
	{
	}
}
