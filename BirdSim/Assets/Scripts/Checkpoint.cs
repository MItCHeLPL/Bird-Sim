using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] private int addBirdAmount = 10; //How many new bird particles to spawn
	[SerializeField] private bool destroyOnExit = false; //Destroy game object on exit
	[SerializeField] private bool disableOnExit = true; //Disable game object on exit
	[SerializeField] private float cooldown = 1.0f; //How long to wait between launching checkpoint and seeing outcome

	[SerializeField] private LevelManager lvlManager;

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			PlayerController playerController = col.GetComponent<PlayerController>();

			BirdsController birdsController = playerController.birdsController;

			birdsController.SetBirdAmount(addBirdAmount); //Add new birds
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
			else if(disableOnExit)
			{
				StartCoroutine(DisableCheckpoint()); //Disable checkpoint
			}

			lvlManager.UseCheckpoint(this); //Tell level manager that player used this checkpoint
		}
	}

	private IEnumerator DestroyCheckpoint()
	{
		yield return new WaitForSeconds(cooldown);
		Destroy(gameObject);
	}

	private IEnumerator DisableCheckpoint()
	{
		yield return new WaitForSeconds(cooldown);
		gameObject.SetActive(false);
	}
}
