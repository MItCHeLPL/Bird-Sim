using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdsController : MonoBehaviour
{
	private int TotalBirdsSpawned = 0;

    [SerializeField] private PlayerController playerController;
	private VisualEffect birds;

	[SerializeField] private float TargetVelocitySpeedSubtraction = 1.0f;

	private void Start()
	{
		birds = GetComponent<VisualEffect>();

		TotalBirdsSpawned += birds.GetInt("BirdAmount"); //Save initial birds amount
	}

	private void Update()
	{
		birds.SetFloat("MaxVelocity", (playerController.speed - TargetVelocitySpeedSubtraction)); //Limit birds max speed
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
