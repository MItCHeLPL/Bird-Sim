using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private Rigidbody rb;
	private float speed = 50.0f;
	[SerializeField] private float speedBoost = 0.0f;
	[SerializeField] private float maxSpeed = 250.0f;
	private float x;
	private float y;
	private float z;
	private Camera cam;

	void Start()
    {
		rb = GetComponent<Rigidbody>();
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); //change to dataholder active camera
    }

    void Update()
    {
		x = Input.GetAxis("Vertical");
		y = Input.GetAxis("Horizontal");
		z = Input.GetAxis("HorizontalQE");

		if(Input.GetKey(KeyCode.Space))
		{
			speed = Mathf.Clamp(speed + speedBoost * Time.deltaTime, 0, maxSpeed);
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = Mathf.Clamp(speed - speedBoost * Time.deltaTime, 0, maxSpeed);
		}

		Debug.Log(speed + "  " + x + "  " + y + "   " + z);
	}

	private void FixedUpdate()
	{
		transform.Rotate(new Vector3(x, y, z), Space.Self);

		rb.MovePosition(transform.position + cam.transform.forward * speed * Time.deltaTime);
	}
}
