using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetSliderLabelValue : MonoBehaviour
{
	private Slider slider;
	[SerializeField] private TextMeshProUGUI text;

	private void Awake()
	{
		slider = GetComponent<Slider>();
	}

	public void ChangeSliderLabelValue()
	{
		text.SetText(Mathf.Round(slider.value * 100) + "%"); //Set text value to match slider value
	}
}
