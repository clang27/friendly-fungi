/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public static class Utility {
	public const int HeadshotDimension = 108;
	public static string FormatTime(float time) {
		var hours = Mathf.FloorToInt(time);
		var minutes = 60f * (time - hours);
		
		var amPmDesignator = "am";
		switch (hours) {
			case 0:
				hours = 12;
				break;
			case 12:
				amPmDesignator = "pm";
				break;
			case > 12:
				hours -= 12;
				amPmDesignator = "pm";
				break;
		}
		return $"{hours}:{minutes:00}{amPmDesignator}";
	}
	
	public static string FormatTime(float hours, float minutes) {
		var amPmDesignator = "am";
		switch (hours) {
			case 0:
				hours = 12;
				break;
			case 12:
				amPmDesignator = "pm";
				break;
			case > 12:
				hours -= 12;
				amPmDesignator = "pm";
				break;
		}
		return $"{hours}:{minutes:00}{amPmDesignator}";
	}
	
	public static int GetHour(string formattedTime) {
		var hour = int.Parse(formattedTime.Split(':')[0]);
		if (formattedTime.Contains("pm"))
			hour += 12;

		return hour;
	}
	
	public static int GetMinute(string formattedTime) {
		var minutesString = formattedTime.Split(':')[1][..2];
		if (minutesString[0] == '0')
			minutesString = minutesString[1].ToString();

		return int.Parse(minutesString);
	}
}
