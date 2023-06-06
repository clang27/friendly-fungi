/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class HowMany : Question {
	[SerializeField] private Mushroom mushroomDependent;
	[SerializeField] private Location locationDependent;
	
	[SerializeField] private string template;
	[SerializeField] private int answer;
	public override string Header => "How Many";
	public override string Template => template;
	public override string DependentName => (mushroomDependent) ? mushroomDependent.Data.Name : null;
	public override string DependentLocation => (locationDependent) ? locationDependent.Name : null;
	public override bool IsRightAnswer(string s) {
		return s.Equals(answer.ToString());
	}
}