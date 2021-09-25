using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
	public void EnablePanel(GameObject panel)
	{
		panel.SetActive(true);
	}

	public void DisablePanel(GameObject panel)
	{
		panel.SetActive(false);
	}

	public void SelectButton(GameObject selection)
	{
		//clear selection
		EventSystem.current.SetSelectedGameObject(null);

		//select new object
		EventSystem.current.SetSelectedGameObject(selection);
	}
}
