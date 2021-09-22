using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReciever : MonoBehaviour
{
	[SerializeField] private AudioController audioController; 

    private void PlaySound(string name)
	{
		if (audioController != null)
		{
			audioController.Play(name);
		}
	}
}
