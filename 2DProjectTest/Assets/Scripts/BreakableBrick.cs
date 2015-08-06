using UnityEngine;
using System.Collections;

public class BreakableBrick : CustomAction 
{
    private bool bounce = false;
    private Vector2 startPos;
    private Vector2 endPos;
    private float bounceDuration = 0.2f;
    private float currentTime = 0f;
    private bool goingUp = true;
    private Vector2 minibrickMultiplier = new Vector2(3f, 6f);

    private Vector2[] minibrickDirections = { new Vector2(-0.866f, 0.5f), new Vector2(-0.5f, 0.866f), new Vector2(0.5f, 0.866f), new Vector2(0.866f, 0.5f) };

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + new Vector2(0f, 0.5f);

        for(int i = 0; i < 4; i++)
        {
            minibrickDirections[i] = Vector2.Scale(minibrickDirections[i], minibrickMultiplier);
        }
    }

    void Update()
    {
        if(bounce)
        {
            if(goingUp)
            {
                Vector2 currentPos = easeOutQuad(currentTime, startPos, endPos - startPos, bounceDuration);
                gameObject.transform.position = currentPos;
                currentTime += Time.deltaTime;

                if(currentTime >= bounceDuration)
                {
                    goingUp = false;
                    currentTime = 0f;
                }
            }
            else
            {
                Vector2 currentPos = easeInQuad(currentTime, endPos, startPos - endPos, bounceDuration);
                gameObject.transform.position = currentPos;
                currentTime += Time.deltaTime;

                if (currentTime >= bounceDuration)
                {
                    bounce = false;
                    goingUp = true;
                    currentTime = 0f;
                }
            }
        }
    }

    public override void run(GameObject other = null, int id = 0)
    {
        if(other != null && other.tag == "Player")
        {
            Health h = other.GetComponent<Health>();

            if(h != null)
            {
                if(h.hp == 2)
                {
                    // break the brick
                    Spawn s = GetComponent<Spawn>();
                    s.spawnUnderParent = true;
                    Rigidbody2D rb;

                    for (int i = 0; i < 4; i++)
                    {
                        s.run();
                        rb = s.lastSpawnedObject.GetComponent<Rigidbody2D>();
                        rb.velocity = minibrickDirections[i];
                    }

                    Despawn d = GetComponent<Despawn>();
                    d.run();
                }
                else if(!bounce && h.hp == 1)
                {
                    // make the brick bounce
                    bounce = true;
                }
            }
        }
    }

    public override void reset()
    {
        bounce = false;
        currentTime = 0f;
        goingUp = true;
        transform.position = startPos;
        Spawn s = GetComponent<Spawn>();
        s.reset();
    }

    // Accelerate from zero
    Vector2 easeInQuad(float curTime, Vector2 start, Vector2 change, float duration) 
    {
        curTime /= duration;
        return change * curTime * curTime + start;
    }
    
    // Decelerate to zero
    Vector2 easeOutQuad(float curTime, Vector2 start, Vector2 change, float duration)
    {
        curTime /= duration;
        return -change * curTime * (curTime - 2) + start;
    }
}
