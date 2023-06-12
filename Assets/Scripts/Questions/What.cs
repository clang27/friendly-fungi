/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum WhatAnswer {
	Stick, Shroo, Tree, Bird, Toad, Butterfly, Heights, Rock, Stairs, Sign, Grass, Fence, Bridge
}

[Serializable]
public class What : Question {
	[SerializeField] private GameObject[] dependencies;
	public override GameObject[] Dependencies => dependencies;

	[SerializeField] private string template;
	[SerializeField] private WhatAnswer answer;
	public override string Header => "What";
	public override string Template => template;
	public static List<string> Choices => Enum.GetNames(typeof(WhatAnswer)).ToList();
	
	public override bool IsRightAnswer(string s) {
		return s.Equals(answer.ToString());
	}
}
