using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
	public string name;

	public AudioSource source;

	public AudioClip clip;

	public bool playOnAwake = false;

	public bool loop = false;

	public bool stopOnGamePause = false;
	public bool playOnGameResume = true;

	public bool useMusicVolume = false;

	[Range(0f, 2f)]
	public float volume = 1;
	[HideInInspector] public float defaultVolume = 1;

	[Range(0.1f, 3f)]
	public float pitch = 1;

	[Range(0f, 1f)]
	public float spatialBlend = 0;
}

public class AudioController : MonoBehaviour
{
	private float globalVolume = 0.5f;
	private float musicVolume = 0.5f;

	[SerializeField] private List<Sound> sounds;


	private void Start()
	{
		//Set up source settings
		foreach (Sound s in sounds)
		{
			s.source.clip = s.clip;

			s.source.playOnAwake = s.playOnAwake;

			s.source.loop = s.loop;

			s.source.volume = s.volume;
			s.defaultVolume = s.volume;

			s.source.pitch = s.pitch;

			s.source.spatialBlend = s.spatialBlend;

			if (s.playOnAwake)
			{
				Play(s);
			}
		}

		GetSettingsFromPlayerPrefs();
	}


	public void Play(string name, float delay=0.0f)
	{
		Sound s = sounds.Find(sound => sound.name == name); //Find source

		Play(s, delay);
	}
	public void Play(Sound s, float delay = 0.0f)
	{
		if (s != null)
		{
			s.source.PlayDelayed(delay);
		}
	}


	public void Stop(string name)
	{
		Sound s = sounds.Find(sound => sound.name == name); //Find source

		Stop(s);
	}
	public void Stop(Sound s)
	{
		if (s != null)
		{
			s.source.Stop();
		}
	}


	public bool isPlaying(string name)
	{
		Sound s = sounds.Find(sound => sound.name == name); //Find source

		return isPlaying(s);
	}
	public bool isPlaying(Sound s)
	{
		if (s != null)
		{
			return s.source.isPlaying;
		}

		return false;
	}


	public void GamePause()
	{
		foreach (Sound s in sounds)
		{
			if (s.stopOnGamePause)
			{
				Stop(s);
			}
		}
	}

	public void GameResume()
	{
		foreach (Sound s in sounds)
		{
			if ((s.playOnAwake && !isPlaying(s)) || (s.playOnGameResume && !isPlaying(s)))
			{
				Play(s);
			}
		}
	}


	public void GetSettingsFromPlayerPrefs()
	{
		if (PlayerPrefs.HasKey("Options_Volume"))
		{
			globalVolume = PlayerPrefs.GetFloat(("Options_Volume")); //Get volume value from user settings
		}

		if (PlayerPrefs.HasKey("Options_MusicVolume"))
		{
			musicVolume = PlayerPrefs.GetFloat(("Options_MusicVolume")); //Get music volume value from user settings
		}

		//Assign volume to all sources
		foreach (Sound s in sounds)
		{
			if(s.useMusicVolume)
			{
				s.source.volume = musicVolume * s.defaultVolume;
			}
			else
			{
				s.source.volume = globalVolume * s.defaultVolume;
			}
			
		}
	}
}
