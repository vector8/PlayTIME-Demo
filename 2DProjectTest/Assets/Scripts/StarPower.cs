using UnityEngine;
using System.Collections;

public class StarPower : MonoBehaviour 
{
	private const float DURATION = 10f;
	private const int DAMAGE = 5;

	private float durationTimer = 0f;
	private bool running = false;
	private Health health;
	private Transfigure[] transfigures;
	
	private Animator anim;
	private RuntimeAnimatorController resetAnimator;
    private RuntimeAnimatorController marioAnimator;
    private RuntimeAnimatorController bigMarioAnimator;
    private RuntimeAnimatorController fireMarioAnimator;
    private RuntimeAnimatorController starPowerAnimator;
	private RuntimeAnimatorController bigStarPowerAnimator;
	private SpriteRenderer sr;
	private BoxCollider2D bc;

	// Use this for initialization
	void Start () 
	{
		health = GetComponent<Health>();
		anim = GetComponent<Animator>();
        resetAnimator = anim.runtimeAnimatorController;
        marioAnimator = Resources.Load<RuntimeAnimatorController>("Mario");
        bigMarioAnimator = Resources.Load<RuntimeAnimatorController>("BigMario");
        fireMarioAnimator = Resources.Load<RuntimeAnimatorController>("FireMario");
		starPowerAnimator = Resources.Load<RuntimeAnimatorController>("MarioStarPower");
		bigStarPowerAnimator = Resources.Load<RuntimeAnimatorController>("BigMarioStarPower");
		transfigures = GetComponents<Transfigure>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(running)
		{
			if(durationTimer < DURATION)
			{
				durationTimer += Time.deltaTime;
			}

			if(durationTimer >= DURATION)
			{
				reset(false);
			}
		}

		// Resize BoxCollider2D in case the size of the sprite has changed
		if(sr == null)
		{
			sr = gameObject.GetComponent<SpriteRenderer>();
		}
		if(bc == null)
		{
			bc = gameObject.GetComponent<BoxCollider2D>();
		}
		if(sr != null && bc != null)
		{
			bc.size = sr.bounds.size;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(!running && coll.gameObject.tag == "SuperStar")
		{
			running = true;
			health.allowDamage = false;
            resetAnimator = anim.runtimeAnimatorController;
            if (resetAnimator.name == "Mario")
			{
				anim.runtimeAnimatorController = starPowerAnimator;
			}
			else
			{
				anim.runtimeAnimatorController = bigStarPowerAnimator;
			}
			for(int i = 0; i < transfigures.Length; i++)
			{
				transfigures[i].enabled = false;
			}
        }
        else if (running && coll.gameObject.tag == "Mushroom")
        {
            if(resetAnimator.name != "FireMario")
            {
                resetAnimator = bigMarioAnimator;
            }
            anim.runtimeAnimatorController = bigStarPowerAnimator;
        }
		else if(running && coll.gameObject.tag == "FireFlower")
		{
            if (resetAnimator.name == "BigMario")
            {
                resetAnimator = fireMarioAnimator;
            }
            else
            {
                resetAnimator = bigMarioAnimator;
            }
			anim.runtimeAnimatorController = bigStarPowerAnimator;
		}
		else if(running && coll.gameObject.tag == "Enemy")
		{
			coll.gameObject.SetActive(false);
		}
	}

	public void reset(bool toOriginal = true)
	{
		running = false;
		durationTimer = 0f;
		health.allowDamage = true;
        if(toOriginal)
        {
            anim.runtimeAnimatorController = marioAnimator;
        }
        else
        {
            anim.runtimeAnimatorController = resetAnimator;
        }
		for(int i = 0; i < transfigures.Length; i++)
		{
			transfigures[i].enabled = true;
		}
	}
}
