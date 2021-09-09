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
		volume = PlayerPrefs.GetInt(("Options_Volume")); //Get volume value from user settings
	}
}
