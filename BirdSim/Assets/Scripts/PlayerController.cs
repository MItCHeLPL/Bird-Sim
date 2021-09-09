using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Rigidbody rb;

	//Speed
	[HideInInspector] public float speed = 2.0f; //Current player speed
	[SerializeField] private float minSpeed = 2.0f; //Slowest player speed
	[SerializeField] private float maxSpeed = 8.0f; //Fastest natural player speed

	//Windboost
	private float dynamicMaxSpeed; //Fastest arbitrary player speed
	[SerializeField] private float timeToReduceDynamicMaxSpeed = 10.0f; //How long does it take to loose speed from arbirary to natural max speed

	//Coroutines
	private Coroutine SpeedRiser;
	private bool speedRiserIsRunning = false;

	private Coroutine dynamicMaxSpeedReductor;
	private bool dynamicMaxSpeedReductorIsRunning = false;

	//Bird angle speed modifications
	[SerializeField] private float downwardSpeedBoost = 0.5f; //How much speed does player gain from going downwards
	[SerializeField] private float angleToLooseSpeed = 0.15f; //At which angle can player go up without loosing speed

	[SerializeField] private float rotationSpeed = 1.0f; //How fast does player rotate

	//Inputs
	private float x;
	private float y;
	private float z;

	private Vector3 input;
	private Vector3 rotation;

	[SerializeField] private Animator anim; //Animations
	public BirdsController birdsController; //Bird particles reference

	//Trails
	[SerializeField] private List<GameObject> trails; 
	private bool trailsActivated = false;

	//Player Settings
	private int invertBirdY = 1;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();

		dynamicMaxSpeed = maxSpeed; //equal max speeds

		if (anim != null)
		{
			anim.SetBool("flying", true); //Start flying animation
		}

		//Deactivate trails
		foreach (GameObject trail in trails)
		{
			trail.SetActive(false);
		}
		trailsActivated = false;

		GetSettingsFromPlayerPrefs(); //Get player settings
	}

	void Update()
    {
		//Inputs
		x = Input.GetAxis("Horizontal"); // left right rotation (yaw)
		y = Input.GetAxis("Vertical"); // up down rotation (pitch)
		z = Input.GetAxis("HorizontalQE"); // roll rotation

		//Convert player input to bird rotations
		input = new Vector3(x, y, z);
		rotation = new Vector3(invertBirdY * y, x, -z);

		//Calculate player speed
		//angle gets angleToLooseSeed subtracted from it, so while flying at angle greater than 0, player is still gaining speed
		//fly down - get speed (angle * 2), fly up - loose speed (angle * 1)
		float angle;

		if(transform.forward.y - angleToLooseSpeed < 0) //flying down
		{
			angle = (transform.forward.y - angleToLooseSpeed) * 2;
		}
		else //flying up
		{
			angle = transform.forward.y - angleToLooseSpeed;
		}
		
		//When not gaining speed arbitrarly
		if(speedRiserIsRunning == false)
		{
			speed = Mathf.Clamp(speed - angle * downwardSpeedBoost * Time.deltaTime, minSpeed, dynamicMaxSpeed); //Calculate player speed
		}
		
		//Animation
		if (anim != null)
		{
			anim.SetFloat("flyingDirectionX", x); //Left right rotation (yaw)
			anim.SetFloat("flyingDirectionY", y); //Left right rotation (yaw)
		}

		//Trails
		//Activate all trails when achieved natural max speed
		if (speed >= maxSpeed && trailsActivated == false)
		{
			foreach (GameObject trail in trails)
			{
				trail.SetActive(true);
			}
			trailsActivated = true;
		}
		else if (speed < maxSpeed && trailsActivated == true)
		{
			foreach (GameObject trail in trails)
			{
				trail.SetActive(false);
			}
			trailsActivated = false;
		}

		//Debug
		//Debug.Log("input: " + input);
		//Debug.Log("speed: " + speed);
		//Debug.Log("dynamicMaxSpeed: " + dynamicMaxSpeed);
		//Debug.Log("velocity: " + rb.velocity.magnitude + ", " + rb.velocity);
		//Debug.Log("Transform forward: " + transform.forward.magnitude + ", " + transform.forward);
	}

	private void FixedUpdate()
	{
		//rotate player
		transform.Rotate(rotation * rotationSpeed, Space.Self);

		//move player towards rotation with player's speed
		rb.velocity = transform.forward * speed;
	}

	public void RiseDynamicMaxSpeed(float newDynamicMaxSpeedValue, float timeToRiseSpeed)
	{
		if (dynamicMaxSpeedReductorIsRunning && dynamicMaxSpeedReductor != null)
		{
			StopCoroutine(dynamicMaxSpeedReductor); //If reducing dynamicMaxSpeed, stop

			dynamicMaxSpeedReductorIsRunning = false;

			//Debug.Log("Stopped dynamicMaxSpeedReductor");
		}

		dynamicMaxSpeed = newDynamicMaxSpeedValue; //rise max dynamic speed

		SpeedRiser = StartCoroutine(RiseSpeedCoroutine(timeToRiseSpeed)); //Rise player speed to match the new dynamic max speed
	}

	public void ReduceDynamicMaxSpeed()
	{
		if (speedRiserIsRunning && SpeedRiser != null)
		{
			StopCoroutine(SpeedRiser); //If rising speed, stop

			speedRiserIsRunning = false;

			//Debug.Log("Stopped SpeedRiser");
		}
		
		dynamicMaxSpeedReductor = StartCoroutine(ReduceDynamicMaxSpeedCoroutine()); //Start reducing dynamic max speed over time
	}

	private IEnumerator RiseSpeedCoroutine(float timeToRiseSpeed)
	{
		speedRiserIsRunning = true;

		float t = 0f;
		float startSpeed = speed;

		//Debug.Log("Started RiseSpeedCoroutine, speed: " + speed);

		while (t < 1)
		{
			t += Time.deltaTime / timeToRiseSpeed;
			speed = Mathf.Lerp(startSpeed, dynamicMaxSpeed, t); //Rise speed to match arbitrary max speed

			//Debug.Log("speed: " + speed);

			yield return null;
		}

		speed = dynamicMaxSpeed; //Round speed
		speedRiserIsRunning = false;

		//Debug.Log("Finished RiseSpeedCoroutine, speed: " + speed);
	}

	private IEnumerator ReduceDynamicMaxSpeedCoroutine()
	{
		dynamicMaxSpeedReductorIsRunning = true;
		
		float t = 0f;
		float startDynamicMaxSpeed = dynamicMaxSpeed;

		//Debug.Log("Started ReduceDynamicMaxSpeedCoroutine, dynamicMaxSpeedCoroutine: " + dynamicMaxSpeed);

		while (t < 1)
		{
			t += Time.deltaTime / timeToReduceDynamicMaxSpeed;
			dynamicMaxSpeed = Mathf.Lerp(startDynamicMaxSpeed, maxSpeed, t); //Reduce speed back to natural max speed

			//Debug.Log("dynamicMaxSpeed: " + dynamicMaxSpeed);

			yield return null;
		}

		dynamicMaxSpeed = maxSpeed; //Round max speeds
		dynamicMaxSpeedReductorIsRunning = false;

		//Debug.Log("Finished ReduceDynamicMaxSpeedCoroutine, dynamicMaxSpeedCoroutine: " + dynamicMaxSpeed);
	}

	public void GetSettingsFromPlayerPrefs()
	{
		invertBirdY = PlayerPrefs.GetInt(("Options_BirdInvertY")) == 1 ? 1 : -1; //Get Invert input user setting
	}
}