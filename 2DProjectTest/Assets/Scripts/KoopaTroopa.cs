using UnityEngine;
using System.Collections;

public class KoopaTroopa : MonoBehaviour, ICanReset
{
	private bool dead = false;
	private bool kicked = false;
	private Animator anim;
	private MoveHorizontalUntilCollision mh;
	private Damage d;
	private float deathDelayTimer = 0;
	private float reviveTimer = 0;
	private float moveHorizOriginalSpeed;

	private const float DEATH_DELAY = 0.5f;
	private const float REVIVE_TIME = 5f;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		mh = GetComponent<MoveHorizontalUntilCollision>();
		if(mh != null)
		{
			moveHorizOriginalSpeed = mh.speed;
		}
		d = GetComponent<Damage>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(anim == null)
		{
			anim = GetComponent<Animator>();
		}

		if(mh == null)
		{
			mh = GetComponent<MoveHorizontalUntilCollision>();
			moveHorizOriginalSpeed = mh.speed;
		}

		if(d == null)
		{
			d = GetComponent<Damage>();
		}

		if(dead && !kicked)
		{
			if(deathDelayTimer >= DEATH_DELAY && reviveTimer < REVIVE_TIME)
			{
				reviveTimer += Time.deltaTime;

				if(reviveTimer >= REVIVE_TIME)
				{
					dead = false;
					anim.SetBool("Dead", false);
					reviveTimer = 0f;
					mh.enabled = true;
					mh.speed = moveHorizOriginalSpeed;
					d.enabled = true;
				}
				anim.SetFloat("ReviveTimer", reviveTimer);
			}
			else if(deathDelayTimer < DEATH_DELAY)
			{
				deathDelayTimer += Time.deltaTime;
			}
		}
		else if(kicked)
		{
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if(!sr.isVisible)
			{
				gameObject.SetActive(false);
			}
		}

	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(coll.gameObject.tag == "Player")
		{
			if(!dead)
			{
				Vector2 v = (Vector2)(coll.gameObject.transform.position - transform.position);
				
				if(Vector2.Angle(v, transform.up) <= 45f)
				{
					dead = true;
					reviveTimer = 0f;
					anim.SetFloat("ReviveTimer", 0f);
					anim.SetBool("Dead", true);
					moveHorizOriginalSpeed = mh.speed;
					mh.enabled = false;
					d.enabled = false;
				}
			}
			else if(deathDelayTimer >= DEATH_DELAY && !kicked)
			{
				mh.enabled = true;
				mh.speed = 4f;
				kicked = true;
				anim.SetFloat("ReviveTimer", 0f);
				d.enabled = true;
				d.includedTags.Add("Enemy");
			}
		}
	}

	public void reset()
	{
		gameObject.SetActive(true);
		kicked = false;
		dead = false;
		mh.enabled = true;
		mh.speed = moveHorizOriginalSpeed;
		anim.SetBool("Dead", false);
		anim.SetFloat("ReviveTimer", 0f);
		d.includedTags.Remove("Enemy");
	}
}
