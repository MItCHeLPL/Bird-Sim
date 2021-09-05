using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdsController : MonoBehaviour
{
	private int TotalBirdsSpawned = 0;

    [SerializeField] private PlayerController playerController;
	private VisualEffect birds;

	[SerializeField] private float TargetVelocitySpeedMultiplier = 1.0f;

	private void Start()
	{
		birds = GetComponent<VisualEffect>();

		Vector3 position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, playerController.transform.position.z - 10.0f);

		TotalBirdsSpawned += birds.GetInt("BirdAmount"); //Save initial birds amount
	}

	private void Update()
	{
		birds.SetFloat("MaxVelocity", (playerController.speed * TargetVelocitySpeedMultiplier)); //Limit birds max speed
	}

	private void TriggerSpawnEvent()
	{
		birds.SendEvent(birds.initialEventID); //Spawn new birds

		TotalBirdsSpawned += birds.GetInt("BirdAmount"); //Save total birds amount
	}

	public void SetBirdAmount(int amount)
	{
		birds.SetInt("BirdAmount", amount);

		TriggerSpawnEvent(); //Spawn new birds
	}
}
