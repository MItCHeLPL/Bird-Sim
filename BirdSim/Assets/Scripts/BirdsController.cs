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

	public void SetBirdAmount(int amount)
	{
		birds.SetInt("BirdAmount", amount);
	}
}
