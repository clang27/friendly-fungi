/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class HowMany : Question {
	[SerializeField] private string template, tip;
	[SerializeField] private int answer;
	public override string Header => "How Many";
	public override string Template => template;
	public override string Tip => tip;
	public override bool IsRightAnswer(string s) {
		return s.Equals(answer.ToString());
	}
}