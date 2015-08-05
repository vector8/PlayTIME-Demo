using UnityEngine;
using System.Collections;

public class Despawn : CustomAction
{
    public int deathAnimID;
    public int defaultDeathAnimID;

    private Animator anim;
    private bool dying = false;
    private bool hashSet = false;
    private int currentAnimHash;
    private int deathAnimHash;
    private string animParamName;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(dying)
        {
            // wait until dying animation is over, then despawn this object
            if(!hashSet)
            {
                if(currentAnimHash != anim.GetCurrentAnimatorStateInfo(0).shortNameHash)
                {
                    deathAnimHash = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
                    hashSet = true;
                    anim.logWarnings = false;
                    anim.SetBool(animParamName, false);
                    anim.logWarnings = true;
                }
            }
            else
            {
                if(anim.GetCurrentAnimatorStateInfo(0).shortNameHash != deathAnimHash)
                {
                    // dying animation finished
                    despawn();                    
                }
            }
        }
    }

    private void despawn()
    {
        if (GetComponent<JustSpawned>() != null)
        {
            // remove spawned object
            LevelManager.instance.removeSpawnedObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

	public override void run(GameObject other = null, int id = 0)
	{
		if(!dying && (other == null || isValidTag(other.tag)))
		{
            if(deathAnimID > 0)
            {
                if(anim == null)
                {
                    anim = GetComponent<Animator>();
                }

                if(anim != null)
                {
                    currentAnimHash = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
                    animParamName = "Dying" + deathAnimID;
                    anim.logWarnings = false;
                    anim.SetBool(animParamName, true);
                    anim.logWarnings = true;
                    dying = true;

                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if(rb != null)
                    {
                        rb.isKinematic = true;
                        rb.velocity = new Vector2();
                    }
                }
                else
                {
                    // animator not found, just despawn now
                    despawn();
                }
            }
            else
            {
			    despawn();
            }
        }
	}

	public override void reset()
	{
		gameObject.SetActive(true);
        dying = false;
        hashSet = false;
        deathAnimHash = 0;
        animParamName = "";
        deathAnimID = defaultDeathAnimID;
	}
}
