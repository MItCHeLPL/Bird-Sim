using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeConverter
{
	public static string ConvertTime(float time)
	{
		string output = TimeSpan.FromSeconds(time).ToString(); //convert float to string (xx:xx:xxxxxxx)
		output = output.Substring(0, output.Length - 4); //remove four zeros from the end of string, output: xx:xx:xxx (mm:ss:ms)
		return output;
	}
}
