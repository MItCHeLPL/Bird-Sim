﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Rigidbody rb;

	[Header("Speed")]
	//Speed
	[HideInInspector] public float speed = 2.0f; //Current player speed
	[SerializeField] private float minSpeed = 2.0f; //Slowest player speed
	[SerializeField] private float maxSpeed = 12.0f; //Fastest natural player speed

	//Stun on collision
	[SerializeField] private float timeToReduceStun= 3.0f;

	//Windboost
	private float dynamicMaxSpeed; //Fastest arbitrary player speed
	[SerializeField] private float timeToReduceDynamicMaxSpeed = 10.0f; //How long does it take to loose speed from arbirary to natural max speed

	//Bird angle speed modifications
	[SerializeField] private float downwardSpeedBoost = 0.75f; //How much speed does player gain from going downwards
	[SerializeField] private float angleToLooseSpeed = 0.15f; //At which angle can player go up without loosing speed

	[SerializeField] private float rotationSpeed = 1.0f; //How fast does player rotate


	[Header("Collision")]
	public LayerMask ignoreCollisionLayers;


	//Inputs
	private float x;
	private float y;
	private float z;

	private Vector3 input;
	private Vector3 rotation;


	[Header("Birds")]
	public BirdsController birdsController; //Bird particles reference


	[Header("Animation")]
	[SerializeField] private Animator anim;

	//Animation smoothing
	[SerializeField] private float animationSmoothingSpeed = 3.0f;
	private float animatedX = 0.0f;
	private float animatedY = 0.0f;

	//Animation speed
	[SerializeField] private bool animationSpeedBasedOnAngle = true;
	[SerializeField] private float minAnimationSpeed = 0.25f;
	[SerializeField] private float maxAnimationSpeed = 1.5f;


	//Trails
	[Header("Trails")]
	[SerializeField] private List<TrailRenderer> trails;
	[SerializeField] private bool trailsActivated = true;
	[SerializeField] private float speedToActivateTrails = 10.0f;
	[SerializeField] private float maxTrailTime = 0.1f;


	//Particles
	[Header("Particles")]
	[SerializeField] private ParticleSystem feathersParticles;
	[SerializeField] private int minParticleCountOnStun = 5;
	[SerializeField] private int maxParticleCountOnStun = 50;


	//Player Settings
	private int invertBirdY = 1;


	//Audio
	[Header("Audio")]
	public AudioController audioController;
	[SerializeField] private string impactAudioName;


	//Coroutines
	private Coroutine SpeedRiser;
	private bool speedRiserIsRunning = false;

	private Coroutine dynamicMaxSpeedReductor;
	private bool dynamicMaxSpeedReductorIsRunning = false;


	void Awake()
	{
		rb = GetComponent<Rigidbody>();


		dynamicMaxSpeed = maxSpeed; //equal max speeds


		if (anim != null)
		{
			anim.SetBool("flying", true); //Start flying animation
		}


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
			animatedX = Mathf.Lerp(animatedX, x, animationSmoothingSpeed * Time.deltaTime); //Calculate soomthed yaw
			animatedY = Mathf.Lerp(animatedY, y, animationSmoothingSpeed * Time.deltaTime); //Calculate soomthed pitch

			anim.SetFloat("flyingDirectionX", animatedX); //Left right rotation (yaw)
			anim.SetFloat("flyingDirectionY", animatedY); //Up down rotation (pitch)

			if(animationSpeedBasedOnAngle)
			{
				//float converted = newMin + (val - minVal) * (newMax - newMin) / (maxVal - minVal);
				//anim.speed = minAnimationSpeed + (transform.forward.y - -1) * (maxAnimationSpeed - minAnimationSpeed) / (1 - -1); //Calculate animation speed based on birds angle (looking down - slower animation, looking up - faster animation)
				anim.speed = ExtendedMathf.Map(transform.forward.y, -1, 1, minAnimationSpeed, maxAnimationSpeed);
			}
		}


		//Trails
		if(trailsActivated)
		{
			foreach (TrailRenderer trail in trails)
			{
				//float converted = newMin + (val - minVal) * (newMax - newMin) / (maxVal - minVal);
				//trail.time = 0 + (speed - speedToActivateTrails) * (maxTrailTime - 0) / (maxSpeed - speedToActivateTrails); //Calculate trail time (length) 
				trail.time = ExtendedMathf.Map(speed, speedToActivateTrails, maxSpeed, 0, maxTrailTime);
			}
		}
		else
		{
			foreach (TrailRenderer trail in trails)
			{
				trail.time = 0;
			}
		}


		//Debug
		//Debug.Log("input: " + input);
		//Debug.Log("speed: " + speed);
		//Debug.Log("dynamicMaxSpeed: " + dynamicMaxSpeed);
		//Debug.Log("velocity: " + rb.velocity.magnitude + ", " + rb.velocity);
		//Debug.Log("Transform forward: " + transform.forward.magnitude + ", " + transform.forward);
		//Debug.Log("Transorm forward Y: " + transform.forward.y + ", Anim speed: " + anim.speed);
	}


	private void FixedUpdate()
	{
		//rotate player
		transform.Rotate(rotation * rotationSpeed, Space.Self);

		//move player towards rotation with player's speed
		rb.velocity = transform.forward * speed;
	}


	private void OnCollisionEnter(Collision collision)
	{
		if((ignoreCollisionLayers.value & (1 << collision.transform.gameObject.layer)) <= 0) //if not colliding with ignored layers
		{
			Vector3 orthogonalVector = collision.contacts[0].point - transform.position;

			//0-180, 0 -> straight hit, 180 -> light touch
			float angle = Vector3.Angle(orthogonalVector, rb.velocity) - 45;

			float stunSpeed = Mathf.Clamp(ExtendedMathf.Map(angle, 0, 180, minSpeed, speed), minSpeed, dynamicMaxSpeed);
			//float converted = newMin + (val - minVal) * (newMax - newMin) / (maxVal - minVal);
			//float stunSpeed = Mathf.Clamp((minSpeed + (angle - 0) * (speed - minSpeed) / (180 - 0)), minSpeed, dynamicMaxSpeed); //Calculate speed after stun based on collision angle

			//Calculate how many particles to emit
			//int particleCount = Mathf.Clamp((minParticleCountOnStun + ((int)speed - (int)minSpeed) * (maxParticleCountOnStun - minParticleCountOnStun) / ((int)dynamicMaxSpeed - (int)minSpeed)), minParticleCountOnStun, maxParticleCountOnStun);
			int particleCount = Mathf.Clamp(ExtendedMathf.Map((int)speed, (int)minSpeed, (int)dynamicMaxSpeed, minParticleCountOnStun, maxParticleCountOnStun), minParticleCountOnStun, maxParticleCountOnStun);

			//Change speed
			RiseDynamicMaxSpeed(stunSpeed, 0);
			Invoke(nameof(ReduceDynamicMaxSpeed), timeToReduceStun);

			//Emit particles
			StartCoroutine(EmitFeathers(particleCount));

			//Play sound
			if (audioController != null)
			{
				audioController.Play(impactAudioName);
			}

			//Debug
			//Debug.Log(collision.gameObject.name + ", angle: " + angle + ", speed: " + stunSpeed);
			//Debug.Log(particleCount);
		}
	}

	public IEnumerator EmitFeathers(int count)
	{
		//Enable particles, set it in bird position
		feathersParticles.gameObject.SetActive(true);
		feathersParticles.transform.parent = null;
		feathersParticles.transform.position = transform.position;

		//Burst passed amount of particles
		feathersParticles.emission.SetBursts(
			new ParticleSystem.Burst[]{
				new ParticleSystem.Burst(0, count)
			});

		//Emit particles
		feathersParticles.Emit(1);

		//Wait until particles system stops emitting
		yield return new WaitForSeconds(feathersParticles.main.duration);

		//Disable particles and bind it to player
		feathersParticles.transform.parent = transform;
		feathersParticles.gameObject.SetActive(false);
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
		if (PlayerPrefs.HasKey("Options_BirdInvertY"))
		{
			invertBirdY = PlayerPrefs.GetInt(("Options_BirdInvertY")) == 1 ? 1 : -1; //Get Invert input user setting
		}
	}
}