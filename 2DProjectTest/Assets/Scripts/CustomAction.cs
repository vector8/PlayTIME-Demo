using UnityEngine;
using System.Collections;

public abstract class CustomAction : MonoBehaviour
{
	public abstract void run(GameObject other);
	public abstract void reset();

	public enum ActionTypes
	{	// DB fields		ID	Directions	Param1			Param2		Param3
 		Spawn = 0,		//	0	directions	TagsAffected	rfidKeyToSpawn	#ToSpawn 
		Despawn,		//	1	directions	TagsAffected
		Transfigure		//	2	directions	TagsAffected	AnimController	Reversible
	}
}