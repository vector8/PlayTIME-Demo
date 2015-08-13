using UnityEngine;
using System.Collections;

public class JumpAI : MonoBehaviour
{
    public float burst = 8f;

    private bool jumped = false;
    private bool canJump = true;
    private const float JUMP_COOLDOWN = 5f;
    private float jumpTimer = JUMP_COOLDOWN;
    private SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!LevelManager.instance.paused)
        {
            if (jumpTimer > 0)
            {
                jumpTimer -= Time.deltaTime;
            }

            if (canJump)
            {
                jumped = true;
                canJump = false;
                jumpTimer = JUMP_COOLDOWN;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LevelManager.instance.paused)
        {
            if (jumped)
            {
                Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(rb.velocity.x, burst);
                jumped = false;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        if (sr.isVisible && !canJump && jumpTimer <= 0)
        {
            Vector2 right = new Vector2(1f, 0f);

            for (int i = 0; i < collision.contacts.Length; i++)
            {
                Vector2 contactDir = (Vector2)collision.gameObject.transform.position - (Vector2)gameObject.transform.position;
                float angle = Vector2.Angle(right, contactDir);
                if (contactDir.y < 0 && angle >= 45f && angle <= 135f)
                {
                    canJump = true;
                    return;
                }
            }
        }
    }
}
