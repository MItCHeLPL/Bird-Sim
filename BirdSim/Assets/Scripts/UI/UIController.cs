using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
	private void Awake()
	{
		SetUpLevelLockStates(); //Set up player prefs if they don't exitst
	}

	public void EnablePanel(GameObject panel)
	{
		panel.SetActive(true);
	}

	public void DisablePanel(GameObject panel)
	{
		panel.SetActive(false);
	}

	public void SelectButton(GameObject selection)
	{
		//clear selection
		EventSystem.current.SetSelectedGameObject(null);

		//select new object
		EventSystem.current.SetSelectedGameObject(selection);
	}

	public void LoadLevel(int levelId)
	{
		//if there is such scene
		if(SceneManager.sceneCountInBuildSettings > levelId)
		{
			//SceneManager.LoadScene(levelId);
			StartCoroutine(LoadYourAsyncScene(levelId));
			Time.timeScale = 1; //UnFreeze gameplay in new scene
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
		//Load scene in the background
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			//Debug.Log("Scene loadning progress: " + asyncLoad.progress);

			yield return null;
		}

		//Finished scene loading
		//Debug.Log("Finished scene loading");
	}
}
