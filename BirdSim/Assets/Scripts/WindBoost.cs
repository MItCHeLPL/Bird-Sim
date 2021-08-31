using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBoost : MonoBehaviour
{
    [SerializeField] private float newDynamicMaxSpeed = 10.0f;
    [SerializeField] private float timeToRiseSpeed = 5.0f;

	[SerializeField] private float newCameraFOV = 70.0f;

	private CameraController camController;

	private void Start()
	{
		camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
	}

	private void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			playerController.RiseDynamicMaxSpeed(newDynamicMaxSpeed, timeToRiseSpeed); //Rise player max speed and speed on enter

			camController.ChangeFOV(newCameraFOV); //Change fov to match speed
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			playerController.ReduceDynamicMaxSpeed(); //Reduce player's max speed on exit

			camController.ChangeFOV(camController.baseFOV); //Change fov to match speed
		}
	}
}
