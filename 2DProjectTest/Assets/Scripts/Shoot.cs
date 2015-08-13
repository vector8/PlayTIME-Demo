using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{
    public GameObject projectile = null;
    public float projectileSpeed;

    private const float SHOOT_COOLDOWN = 5f;
    private float shootTimer = 0f;
    private SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (projectile != null && !LevelManager.instance.paused && sr.isVisible)
        {
            shootTimer += Time.deltaTime;

            if(shootTimer >= SHOOT_COOLDOWN)
            {
                // Find closest player and shoot at them.
                GameObject player = AIUtilities.getClosestPlayer(transform.position);

                // only shoot if player is approximately on our y-level
                if(Mathf.Abs(player.transform.position.y - transform.position.y) < 2f)
                {
                    // shoot
                    float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
                    GameObject newProjectile = LevelManager.instance.placeSpawnedObject((Vector2)gameObject.transform.position + new Vector2(direction, 0f), projectile, transform.parent);
                    Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
                    if(rb != null)
                    {
                        rb.velocity = new Vector2(projectileSpeed * direction, rb.velocity.y);
                    }
                    Vector3 scale = newProjectile.transform.localScale;
                    scale.x = Mathf.Abs(scale.x) * direction;
                    newProjectile.transform.localScale = scale;
                    shootTimer = 0f;
                }
            }
        }
    }
}
