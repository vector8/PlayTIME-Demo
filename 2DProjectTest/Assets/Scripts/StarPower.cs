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
	private RuntimeAnimatorController originalAnimator;
	private RuntimeAnimatorController starPowerAnimator;
	private RuntimeAnimatorController bigStarPowerAnimator;
	private SpriteRenderer sr;
	private BoxCollider2D bc;

	// Use this for initialization
	void Start () 
	{
		health = GetComponent<Health>();
		anim = GetComponent<Animator>();
		originalAnimator = anim.runtimeAnimatorController;
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
				reset();
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
			health.enabled = false;
			originalAnimator = anim.runtimeAnimatorController;
			if(originalAnimator.name == "Mario")
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
		else if(running && coll.gameObject.tag == "Mushroom")
		{
			originalAnimator = Resources.Load<RuntimeAnimatorController>("BigMario");
			anim.runtimeAnimatorController = bigStarPowerAnimator;
		}
		else if(running && coll.gameObject.tag == "Enemy")
		{
			coll.gameObject.SetActive(false);
		}
	}

	public void reset()
	{
		running = false;
		durationTimer = 0f;
		health.enabled = true;
		anim.runtimeAnimatorController = originalAnimator;
		for(int i = 0; i < transfigures.Length; i++)
		{
			transfigures[i].enabled = true;
		}
	}
}
