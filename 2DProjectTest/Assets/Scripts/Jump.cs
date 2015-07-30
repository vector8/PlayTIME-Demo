using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour 
{	
	public float burst = 8f;

	private bool jumped = false;
	private bool midJump = false;
	private bool canJump = true;
	private float jumpTimer = 0;
	private const float JUMP_COOLDOWN = 0.5f;
	
	// Use this for initialization
	void Start () 
	{
	}

	void Update()
	{
		Animator animator = GetComponent<Animator>();

		if(jumpTimer > 0)
		{
			jumpTimer -= Time.deltaTime;
		}
			
		if(Input.GetButton("Jump") && canJump)
		{
			jumped = true;
			canJump = false;
			jumpTimer = JUMP_COOLDOWN;
			animator.SetBool("Jumping", true);
			animator.SetBool("MidJump", false);
			midJump = false;
		}
		else if(!midJump)
		{
			animator.SetBool("MidJump", true);
			midJump = true;
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

	void OnCollisionStay2D(Collision2D collision)
	{
		if(!canJump && jumpTimer <= 0)
		{
			Vector2 right = new Vector2(1f, 0f);

			for(int i = 0; i < collision.contacts.Length; i++)
			{
				Vector2 contactDir = (Vector2)collision.gameObject.transform.position - (Vector2)gameObject.transform.position;
				float angle = Vector2.Angle(right, contactDir);
				if(contactDir.y < 0 && angle >= 45f && angle <= 135f)
				{
					canJump = true;
					Animator animator = GetComponent<Animator>();
					animator.SetBool("Jumping", false);
					animator.SetBool("MidJump", false);
					return;
				}
			}
		}
	}
}
