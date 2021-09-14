using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshUILevelLockState : MonoBehaviour
{
	[SerializeField] private int levelIndex;
	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		RefreshLockState(); //Check if level is locked
	}

	public void RefreshLockState()
	{
		//If level is unclocked, activate button
		if (PlayerPrefs.HasKey(("LevelLockedStatus_" + levelIndex)))
		{
			int state = PlayerPrefs.GetInt(("LevelLockedStatus_" + levelIndex));

			if (state == 1)
			{
				button.interactable = false;
			}
			else
			{
				button.interactable = true;
			}
		}
	}
}
