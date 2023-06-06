/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public enum CanAnswer {
	Yes, No
}

[Serializable]
public class Can : Question {
	[SerializeField] private Mushroom mushroomDependent;
	[SerializeField] private Location locationDependent;
	
	[SerializeField] private string template;
	[SerializeField] private CanAnswer answer;
	
	[SerializeField] private bool freebie;
	public override string Header => "Can";
	public override string Template => template;
	public override string DependentName => (mushroomDependent) ? mushroomDependent.Data.Name : null;
	public override string DependentLocation => (locationDependent) ? locationDependent.Name : null;
	public override bool IsRightAnswer(string s) {
		return freebie || s.Equals(answer.ToString());
	}
}