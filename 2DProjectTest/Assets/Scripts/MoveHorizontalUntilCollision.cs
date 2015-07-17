using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveHorizontalUntilCollision : MonoBehaviour 
{
	public float speed = 1f;
	public List<string> tagsToIgnore;

	private int direction = 1;
	private float sideAngle;
	private Animator anim;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(anim == null)
		{
			anim = GetComponent<Animator>();
		}
		if(anim != null)
		{
			anim.SetBool("HorizontalMovement", true);
		}

		Vector3 pos = transform.position;
		pos.x += (float)direction * speed * Time.deltaTime;
		transform.position = pos;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(!tagsToIgnore.Contains(coll.gameObject.tag))
		{
			Vector2 v = (Vector2)(coll.gameObject.transform.position - transform.position);
			
			if (Vector2.Angle(v, transform.right) <= 40f)
			{
				direction = -1;
			}
			else if(Vector2.Angle(v, -transform.right) <= 40f)
			{
				direction = 1;
			}
		}
		else
		{
			BoxCollider2D bc = GetComponent<BoxCollider2D>();
			if(bc != null)
			{
				Physics2D.IgnoreCollision(coll.collider, bc);
				
			}
		}
	}
}
