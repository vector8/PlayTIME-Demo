using UnityEngine;
using System;
using System.Collections.Generic;

public class CollideTrigger : MonoBehaviour 
{
	public List<CustomAction> actions = new List<CustomAction>();
	public List<int> directions = new List<int>();

	private float topAngle, sideAngle;
	
	private enum CollideDirections
	{
		Top = 1,
		Right = 2,
		Bottom = 4,
		Left = 8
	}

	void Start()
	{
		Vector2 size = GetComponent<BoxCollider2D>().size;
		size = Vector2.Scale(size, (Vector2)transform.localScale);
		topAngle = Mathf.Atan(size.x / size.y) * Mathf.Rad2Deg;
		sideAngle = 90.0f - topAngle;
	}

	public void initialize()
	{
		foreach(CustomAction a in actions)
		{
			a.initialize();
		}
	}

	public void reset() 
	{
		foreach(CustomAction a in actions)
		{
			a.reset();
		}
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        triggerCollision(coll.gameObject);
    }
	
	void OnCollisionEnter2D(Collision2D coll)
	{
        triggerCollision(coll.gameObject);
	}

    private void triggerCollision(GameObject other)
    {
        JustSpawned js = other.GetComponent<JustSpawned>();
        if (js != null && js.enabled)
        {
            return;
        }

        Vector2 v = (Vector2)(other.transform.position - transform.position);

        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].enabled &&
                (Vector2.Angle(v, transform.up) <= topAngle && directionsInclude(directions[i], CollideDirections.Top) ||
                Vector2.Angle(v, transform.right) <= sideAngle && directionsInclude(directions[i], CollideDirections.Right) ||
                Vector2.Angle(v, -transform.up) <= topAngle && directionsInclude(directions[i], CollideDirections.Bottom) ||
                Vector2.Angle(v, -transform.right) <= sideAngle && directionsInclude(directions[i], CollideDirections.Left)))
            {
                LevelManager.instance.addActionQueueItem(actions[i], other, i);
            }
        }
    }
	
	private bool directionsInclude(int directions, CollideDirections collisionDirection)
	{
		bool value = (directions & (int)collisionDirection) == (int)collisionDirection;
		return value;
	}
}
