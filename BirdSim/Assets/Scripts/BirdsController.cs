using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdsController : MonoBehaviour
{
	[HideInInspector] public int TotalBirdsSpawned = 0; //Current amount of particle birds spawned

    [SerializeField] private PlayerController playerController;
	private VisualEffect birds;

	[SerializeField] private float TargetVelocitySpeedModifier = +1.0f; //Max speed difference between player and particle birds

	private AudioController audioController;
	[SerializeField] private string[] birdAudioNames;

	private void Awake()
	{
		birds = GetComponent<VisualEffect>();

		TotalBirdsSpawned += birds.GetInt("BirdAmount"); //Save initial birds amount

		audioController = playerController.audioController;

		Invoke("CheckBirdsAudio", 1);
	}

	private void Update()
	{
		birds.SetFloat("MaxVelocity", (playerController.speed + TargetVelocitySpeedModifier)); //Limit birds max speed
	}

	private void TriggerSpawnEvent()
	{
		birds.SendEvent(birds.initialEventID); //Spawn new birds

		TotalBirdsSpawned += birds.GetInt("BirdAmount"); //Save total birds amount

		CheckBirdsAudio();
	}

	public void SetBirdAmount(int amount)
	{
		birds.SetInt("BirdAmount", amount);

		TriggerSpawnEvent(); //Spawn new birds
	}

	public void CheckBirdsAudio()
	{
		if(audioController != null)
		{
			foreach (string name in birdAudioNames)
			{
				if (!audioController.isPlaying(name) && TotalBirdsSpawned > 0)
				{
					audioController.Play(name, Random.Range(0.5f, 2.0f));
				}
			}
		}
		
	}
}
