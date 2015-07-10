using UnityEngine;
using System.Collections;

public class Heal : CustomAction
{
	public int amount;
	public string targetTag;

	private bool done = false;

	public override void run(GameObject other)
	{
		if(!done && other.tag == targetTag)
		{
			Health h = other.GetComponent<Health>();
			if(h != null)
			{
				h.hp += amount;
				if(h.hp > h.maxHP)
				{
					h.hp = h.maxHP;
				}
			}

			done = true;
		}
	}

	public override void reset()
	{
		done = false;
	}
}
