using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GraphicsController : MonoBehaviour
{
	private Dictionary<string, int> qualityLevels = new Dictionary<string, int>()
	{
		{"Low", 2},
		{"Medium", 1},
		{"High", 0}
	};

	private void Start()
	{
		//StartCoroutine(UpdateAll());
	}

	public void UpdateVSync()
	{
		if(PlayerPrefs.HasKey("Graphics_VSync"))
		{
			QualitySettings.vSyncCount = PlayerPrefs.GetInt("Graphics_VSync");
		}
	}

	public void UpdateWinowed()
	{
		if (PlayerPrefs.HasKey("Graphics_Windowed"))
		{
			if(PlayerPrefs.GetInt("Graphics_Windowed") == 0)
			{
				Screen.fullScreen = true;
				Screen.fullScreenMode = FullScreenMode.Windowed;
			}
			else
			{
				Screen.fullScreen = false;
				Screen.fullScreenMode = FullScreenMode.Windowed;
			}
		}
	}

	public void UpdateQuality()
	{
		if (PlayerPrefs.HasKey("Graphics_Quality"))
		{
			int currentQualityLevel = QualitySettings.GetQualityLevel();
			int wantedQualityLevel = qualityLevels.GetValueOrDefault(PlayerPrefs.GetString("Graphics_Quality"));

			if(currentQualityLevel != wantedQualityLevel)
			{
				StartCoroutine(UpdateQualityCoroutine(wantedQualityLevel));
			}
		}
	}
	private IEnumerator UpdateQualityCoroutine(int wantedQualityLevel)
	{
		QualitySettings.SetQualityLevel(wantedQualityLevel, true);

		yield return null;
	}

	public void UpdateResolution()
	{
		if (PlayerPrefs.HasKey("Graphics_Resolution"))
		{
			string res = PlayerPrefs.GetString("Graphics_Resolution"); //get resolution string ("1920x1080@60" or "640x480@60")

			int xPos = res.IndexOf('x'); //get position of 'x'
			int atPos = res.IndexOf('@'); //get position of 'at'

			int x = int.Parse(res.Substring(0, xPos)); //get first value forom string
			int y = int.Parse(res.Substring(xPos + 1, res.Length - atPos).Replace("@", "")); //get second value from string
			int refreshRate = int.Parse(res.Substring(atPos + 1)); //get refresh rate value from string

			Screen.SetResolution(x, y, Screen.fullScreen, refreshRate); //Set resolution
			Application.targetFrameRate = refreshRate;
		}
	}

	public void UpdateAllParticles(string keyName)
	{
		ObjectShower[] objectShowers = FindObjectsOfType<ObjectShower>();

		foreach(ObjectShower objectShower in objectShowers)
		{
			if(objectShower.keyName == keyName)
			{
				objectShower.UpdateVisibility(keyName);
			}
		}
	}

	private IEnumerator UpdateAll()
	{
		UpdateQuality();
		UpdateVSync();
		UpdateWinowed();
		UpdateResolution();

		yield return null;
	}
}
