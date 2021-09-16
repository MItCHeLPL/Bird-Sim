using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectShower : MonoBehaviour
{
	public string keyName;

	private void Awake()
	{
		UpdateVisibility(keyName);
	}

	public void UpdateVisibility(string keyName)
	{
		gameObject.SetActive(PlayerPrefs.GetInt((keyName)) == 1 ? true : false);
	}
}
