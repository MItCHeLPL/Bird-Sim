using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TimeConverter
{
	public static string ConvertTime(float time)
	{
		string output = TimeSpan.FromSeconds(time).ToString(); //convert float to string (xx:xx:xxxxxxx)
		output = output.Substring(0, output.Length - 4); //remove four zeros from the end of string, output: xx:xx:xxx (mm:ss:ms)
		return output;
	}
}

public static class ExtendedMathf
{
	public static float Map(float value, float oldMinValue, float oldMaxValue, float newMinValue, float newMaxValue)
	{
		return newMinValue + (value - oldMinValue) * (newMaxValue - newMinValue) / (oldMaxValue - oldMinValue); //Map value from old range onto a new range
	}
	public static int Map(int value, int oldMinValue, int oldMaxValue, int newMinValue, int newMaxValue)
	{
		return newMinValue + (value - oldMinValue) * (newMaxValue - newMinValue) / (oldMaxValue - oldMinValue); //Map value from old range onto a new range
	}

	public static float MapFrom01(float value, float newMinValue, float newMaxValue)
	{
		return newMinValue + (value - 0) * (newMaxValue - newMinValue) / (1 - 0); //Map value from old range onto a new range
	}
	public static float MapTo01(float value, float oldMinValue, float oldMaxValue)
	{
		return 0 + (value - oldMinValue) * (1 - 0) / (oldMaxValue - oldMinValue); //Map value from old range onto a new range
	}
}

/*public static class ExtendedCoroutines
{
	public class Lerp : MonoBehaviour
	{
		private bool isRunning { get; set; }
		public bool IsRunning { get { return isRunning; } }

		public float Value { get; set; }

		public IEnumerator Coroutine { get; set; }
		private Coroutine runningCoroutine = null;


		public Lerp()
		{
			isRunning = false;
			Value = 0;
			Coroutine = LerpScaled(0,1,1);
		}
		public Lerp(float from, float to, float time)
		{
			isRunning = false;
			Value = 0;
			Coroutine = LerpScaled(from, to, time);
		}
		public Lerp(IEnumerator coroutine)
		{
			isRunning = false;
			Value = 0;
			Coroutine = coroutine;
		}
		public Lerp(ref float value)
		{
			isRunning = false;
			Value = value;
		}
		public Lerp(ref float value, float from, float to, float time)
		{
			isRunning = false;
			Value = value;
			Coroutine = LerpScaled(from, to, time);
		}
		public Lerp(ref float value, IEnumerator coroutine)
		{
			isRunning = false;
			Value = value;
			Coroutine = coroutine;
		}


		public void Start()
		{
			if(!isRunning)
			{
				runningCoroutine = StartCoroutine(Coroutine);
			}
			else
			{
				Debug.Log("Already running a coroutine");
			}
		}

		public void Stop()
		{
			if(isRunning && runningCoroutine != null)
			{
				StopCoroutine(runningCoroutine);
			}
		}


		public IEnumerator LerpScaled(float from, float to, float time)
		{
			isRunning = true;

			float t = 0f;
			Value = from;

			while (t < 1)
			{
				t += Time.deltaTime / time;

				Value = Mathf.Lerp(from, to, t);

				yield return null;
			}

			isRunning = false;
		}

		public IEnumerator LerpUnscaled(float from, float to, float time)
		{
			isRunning = true;

			float t = 0f;
			Value = from;

			while (t < 1)
			{
				t += Time.unscaledDeltaTime / time;

				Value = Mathf.Lerp(from, to, t);

				yield return null;
			}

			isRunning = false;
		}


		public IEnumerator SlerpScaled(float from, float to, float time)
		{
			isRunning = true;

			float t = 0f;
			Value = from;

			while (t < 1)
			{
				t += Time.deltaTime / time;

				Value = Mathf.SmoothStep(from, to, t);

				yield return null;
			}

			isRunning = false;
		}

		public IEnumerator SlerpUnscaled(float from, float to, float time)
		{
			isRunning = true;

			float t = 0f;
			Value = from;

			while (t < 1)
			{
				t += Time.unscaledDeltaTime / time;

				Value = Mathf.SmoothStep(from, to, t);

				yield return null;
			}

			isRunning = false;
		}
	}
}*/