using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	private float volume;

	[SerializeField] private List<AudioSource> audioSources;

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	public void GetSettingsFromPlayerPrefs()
	{
		if (PlayerPrefs.HasKey("Options_Volume"))
		{
			volume = PlayerPrefs.GetInt(("Options_Volume")); //Get volume value from user settings
		}
	}
}
