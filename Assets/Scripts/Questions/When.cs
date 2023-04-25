/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class When : Question {
	[SerializeField] private string template;
	[SerializeField] private int hour, minute;
	public override string Header => "When";
	public override string Template => template;
	public override bool IsRightAnswer(string s) {
		var h = Utility.GetHour(s);
		var m = Utility.GetMinute(s);

		// Within 10 minutes apart
		return (hour - h == 0  && Mathf.Abs(minute - m) < 10) ||	// 4:55pm(guess) & 4:50pm(right answer)
		       (hour - h == -1 && minute - m            > 50) ||	// 5:05pm(guess) & 4:55pm(right answer)
		       (hour - h == 1  && m - minute            > 50);		// 4:55pm(guess) & 5:05pm(right answer)
	}
}
