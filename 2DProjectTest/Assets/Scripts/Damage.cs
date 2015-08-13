using UnityEngine;
using System.Collections;

public class Damage : CustomAction 
{
	public int dmg;

	public override void run(GameObject other = null, int id = 0)
	{
		if(other != null && isValidTag(other.tag))
		{
			Health h = other.GetComponent<Health>();
			if(h != null && h.enabled)
			{
                h.receiveDamage(gameObject, dmg);
			}
		}
	}
	
	public override void reset()
	{
	}
}
