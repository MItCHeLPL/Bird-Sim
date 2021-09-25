using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
	[SerializeField] private FadeController fade = null;
	[SerializeField] private bool waitForEndOfFade = true;

	[SerializeField] private float timeToHoldOffLoading = 2.0f;

	[SerializeField] private UnityEvent OnLoadStart;
	[SerializeField] private UnityEvent OnLoaded;

	void Start()
    {
		SetUpLevelLockStates(); //Set up player prefs if they don't exitst
	}

	public void LoadLevel(int levelId)
	{
		//if there is such scene
		if (SceneManager.sceneCountInBuildSettings > levelId)
		{
			//SceneManager.LoadScene(levelId);
			StartCoroutine(LoadYourAsyncScene(levelId));
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	private void SetUpLevelLockStates()
	{
		//Level lock states
		for (int lvlID = 2; lvlID < SceneManager.sceneCountInBuildSettings; lvlID++)
		{
			if (!PlayerPrefs.HasKey(("LevelLockedStatus_" + lvlID))) //If setting doesn't exits
			{
				PlayerPrefs.SetInt(("LevelLockedStatus_" + lvlID), 1); //Assign lock key to every level
			}
		}
		PlayerPrefs.SetInt(("LevelLockedStatus_1"), 0); //Unlock first level

		PlayerPrefs.Save(); //Save player settings
	}

	private IEnumerator LoadYourAsyncScene(int sceneIndex)
	{
		//Wait until finsihed fading
		if (fade != null && waitForEndOfFade)
		{
			while (fade.isFading)
			{
				yield return null;
			}
		}

		OnLoadStart.Invoke();

		//Begin to load the Scene you specify
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

		//Don't let the Scene activate until you allow it to
		asyncLoad.allowSceneActivation = false;

		//When the load is still in progress
		while (!asyncLoad.isDone)
		{
			//Debug.Log("Scene loadning progress: " + asyncLoad.progress);

			// Check if the load has finished
			if (asyncLoad.progress >= 0.9f)
			{
				yield return new WaitForSecondsRealtime(timeToHoldOffLoading);

				Time.timeScale = 1; //UnFreeze gameplay in new scene

				OnLoaded.Invoke();

				//Debug.Log("Finished scene loading");

				//Activate the Scene
				asyncLoad.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
