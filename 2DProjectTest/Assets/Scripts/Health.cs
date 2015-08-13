using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class Health : MonoBehaviour 
{
	public int hp, maxHP, startHP;
	public int directions;
    public float damageCooldown;

    public bool allowHeal = true, allowDamage = true;

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
	private float topAngle, sideAngle;
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
			switch(da)
			{
			case DeathActions.Despawn:
            {
				// De-activate this object, it will re-activate when the Reset button is pressed.
                Despawn d = gameObject.GetComponent<Despawn>();
                if(d == null)
                {
                    d = gameObject.AddComponent<Despawn>();
                }

                d.deathAnimID = d.defaultDeathAnimID;
                d.run();
            }
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

		if(damageCooldownTimer > 0)
		{
			damageCooldownTimer -= Time.deltaTime;
		}
	}

	public bool receiveDamage(GameObject other, int dmg)
	{
        if(dmg > 0 && !allowDamage || dmg < 0 && !allowHeal)
        {
            return false;
        }

		if(damageCooldownTimer <= 0)
		{
			Vector2 v = (Vector2)(other.transform.position - transform.position);

			if (Vector2.Angle(v, transform.up) <= topAngle && directionsInclude(directions, DamageDirections.Top) ||
			    Vector2.Angle(v, transform.right) <= sideAngle && directionsInclude(directions, DamageDirections.Right) ||
			    Vector2.Angle(v, -transform.up) <= topAngle && directionsInclude(directions, DamageDirections.Bottom) ||
			    Vector2.Angle(v, -transform.right) <= sideAngle && directionsInclude(directions, DamageDirections.Left))
			{
				hp -= dmg;
                damageCooldownTimer = damageCooldown;
				if(hp <= 0)
				{
					hp = 0;
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
