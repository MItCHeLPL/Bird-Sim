using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateResolutions : MonoBehaviour
{
    private TMP_Dropdown dropdown;

	private void Awake()
	{
        dropdown = GetComponent<TMP_Dropdown>();

        UpdateResolutionList();
    }

	public void UpdateResolutionList()
	{
        Resolution[] resolutions = Screen.resolutions;

        dropdown.options.Clear(); //Reset list

        foreach (var res in resolutions)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height + "@" + res.refreshRate));
        }
    }
}
