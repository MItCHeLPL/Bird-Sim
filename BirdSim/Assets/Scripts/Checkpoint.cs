using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private int addBirdAmount = 10;
	[SerializeField] private Vector3 spawnOffset = new Vector3(0,0,-10);
	[SerializeField] private bool destroyOnExit = false;
	[SerializeField] private float destroyCooldown = 0.0f;

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			BirdsController birdsController = playerController.birdsController;

			Vector3 spawnPosition = playerController.transform.position + spawnOffset;

			birdsController.SetBirdAmount(addBirdAmount, spawnPosition);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player")
		{
			if(destroyOnExit)
			{
				StartCoroutine(DestroyCheckpoint()); //Destroy checkpoint
			}
		}
	}

	private IEnumerator DestroyCheckpoint()
	{
		yield return new WaitForSeconds(destroyCooldown);
		Destroy(gameObject);
	}
}
