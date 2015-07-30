using UnityEngine;
using System.Collections;

public class SuperStarMovement : MonoBehaviour 
{
	public float velocity = 3f;
	private float direction = -1f;
	private float timer = 0f;
	public float MAX_VERTICAL_TRAVEL_TIME = 1.2f;

	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		if(timer > MAX_VERTICAL_TRAVEL_TIME)
		{
			direction *= -1;
			timer = 0f;
		}

		Vector3 position = gameObject.transform.position;
		position.x += velocity * Time.deltaTime;
		position.y += direction * velocity * Time.deltaTime;
		gameObject.transform.position = position;
	}
}
