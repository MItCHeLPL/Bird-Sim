using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
	[SerializeField] private Toggle ToggleInvertCamY = null;
	[SerializeField] private Toggle ToggleInvertBirdY = null;
	[SerializeField] private Slider VolumeSlider = null;

	private void Start()
	{
		SetUpPlayerPrefs(); //Set up player prefs if they don't exitst
	}

	public void EnablePanel(GameObject panel)
	{
		panel.SetActive(true);
	}

	public void DisablePanel(GameObject panel)
	{
		panel.SetActive(false);
	}

	public void LoadLevel(int levelId)
	{
		//SceneManager.LoadScene(levelId);
		StartCoroutine(LoadYourAsyncScene(levelId));
		Time.timeScale = 1; //UnFreeze gameplay in new scene
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void Options_ToggleInvertCamY(Toggle toggle)
	{
		//Save changed option to player settings
		if (toggle.isOn)
		{
			PlayerPrefs.SetInt(("Options_CamInvertY"), 1);
		}
		else
		{
			PlayerPrefs.SetInt(("Options_CamInvertY"), 0);
		}

		PlayerPrefs.Save();
	}

	public void Options_ToggleInvertBirdY(Toggle toggle)
	{
		//Save changed option to player settings
		if (toggle.isOn)
		{
			PlayerPrefs.SetInt(("Options_BirdInvertY"), 1);
		}
		else
		{
			PlayerPrefs.SetInt(("Options_BirdInvertY"), 0);
		}

		PlayerPrefs.Save();
	}

	public void Options_ChangeVolume(Slider slider)
	{
		PlayerPrefs.SetFloat(("Options_Volume"), slider.value); //Save changed option to player settings

		PlayerPrefs.Save();
	}

	private void SetUpPlayerPrefs()
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

		//Set up default options
		if (!PlayerPrefs.HasKey(("Options_CamInvertY")))
		{
			PlayerPrefs.SetInt(("Options_CamInvertY"), 1); //default invert Y cam axis
		}
		if (!PlayerPrefs.HasKey(("Options_BirdInvertY")))
		{
			PlayerPrefs.SetInt(("Options_BirdInvertY"), 1); //default invert Y bird axis
		}
		if (!PlayerPrefs.HasKey(("Options_Volume")))
		{
			PlayerPrefs.SetFloat(("Options_Volume"), .5f); //default volume
		}

		PlayerPrefs.Save(); //Save player settings
	}

	public void LoadOptionValuesFromPlayerPrefs()
	{
		if(ToggleInvertCamY != null)
		{
			ToggleInvertCamY.isOn = PlayerPrefs.GetInt(("Options_CamInvertY")) == 1 ? true : false; //Load Camera Invert Y option from player settings
		}
		if (ToggleInvertBirdY != null)
		{
			ToggleInvertBirdY.isOn = PlayerPrefs.GetInt(("Options_BirdInvertY")) == 1 ? true : false; //Load Bird Invert Y option from player settings
		}
		if (VolumeSlider != null)
		{
			VolumeSlider.value = PlayerPrefs.GetFloat(("Options_Volume")); //Load volume option from player settings
		}
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
