/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum CanAnswer {
	Yes, No
}

[Serializable]
public class Can : Question {
	[SerializeField] private bool convertToDoes;
	[SerializeField] private string template;
	[SerializeField] private CanAnswer answer;
	public override string Header => convertToDoes ? "Does" : "Can";
	public override string Template => template;
	public static List<string> Choices => Enum.GetNames(typeof(CanAnswer)).ToList();
	
	public override bool IsRightAnswer(string s) {
		return s.Equals(answer.ToString());
	}
}