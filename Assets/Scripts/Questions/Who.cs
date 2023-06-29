/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class Who : Question {
	[SerializeField] private string template;
	[SerializeField] private int answer;
	public override string Header => "Who";
	public override string Template => template;
	public override bool IsRightAnswer(string s) {
		Debug.Log($"{s} == {MushroomData.AllData[answer].Name}");
		return s.Equals(MushroomData.AllData[answer].Name);
	}
}