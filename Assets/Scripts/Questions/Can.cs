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
	[SerializeField] private GameObject[] dependencies;
	public override GameObject[] Dependencies => dependencies;

	[SerializeField] private string template;
	[SerializeField] private CanAnswer answer;

	[SerializeField] private bool freebie;
	public override string Header => "Can";
	public override string Template => template;
	public static List<string> Choices => Enum.GetNames(typeof(CanAnswer)).ToList();
	
	public override bool IsRightAnswer(string s) {
		return freebie || s.Equals(answer.ToString());
	}
}