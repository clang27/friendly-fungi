/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class Who : Question {
	[SerializeField] private Mushroom dependent;
	
	[SerializeField] private string template;
	[SerializeField] private Mushroom answer;
	public override string Header => "Who";
	public override string Template => template;
	public override string DependentName => (dependent) ? dependent.Data.Name : null;
	public override bool IsRightAnswer(string s) {
		return s.Equals(answer.Data.Name);
	}
}