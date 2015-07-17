using UnityEngine;
using System.Collections;

public class Damage : CustomAction 
{
	public int dmg;

	public override void run(GameObject other, int id)
	{
		if(isValidTag(other.tag))
		{
			Health h = other.GetComponent<Health>();
			if(h != null)
			{
				h.receiveDamage(gameObject, dmg);
			}
		}
	}
	
	public override void reset()
	{
	}
}
