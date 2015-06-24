﻿using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	public int hp, maxHP;
	public string enemyTag;
	public int directions;

	private enum DamageDirections
	{
		Top = 1,
		Right = 2,
		Bottom = 4,
		Left = 8
	}

	private float damageCooldownTimer;
	private const float DAMAGE_COOLDOWN = 0.5f;

	private float topAngle, sideAngle;

	// Use this for initialization
	void Start () 
	{
		hp = maxHP;

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

		}

		if(damageCooldownTimer > 0)
		{
			damageCooldownTimer -= Time.deltaTime;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(damageCooldownTimer <= 0 && coll.gameObject.tag == enemyTag)
		{
			Damage d = coll.gameObject.GetComponent<Damage>();
			if(d != null)
			{
				Vector3 v = (Vector3)coll.gameObject.transform.position - transform.position;

				if (Vector3.Angle(v, transform.up) <= topAngle && directionsInclude(directions, DamageDirections.Top) && directionsInclude(d.directions, DamageDirections.Bottom) ||
				    Vector3.Angle(v, transform.right) <= sideAngle && directionsInclude(directions, DamageDirections.Right) && directionsInclude(d.directions, DamageDirections.Left) ||
				    Vector3.Angle(v, -transform.up) <= topAngle && directionsInclude(directions, DamageDirections.Bottom) && directionsInclude(d.directions, DamageDirections.Top) ||
				    Vector3.Angle(v, -transform.right) <= sideAngle && directionsInclude(directions, DamageDirections.Left) && directionsInclude(d.directions, DamageDirections.Right))
				{
					hp -= d.dmg; 
					damageCooldownTimer = DAMAGE_COOLDOWN;
				}
			}

		}
	}

	private bool directionsInclude(int directions, DamageDirections collisionDirection)
	{
		bool value = (directions & (int)collisionDirection) == (int)collisionDirection;
		return value;
	}
}
