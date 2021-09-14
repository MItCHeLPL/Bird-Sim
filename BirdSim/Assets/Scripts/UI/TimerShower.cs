using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerShower : MonoBehaviour
{
	private void Awake()
	{
		UpdateTimerVisibility();
	}

	public void UpdateTimerVisibility()
	{
		gameObject.SetActive(PlayerPrefs.GetInt(("Options_ShowTimer")) == 1 ? true : false);
	}
}
