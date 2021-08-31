using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdsController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
	private VisualEffect birds;

	[SerializeField] private float TargetVelocitySpeedMultiplier = 1.0f;

	private void Start()
	{
		birds = GetComponent<VisualEffect>();
	}

	private void Update()
	{
		birds.SetFloat("MaxVelocity", (playerController.speed * TargetVelocitySpeedMultiplier));
	}

	public void SetBirdAmount(int amount, Vector3 position)
	{
		birds.SetInt("Set/RandomSpawnPosition", 0);
		birds.SetVector3("SpawnPosition", position);
		birds.SetInt("BirdAmount", amount);
	}
}
