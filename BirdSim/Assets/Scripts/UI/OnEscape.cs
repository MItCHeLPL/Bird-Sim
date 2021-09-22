using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEscape : MonoBehaviour
{
    public UnityEvent OnEscapeEvent;

	private void Update()
	{
		//Wait for escape
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.JoystickButton1))
		{
			OnEscapeEvent.Invoke();
		}
	}
}
