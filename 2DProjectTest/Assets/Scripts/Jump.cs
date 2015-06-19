using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour 
{	
	public float burst = 8f;

	private bool jumped = false;
	private bool canJump = true;
	
	// Use this for initialization
	void Start () 
	{
	}

	void Update()
	{
		if(Input.GetButton("Jump") && canJump)
		{
			jumped = true;
			canJump = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(jumped)
		{
			Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
			rb.AddRelativeForce(new Vector2(0f, burst), ForceMode2D.Impulse);
			jumped = false;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Vector2 right = new Vector2(1f, 0f);

		for(int i = 0; i < collision.contacts.Length; i++)
		{
			Vector2 contactDir = collision.contacts[i].point - (Vector2)gameObject.transform.position;
			float angle = Vector2.Angle(right, contactDir);
			if(contactDir.y < 0 && angle > 50f && angle < 130f)
			{
				canJump = true;
				print ("Jump reset");
				return;
			}
		}
	}
}
