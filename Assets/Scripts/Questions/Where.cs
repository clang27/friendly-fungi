/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public class Where : Question {
	[SerializeField] private GameObject[] dependencies;
	public override GameObject[] Dependencies => dependencies;
	
	[SerializeField] private string template;
	[SerializeField] private Location answer;
	public override string Header => "Where";
	public override string Template => template;
	
	public override bool IsRightAnswer(string s) {
		return s.Equals(answer.Name);
	}
}
