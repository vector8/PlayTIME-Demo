using UnityEngine;
using System.Collections;

public class Bounce : CustomAction
{
    private Rigidbody2D rb;

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
        }
    }

    public override void reset()
    {
    }
}
