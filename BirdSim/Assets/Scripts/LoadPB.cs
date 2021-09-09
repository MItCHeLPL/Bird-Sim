using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPB : MonoBehaviour
{
	private TextMeshProUGUI text;
	[SerializeField] private int LevelID;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();

		LoadPersonalBest(LevelID);
	}

	public void LoadPersonalBest(int LevelID, bool locked = false)
	{
		//If level is locked
		if(locked || (PlayerPrefs.HasKey(("LevelLockedStatus_" + LevelID)) && PlayerPrefs.GetInt(("LevelLockedStatus_" + LevelID)) == 1))
		{
			text.SetText("LOCKED");
		}
		//If level has PB
		else if (PlayerPrefs.HasKey(("TimePB_" + LevelID)) && PlayerPrefs.GetFloat(("TimePB_" + LevelID)) > 0.0f)
		{
			text.SetText(TimeConverter.ConvertTime(PlayerPrefs.GetFloat("TimePB_" + LevelID)));
		}
		//If player didn't ever beat the level
		else
		{
			text.SetText("New");
		}
	}
}
