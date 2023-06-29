/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class Where : Question {
	[SerializeField] private string template;
	[SerializeField] private int answer;
	public override string Header => "Where";
	public override string Template => template;
	
	public override bool IsRightAnswer(string s) {
		Debug.Log($"{s} contains {LocationData.AllData[answer].Name}");
		return s.Contains(LocationData.AllData[answer].Name);
	}
}
