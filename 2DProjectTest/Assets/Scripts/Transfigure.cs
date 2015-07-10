using UnityEngine;
using System.Collections;

public class Transfigure : CustomAction
{
	public string targetAnimControllerName;
	public bool reversible;
	public string targetTag;

	private RuntimeAnimatorController originalAnimController = null;
	private RuntimeAnimatorController targetAnimController = null;
	private bool done = false;
	private SpriteRenderer sr;
	private BoxCollider2D bc;

	void Update()
	{
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

	public override void run(GameObject other)
	{
		if((!done || reversible) && other.tag.Equals(targetTag))
		{
			Animator anim = gameObject.GetComponent<Animator>();
			if(anim == null)
			{
				anim = gameObject.AddComponent<Animator>();
			}
			else if(originalAnimController == null)
			{
				originalAnimController = anim.runtimeAnimatorController;
			}

			if(targetAnimController == null)
			{
				targetAnimController = Resources.Load<RuntimeAnimatorController>(targetAnimControllerName);
			}

			if(!done)
			{
				anim.runtimeAnimatorController = targetAnimController;
			}
			else
			{
				anim.runtimeAnimatorController = originalAnimController;
			}

			done = !done;
		}
	}

	public override void reset()
	{
		if(originalAnimController != null)
		{
			Animator anim = gameObject.GetComponent<Animator>();
			anim.runtimeAnimatorController = originalAnimController;
			done = false;
		}
	}
}
