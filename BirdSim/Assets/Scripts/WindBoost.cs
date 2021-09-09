using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBoost : MonoBehaviour
{
    [SerializeField] private float newDynamicMaxSpeed = 10.0f; //Arbitrary max speed value
    [SerializeField] private float timeToRiseSpeed = 5.0f; //How long does it take to rise speed to match arbitrary max speed

	[SerializeField] private float newCameraFOV = 70.0f; //Camera fov during boost

	private CameraController camController;

	private void Awake()
	{
		camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>(); //Get camera
	}

	private void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			playerController.RiseDynamicMaxSpeed(newDynamicMaxSpeed, timeToRiseSpeed); //Rise player arbitrary max speed and speed on enter

			camController.ChangeFOV(newCameraFOV); //Change fov to match boost camera fov
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			playerController.ReduceDynamicMaxSpeed(); //Reduce player's arbitrary max speed back to natural max speed on exit

			camController.ChangeFOV(camController.baseFOV); //Change fov to match speed
		}
	}
}
