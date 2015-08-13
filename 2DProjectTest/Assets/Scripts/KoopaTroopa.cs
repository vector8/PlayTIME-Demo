using UnityEngine;
using System.Collections;

public class KoopaTroopa : MonoBehaviour, ICanReset
{
	private bool dead = false;
	private bool kicked = false;
	private Animator anim;
	private MoveHorizontalUntilCollision mh;
	private Damage d;
	private float deathDelayTimer = 0f;
	private float reviveTimer = 0f;
    private float kickDamageDelayTimer = 0f;
    private float moveHorizOriginalSpeed;

	private const float DEATH_DELAY = 0.5f;
	private const float REVIVE_TIME = 5f;
    private const float KICK_DAMAGE_DELAY = 0.5f;
    private const float SHELL_MOVE_SPEED = 6f;

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
        if(!LevelManager.instance.paused)
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
                        deathDelayTimer = 0f;
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
                if (!d.enabled)
                {
                    kickDamageDelayTimer += Time.deltaTime;

                    if (kickDamageDelayTimer >= KICK_DAMAGE_DELAY)
                    {
                        d.enabled = true;
                        kickDamageDelayTimer = 0f;
                    }
                }

                SpriteRenderer sr = GetComponent<SpriteRenderer>();
			    if(!sr.isVisible)
			    {
				    gameObject.SetActive(false);
			    }
		    }
        }
	}

	void OnCollisionStay2D(Collision2D coll)
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
                mh.speed = SHELL_MOVE_SPEED;
				kicked = true;
				anim.SetFloat("ReviveTimer", 0f);
				d.includedTags.Add("Enemy");
			}
		}
	}

	public void reset()
	{
		gameObject.SetActive(true);
        deathDelayTimer = 0f;
        reviveTimer = 0f;
        kickDamageDelayTimer = 0f;
        kicked = false;
		dead = false;
		mh.enabled = true;
        mh.reset();
        mh.speed = moveHorizOriginalSpeed;
        mh.run();
		anim.SetBool("Dead", false);
		anim.SetFloat("ReviveTimer", 0f);
		d.includedTags.Remove("Enemy");
        d.enabled = true;
	}
}
