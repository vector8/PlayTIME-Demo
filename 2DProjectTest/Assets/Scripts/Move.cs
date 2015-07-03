using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour 
{	
	public const int UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3;
	public float[] maxSpeed = {4f, 4f, 4f, 4f};
	public float[] currentSpeed = {0f, 0f, 0f, 0f};

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animator animator = GetComponent<Animator>();

		float horizSpeed = Input.GetAxis("Horizontal");
		float vertSpeed = Input.GetAxis("Vertical");

		if(!Mathf.Approximately(horizSpeed, 0f))
		{
			if(animator != null)
			{
				animator.SetBool("HorizontalMovement", true);
			}

			if(horizSpeed > 0)
			{
				currentSpeed[RIGHT] = horizSpeed * maxSpeed[RIGHT];
				currentSpeed[LEFT] = 0;
				Vector3 scale = transform.localScale;
				scale.x = Mathf.Abs(scale.x);
				transform.localScale = scale;
			}
			else
			{
				currentSpeed[LEFT] = -horizSpeed * maxSpeed[LEFT];
				currentSpeed[RIGHT] = 0;
				Vector3 scale = transform.localScale;
				scale.x = -Mathf.Abs(scale.x);
				transform.localScale = scale;
			}
		}
		else
		{
			if(animator != null)
			{
				animator.SetBool("HorizontalMovement", false);
			}
			currentSpeed[LEFT] = 0f;
			currentSpeed[RIGHT] = 0f;
		}

		if(vertSpeed > 0)
		{
			currentSpeed[UP] = vertSpeed * maxSpeed[UP];
			currentSpeed[DOWN] = 0;
		}
		else
		{
			currentSpeed[DOWN] = -vertSpeed * maxSpeed[DOWN];
			currentSpeed[UP] = 0;
		}

		Vector3	pos = gameObject.transform.position;
		pos.x += currentSpeed[RIGHT] * Time.deltaTime;
		pos.x -= currentSpeed[LEFT] * Time.deltaTime;
		pos.y += currentSpeed[UP] * Time.deltaTime;
		pos.y -= currentSpeed[DOWN] * Time.deltaTime;
		gameObject.transform.position = pos;
	}

	public void setMaxSpeed(float speed)
	{
		for(int i = 0; i < 4; i++)
		{
			if(!Mathf.Approximately(maxSpeed[i], 0))
			{
				maxSpeed[i] = speed;
			}
		}
	}
}
