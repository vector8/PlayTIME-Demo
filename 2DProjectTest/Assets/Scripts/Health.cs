using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class Health : MonoBehaviour 
{
	public int hp, maxHP, startHP;
	public int directions;
	public float deathAnimTime = 0f;

	public enum DeathActions
	{
		Despawn = 0,
		Respawn = 1
	}
		
	private enum DamageDirections
	{
		Top = 1,
		Right = 2,
		Bottom = 4,
		Left = 8
	}

	public DeathActions da;

	private float damageCooldownTimer;
	private const float DAMAGE_COOLDOWN = 0.5f;
	private float deathAnimTimer = 0f;
	private float topAngle, sideAngle;
	private Animator anim;
	private static DeathActions maxDA = Enum.GetValues(typeof(DeathActions)).Cast<DeathActions>().Max();
	

	// Use this for initialization
	void Start () 
	{
		hp = startHP;

		Vector2 size = GetComponent<BoxCollider2D>().size;
		size = Vector2.Scale(size, (Vector2)transform.localScale);
		topAngle = Mathf.Atan(size.x / size.y) * Mathf.Rad2Deg;
		sideAngle = 90.0f - topAngle;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(hp <= 0)
		{
			if(deathAnimTimer < deathAnimTime)
			{
				deathAnimTimer += Time.deltaTime;
			}
			
			if(deathAnimTimer >= deathAnimTime)
			{
				hp = startHP;
				deathAnimTimer = 0f;

				switch(da)
				{
				case DeathActions.Despawn:
					// De-activate this object, it will re-activate when the Reset button is pressed.
					gameObject.SetActive(false);
					break;
				case DeathActions.Respawn:
					LevelManager.instance.revertObject(gameObject);
					break;
				default:
					break;
				}
				
				DeathTrigger dt = GetComponent<DeathTrigger>();
				if(dt != null)
				{
					dt.run();
				}
			}
		}
		else
		{
			if(anim == null)
			{
				anim = GetComponent<Animator>();
			}
			if(anim != null)
			{
				// disable warning in case the bool "dying" does not exist on this gameobject's animator
				anim.logWarnings = false;
				anim.SetBool("Dying", false);
				anim.logWarnings = true;
			}
		}

		if(damageCooldownTimer > 0)
		{
			damageCooldownTimer -= Time.deltaTime;
		}
	}

	public bool receiveDamage(GameObject other, int dmg)
	{
		if(damageCooldownTimer <= 0)
		{
			Vector2 v = (Vector2)(other.transform.position - transform.position);

			if (Vector2.Angle(v, transform.up) <= topAngle && directionsInclude(directions, DamageDirections.Top) ||
			    Vector2.Angle(v, transform.right) <= sideAngle && directionsInclude(directions, DamageDirections.Right) ||
			    Vector2.Angle(v, -transform.up) <= topAngle && directionsInclude(directions, DamageDirections.Bottom) ||
			    Vector2.Angle(v, -transform.right) <= sideAngle && directionsInclude(directions, DamageDirections.Left))
			{
				hp -= dmg;
				damageCooldownTimer = DAMAGE_COOLDOWN;
				if(hp <= 0)
				{
					hp = 0;
					if(anim == null)
					{
						anim = GetComponent<Animator>();
					}
					if(anim != null)
					{
#pragma warning disable
						anim.logWarnings = false;
						anim.SetBool("Dying", true);
						anim.logWarnings = true;
#pragma warning restore
					}
				}
				else if(hp > maxHP)
				{
					hp = maxHP;
				}
				return true;
			}
		}
		return false;
	}

	private bool directionsInclude(int directions, DamageDirections collisionDirection)
	{
		bool value = (directions & (int)collisionDirection) == (int)collisionDirection;
		return value;
	}

	public void setDeathAction(int actionID)
	{
		if(actionID <= (int) maxDA)
		{
			da = (DeathActions) actionID;
		}
	}
}
