using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpoints;

    private float timer = 0.0f;
    private Coroutine timerCounter;

    void Start()
    {
		timerCounter = StartCoroutine(TimerCoroutine());
	}

	public void UseCheckpoint(Checkpoint cp)
	{
        checkpoints.Remove(cp);

		Debug.Log("Achieved checkpoint");

        if(checkpoints.Count == 0)
		{
            FinishLevel();
        }
	}

    private void FinishLevel()
	{
		Time.timeScale = 0; //Freeze gameplay

		StopCoroutine(timerCounter); //Save time

		Debug.Log("Finished level, time: " + timer);
	}

	private IEnumerator TimerCoroutine()
	{
		while (true)
		{
			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
	}
}
