/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public static class Utility {
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
}
