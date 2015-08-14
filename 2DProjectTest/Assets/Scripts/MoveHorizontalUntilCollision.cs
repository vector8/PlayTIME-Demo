using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveHorizontalUntilCollision : CustomAction 
{
	public float speed = 1f;
	public List<string> tagsToIgnore;

	private int direction = -1;
	private float sideAngle;
	private Animator anim;
	private bool running = false;

    private SpriteRenderer sr;

	public override void run(GameObject other = null, int id = 0)
	{
		running = true;
	}

	public override void reset()
	{
        direction = -1;
        Vector3 scale = gameObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        gameObject.transform.localScale = scale;
        running = false;
		if(anim == null)
		{
			anim = GetComponent<Animator>();
		}
		if(anim != null)
		{
			anim.SetBool("HorizontalMovement", true);
		}
	}

	// Use this for initialization
	void Start () 
	{
        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        bool visible = true;
        
        if(sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        if(sr != null)
        {
            visible = sr.isVisible;
        }

		if(running && !LevelManager.instance.paused && visible)
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
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(running)
		{
			if(!tagsToIgnore.Contains(coll.gameObject.tag))
			{
				Vector2 v = (Vector2)(coll.gameObject.transform.position - transform.position);
				Vector3 scale = gameObject.transform.localScale;
				
				if (Vector2.Angle(v, transform.right) <= 40f)
				{
					direction = -1;
					scale.x = Mathf.Abs(scale.x);

				}
				else if(Vector2.Angle(v, -transform.right) <= 40f)
				{
					direction = 1;
					scale.x = -Mathf.Abs(scale.x);
				}

				gameObject.transform.localScale = scale;
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
}
