using UnityEngine;
using System.Collections;

public class Transfigure : CustomAction
{
	public string targetAnimControllerName;
	public bool reversible;

	private RuntimeAnimatorController originalAnimController = null;
	private RuntimeAnimatorController targetAnimController = null;
	private bool done = false;

	public override void run(GameObject other)
	{
		if(!done || reversible)
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
