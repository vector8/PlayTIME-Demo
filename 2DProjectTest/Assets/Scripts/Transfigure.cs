using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetAnimControllerAndTags
{
	public RuntimeAnimatorController targetAnimController;
	public List<string> includedTags;
	public List<string> excludedTags;
	public bool done;
	public bool reversible;
	public int id;

	public bool isValidTag(string tag)
	{
		if(includedTags.Count > 0)
		{
			return includedTags.Contains(tag) && !excludedTags.Contains(tag);
		}
		else
		{
			return !excludedTags.Contains(tag);
		}
	}
}

public class Transfigure : CustomAction
{
	public List<TargetAnimControllerAndTags> targetAndTags = new List<TargetAnimControllerAndTags>();

	private RuntimeAnimatorController originalAnimController = null;
	private SpriteRenderer sr;
	private BoxCollider2D bc;
	private Animator anim;

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

	public override void initialize()
	{
		anim = gameObject.GetComponent<Animator>();
		if(anim == null)
		{
			anim = gameObject.AddComponent<Animator>();
		}
		
		if(originalAnimController == null)
		{
			originalAnimController = anim.runtimeAnimatorController;
		}
	}

	public override void run(GameObject other, int id)
	{
		for(int i = 0; i < targetAndTags.Count; i++)
		{
			if(targetAndTags[i].id == id && ((!targetAndTags[i].done || targetAndTags[i].reversible) && targetAndTags[i].isValidTag(other.tag)))
			{
				if(!targetAndTags[i].done)
				{
					anim.runtimeAnimatorController = targetAndTags[i].targetAnimController;
				}
				else
				{
					anim.runtimeAnimatorController = originalAnimController;
				}
				
				targetAndTags[i].done = !targetAndTags[i].done;
			}
		}
	}

	public override void reset()
	{
		if(originalAnimController != null)
		{
			anim.runtimeAnimatorController = originalAnimController;
			foreach(TargetAnimControllerAndTags t in targetAndTags)
			{
				t.done = false;
			}
		}
	}

	public void addTargetAnimControllerAndTags(string targetAnimControllerName, List<string> includedTags, List<string> excludedTags, bool reversible, int id)
	{
		TargetAnimControllerAndTags t = new TargetAnimControllerAndTags();
		t.targetAnimController = Resources.Load<RuntimeAnimatorController>(targetAnimControllerName);
		t.includedTags = new List<string>(includedTags);
		t.excludedTags = new List<string>(excludedTags);
		t.reversible = reversible;
		t.id = id;
		t.done = false;
		targetAndTags.Add(t);
	}
}
