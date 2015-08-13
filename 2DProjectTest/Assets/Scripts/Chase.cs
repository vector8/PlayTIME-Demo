using UnityEngine;
using System.Collections;

public class Chase : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        if(!sr.isVisible)
        {
            return;
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Find closest player and move toward it
        GameObject player = AIUtilities.getClosestPlayer(transform.position);

        if(player != null && rb != null)
        {
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

            Vector2 position = transform.position;
            position.x += (direction*moveSpeed) * Time.deltaTime;
            transform.position = position;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -direction;
            transform.localScale = scale;
        }
    }
}
