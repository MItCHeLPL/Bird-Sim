using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpoints; //List of all checkpoints of current level

    public float timer = 0.0f; //Gameplay timer
    private Coroutine timerCounter; //Coroutine

	//Level states
	[HideInInspector] public bool isPaused = false;
	[HideInInspector] public bool finished = false;

	//Unity Events to use in other scripts
	[SerializeField] private UnityEvent OnPause;
	[SerializeField] private UnityEvent OnResume;
	[SerializeField] private UnityEvent OnFinished;
	[SerializeField] private UnityEvent OnPersonalBest;

	void Awake()
    {
		timerCounter = StartCoroutine(TimerCoroutine()); //Start counting time

		//Hide cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		//Set states
		isPaused = false;
		finished = false;
	}

	private void Update()
	{
		//Wait for escape to pause
		if (Input.GetKeyDown(KeyCode.Escape) && finished == false)
		{
			if(isPaused)
			{
				Resume();		
			}
			else
			{
				Pause();
			}	
		}
	}

	public void UseCheckpoint(Checkpoint cp)
	{
        checkpoints.Remove(cp); //Remove checkpoint from list

		//Debug.Log("Achieved checkpoint");

		//Finish level if used all checkpoint
        if(checkpoints.Count == 0)
		{
            FinishLevel();
        }
	}

	public void Pause()
	{
		Time.timeScale = 0; //Freeze gameplay

		StopCoroutine(timerCounter); //Stop counting the time

		//Show cursor
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		OnPause.Invoke(); //UnityEvent

		isPaused = true; //State
	}

	public void Resume()
	{
		Time.timeScale = 1; //UnFreeze gameplay

		timerCounter = StartCoroutine(TimerCoroutine()); //Resume counting the time

		//Hide cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		OnResume.Invoke(); //UnityEvent

		isPaused = false; //State
	}

	private void FinishLevel()
	{
		finished = true; //State

		Time.timeScale = 0; //Freeze gameplay

		StopCoroutine(timerCounter); //Stop counting the time

		//Show cursor
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		OnFinished.Invoke(); //UnityEvent

		SavePB(); //Save player's PB if achieved

		if(SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings) //If there is next level
		{
			PlayerPrefs.SetInt(("LevelLockedStatus_" + (SceneManager.GetActiveScene().buildIndex + 1)), 0); //Unlock new level

			PlayerPrefs.Save();
		}

		//Debug.Log("Finished level, time: " + timer);
	}

	private IEnumerator TimerCoroutine()
	{
		while (true)
		{
			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
	}

	private void SavePB()
	{
		int sceneID = SceneManager.GetActiveScene().buildIndex; //Current scene id

		if (PlayerPrefs.HasKey("TimePB_" + sceneID)) //If already achieved PB
		{
			if(timer < PlayerPrefs.GetFloat(("TimePB_" + sceneID))) //New personal best
			{
				PlayerPrefs.SetFloat(("TimePB_" + sceneID), timer); //Save time

				OnPersonalBest.Invoke(); //UnityEvent
			}
		}
		else //Player finished level for the first time 
		{
			PlayerPrefs.SetFloat(("TimePB_" + sceneID), timer); //Save time

			OnPersonalBest.Invoke(); //UnityEvent
		}

		PlayerPrefs.Save();
	}
}
