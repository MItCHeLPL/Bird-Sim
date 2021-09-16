using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public struct UIElement<T>
{
	public T element;
	public string keyName;
}

public class SettingsController : MonoBehaviour
{
	//UI Objects
	public UIElement<Toggle>[] toggles;
	public UIElement<Slider>[] sliders;
	public UIElement<TMP_Dropdown>[] dropdowns;

	//Defaults
	private Dictionary<string, int> intKeys = new Dictionary<string, int>()
	{
		{"Options_CamInvertY", 0},
		{"Options_BirdInvertY", 1},
		{"Options_ShowTimer", 1},
		{"Graphics_VSync", 0},
		{"Graphics_Windowed", 0},
		{"Graphics_AdditionalParticles", 1}
	};
	private Dictionary<string, float> floatKeys = new Dictionary<string, float>()
	{
		{"Options_Volume", 0.5f}
	};
	private Dictionary<string, string> stringKeys = new Dictionary<string, string>()
	{
		{"Graphics_Quality", "Medium"},
		{"Graphics_Resolution", "1920x1080@60"}
	};

	private void Awake()
	{
		InitPlayerPrefs(); //Set up player prefs if they don't exist when panel becomes visible
	}

	//Call onClick on Toggle
	public void UpdateToggle(int togglesIndex)
	{
		//Save changed option to player settings
		if (toggles[togglesIndex].element.isOn)
		{
			PlayerPrefs.SetInt((toggles[togglesIndex].keyName), 1);
		}
		else
		{
			PlayerPrefs.SetInt((toggles[togglesIndex].keyName), 0);
		}

		PlayerPrefs.Save();
	}
	public void LoadToggle(Toggle toggle, string playerPrefsKey)
	{
		if (PlayerPrefs.HasKey(playerPrefsKey))
		{
			toggle.isOn = PlayerPrefs.GetInt((playerPrefsKey)) == 1 ? true : false; //Load toggle option from player settings
		}
	}

	//Call onChange on Slider
	public void UpdateSlider(int slidersIndex)
	{
		PlayerPrefs.SetFloat((sliders[slidersIndex].keyName), sliders[slidersIndex].element.value); //Save changed option to player settings

		PlayerPrefs.Save();
	}
	public void LoadSlider(Slider slider, string playerPrefsKey)
	{
		if (PlayerPrefs.HasKey(playerPrefsKey))
		{
			slider.value = PlayerPrefs.GetFloat((playerPrefsKey)); //Load slider value from player settings
		}
	}

	//Call onChange on Dropdown
	public void UpdateDropdown(int dropownsIndex)
	{
		PlayerPrefs.SetString((dropdowns[dropownsIndex].keyName), dropdowns[dropownsIndex].element.options[dropdowns[dropownsIndex].element.value].text); //Save changed option to player settings

		PlayerPrefs.Save();
	}
	public void LoadDropdown(TMP_Dropdown dropdown, string playerPrefsKey)
	{
		if (PlayerPrefs.HasKey(playerPrefsKey))
		{
			string key = PlayerPrefs.GetString((playerPrefsKey)); //Load dropown string from player settings

			dropdown.value = dropdown.options.FindIndex(option => option.text == key); //Assign dropown value based on string
		}
	}

	//Init playerprefs to load from
	public void InitPlayerPrefs(bool forceDefaults = false)
	{
		//Set up defaults
		foreach (KeyValuePair<string, int> key in intKeys)
		{
			if (!PlayerPrefs.HasKey((key.Key)) || forceDefaults) //if key doesn't exits or defaults are forced
			{
				PlayerPrefs.SetInt((key.Key), key.Value); //default value to given key
			}
		}

		foreach (KeyValuePair<string, float> key in floatKeys)
		{
			if (!PlayerPrefs.HasKey((key.Key)) || forceDefaults) //if key doesn't exits or defaults are forced
			{
				PlayerPrefs.SetFloat((key.Key), key.Value); //default value to given key
			}
		}

		foreach (KeyValuePair<string, string> key in stringKeys)
		{
			if (!PlayerPrefs.HasKey((key.Key)) || forceDefaults) //if key doesn't exits or defaults are forced
			{
				PlayerPrefs.SetString((key.Key), key.Value); //default value to given key
			}
		}

		PlayerPrefs.Save(); //Save player settings
	}

	//Load values to UI elements
	public void LoadOptionValuesFromPlayerPrefs()
	{
		foreach(UIElement<Toggle> toggle in toggles)
		{
			LoadToggle(toggle.element, toggle.keyName);
		}

		foreach(UIElement<Slider> slider in sliders)
		{
			LoadSlider(slider.element, slider.keyName);
		}

		foreach(UIElement<TMP_Dropdown> dropdown in dropdowns)
		{
			LoadDropdown(dropdown.element, dropdown.keyName);
		}
	}
}
