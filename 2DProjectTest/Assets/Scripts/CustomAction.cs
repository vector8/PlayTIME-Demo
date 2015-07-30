using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CustomAction : MonoBehaviour
{
	public virtual void initialize()
	{
	}
	public abstract void run(GameObject other = null, int id = 0);
	public abstract void reset();

	public List<string> includedTags;
	public List<string> excludedTags;

	public bool isValidTag(string tag)
	{
		if(includedTags.Count > 0)
		{
			return includedTags.Contains(tag) && !excludedTags.Contains(tag);
		}
		else
		{
			return !excludedTags.Contains(tag);
		}
	}

	public enum ActionTypes
	{	// DB fields		ID	Directions	Param1			Param2			Param3
 		Spawn = 0,		//	0	directions	TagsAffected	rfidKeyToSpawn	#ToSpawn 
		Despawn,		//	1	directions	TagsAffected
		Transfigure,	//	2	directions	TagsAffected	AnimController	Reversible
		DespawnOther,	//	3	directions	TagsAffected
		RespawnOther,	//	4	directions	TagsAffected
		Damage,			//	5	directions	TagsAffected	Amount
		MoveHoriz		//	6	directions	TagsAffected	Speed			TagsToIgnore
	}
}