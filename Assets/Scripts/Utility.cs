/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Linq;
using UnityEngine;

public static class Utility {
	public const int HeadshotDimension = 640;
	public const int GroundMask = 1 << 7;
	public const int RoadMask = 1 << 14;
	
	public static string FormatTime(float time) {
		var hours = Mathf.FloorToInt(time);
		var minutes = Mathf.FloorToInt(60f * (time - hours));
		
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

	public static string ReplaceTemplatedString(string input) {
		return LocationData.AllData.Aggregate(
			MushroomData.AllData.Aggregate(input, (current, md) => current.Replace($"<M{md.Index}>", md.Name))
			, (current, ld) => {
				current = current.Replace($"<LTB{ld.Index}>", $"{ld.Name} {ld.Suffix(LocationType.Building)}");
				current = current.Replace($"<LTC{ld.Index}>", $"{ld.Name} {ld.Suffix(LocationType.Cliff)}");
				current = current.Replace($"<LTF{ld.Index}>", $"{ld.Name} {ld.Suffix(LocationType.Forest)}");
				current = current.Replace($"<LTP{ld.Index}>", $"{ld.Name} {ld.Suffix(LocationType.Pond)}");
				return current;
			});
	}
}
