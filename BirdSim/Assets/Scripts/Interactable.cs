using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
	[SerializeField] private UnityEvent InteractionStart;
	[SerializeField] private UnityEvent InteractionCancel;
	[SerializeField] private UnityEvent InteractionSuccess;

	private bool InTrigger = false;

	private void Update()
	{
		if(InTrigger)
		{
			InteractionStart.Invoke();
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			InTrigger = true;
			CallOnInteraction();
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player")
		{
			InTrigger = false;
			CancelInteraction();
		}
	}

	private void CallOnInteraction()
	{
		InteractionSuccess.Invoke(); //invoke event function
	}

	private void CancelInteraction() //if user stopped holding before set time
	{
		InteractionCancel.Invoke();
	}

	public void TestDebugStart()
	{
		Debug.Log("Started");
	}

	public void TestDebugCancel()
	{
		Debug.Log("Canceled");
	}

	public void TestDebugSuccess()
	{
		Debug.Log("Success");
	}
}
