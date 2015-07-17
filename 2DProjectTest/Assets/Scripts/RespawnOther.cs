using UnityEngine;
using System.Collections;

public class RespawnOther : CustomAction 
{
	public override void run(GameObject other, int id)
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
