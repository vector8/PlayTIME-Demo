using UnityEngine;
using System.Collections;

public class Bounce : CustomAction
{
    private Rigidbody2D rb;
    private Animator anim;

    public float bounceHeight;

    public override void run(GameObject other = null, int id = 0)
    {
        if(other == null || isValidTag(other.tag))
        {
            if (rb == null)
            {
                rb = gameObject.GetComponent<Rigidbody2D>();
            }

            rb.velocity = new Vector2(rb.velocity.x, bounceHeight);

            if(anim == null)
            {
                anim = GetComponent<Animator>();
            }

            if(anim != null)
            {
                anim.logWarnings = false;
                anim.SetBool("Jumping", true);
                anim.SetBool("MidJump", true);
                anim.logWarnings = true;
            }
        }
    }

    public override void reset()
    {
    }
}
