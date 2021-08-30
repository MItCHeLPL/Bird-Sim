using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Rigidbody rb;

	[HideInInspector] public float speed = 2.0f;
	[SerializeField] private float minSpeed = 2.0f;
	[SerializeField] private float maxSpeed = 8.0f;
	[SerializeField] private float speedBoost = 0.5f;
	[SerializeField] private float angleToLooseSeed = 0.15f;

	[SerializeField] private float rotationSpeed = 1.0f;

	private float x;
	private float y;
	private float z;

	private Vector3 input;
	private Vector3 rotation;

	[SerializeField] private Animator anim;

	void Start()
    {
		rb = GetComponent<Rigidbody>();

		if (anim != null)
		{
			anim.SetBool("flying", true); //Start flying animation
		}
	}

    void Update()
    {
		//Inputs
		x = Input.GetAxis("Horizontal"); // left right rotation (yaw)
		y = Input.GetAxis("Vertical"); // up down rotation (pitch)
		z = Input.GetAxis("HorizontalQE"); // roll rotation

		input = new Vector3(x, y, z);
		rotation = new Vector3(y, x, -z);

		//Calculate player speed
		//angle gets angleToLooseSeed subtracted from it so for example while flying straight, player is still gaining speed
		//fly down - get speed (angle * 2), fly up - loose speed (angle * 1)
		float angle;

		if(transform.forward.y - angleToLooseSeed < 0)
		{
			angle = (transform.forward.y - angleToLooseSeed) * 2;
		}
		else
		{
			angle = transform.forward.y - angleToLooseSeed;
		}
		
		speed = Mathf.Clamp(speed - angle * speedBoost * Time.deltaTime, minSpeed, maxSpeed);

		//Animation
		if (anim != null)
		{
			anim.SetFloat("flyingDirectionX", x); //Left right rotation (yaw)
			anim.SetFloat("flyingDirectionY", y); //Left right rotation (yaw)
		}

		//Debug
		/*Debug.Log("input: " + input);
		Debug.Log("speed: " + speed);
		Debug.Log("velocity: " + rb.velocity.magnitude + ", " + rb.velocity);
		Debug.Log("Transform forward: " + transform.forward.magnitude + ", " + transform.forward);*/
	}

	private void FixedUpdate()
	{
		//rotate player
		transform.Rotate(rotation * rotationSpeed, Space.Self);

		//move player towards rotation with player's velocity
		rb.velocity = transform.forward * speed;
		//rb.AddForce(transform.forward * speed);

		//Limit player speed
		//rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
	}
}