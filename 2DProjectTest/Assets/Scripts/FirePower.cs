using UnityEngine;
using System.Collections;

public class FirePower : MonoBehaviour, ICanReset
{
    private Animator anim;
    private RuntimeAnimatorController marioAnimator;
    private RuntimeAnimatorController fireMarioAnimator;
    private RuntimeAnimatorController bigMarioAnimator;
    private RuntimeAnimatorController bigMarioStarPowerAnimator;
    private bool active = false;

    private float fireballTimer = 0f;
    private const float FIREBALL_COOLDOWN = 0.25f;

    private Spawn fireballSpawn;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        marioAnimator = Resources.Load<RuntimeAnimatorController>("Mario");
        fireMarioAnimator = Resources.Load<RuntimeAnimatorController>("FireMario");
        bigMarioAnimator = Resources.Load<RuntimeAnimatorController>("BigMario");
        bigMarioStarPowerAnimator = Resources.Load<RuntimeAnimatorController>("BigMarioStarPower");
    }

    // Update is called once per frame
    void Update()
    {
        if(fireballTimer < FIREBALL_COOLDOWN)
        {
            fireballTimer += Time.deltaTime;
        }

        if (active && Input.GetButtonDown("Fire1") && fireballTimer >= FIREBALL_COOLDOWN)
        {
            if (fireballSpawn == null)
            {
                fireballSpawn = GetComponent<Spawn>();
            }

            fireballSpawn.spawnUnderParent = true;
            fireballSpawn.run();
            fireballSpawn.currentSpawnCount++;
            Rigidbody2D rb = fireballSpawn.lastSpawnedObject.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(5f * gameObject.transform.localScale.x, rb.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (!active && coll.gameObject.tag == "FireFlower")
        {
            if(anim.runtimeAnimatorController.name == "BigMario")
            {
                active = true;
                anim.runtimeAnimatorController = fireMarioAnimator;
            }
            else if(anim.runtimeAnimatorController.name == "BigMarioStarPower")
            {
                active = true;
            }
            else if(anim.runtimeAnimatorController.name == "Mario")
            {
                anim.runtimeAnimatorController = bigMarioAnimator;
            }
            else if(anim.runtimeAnimatorController.name == "MarioStarPower")
            {
                anim.runtimeAnimatorController = bigMarioStarPowerAnimator;
            }
        }
    }

    public void reset()
    {
        active = false;
        anim.runtimeAnimatorController = marioAnimator;
    }
}
