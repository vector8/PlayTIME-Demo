using UnityEngine;
using System.Collections;

public class Damage : CustomAction 
{
	public int dmg;

	public override void run(GameObject other = null, int id = 0)
	{
		if(isValidTag(other.tag))
		{
			Health h = other.GetComponent<Health>();
			if(h != null && h.enabled)
			{
				if(h.receiveDamage(gameObject, dmg) && gameObject.tag == "Player" && GetComponent<Jump>() != null)
				{
					Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
					rb.AddRelativeForce(new Vector2(0f, 3f), ForceMode2D.Impulse);
					Animator anim = GetComponent<Animator>();
					if(anim != null)
					{
						anim.SetBool("Jumping", true);
						anim.SetBool("MidJump", true);
					}
				}
			}
		}
	}
	
	public override void reset()
	{
	}
}
