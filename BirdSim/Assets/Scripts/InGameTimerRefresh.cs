using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameTimerRefresh : MonoBehaviour
{
    [SerializeField] private LevelManager lvlManager;

	private TextMeshProUGUI text;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.SetText(TimeConverter.ConvertTime(lvlManager.timer)); //Update ui timer
	}
}
