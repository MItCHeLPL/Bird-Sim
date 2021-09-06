using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private int addBirdAmount = 10;
	[SerializeField] private bool destroyOnExit = false;
	[SerializeField] private float destroyCooldown = 0.0f;

	[SerializeField] private LevelManager lvlManager;

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			BirdsController birdsController = playerController.birdsController;

			birdsController.SetBirdAmount(addBirdAmount);
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

			lvlManager.UseCheckpoint(this);
		}
	}

	private IEnumerator DestroyCheckpoint()
	{
		yield return new WaitForSeconds(destroyCooldown);
		Destroy(gameObject);
	}
}
