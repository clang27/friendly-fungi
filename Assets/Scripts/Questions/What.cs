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
	Stick, Sparrow, Tree, Squid, Butterfly, Rock, Snake, Stairs, Mouse, Bridge, Frog, Gecko, Hat, Fish, Sweatband, Glasses, Net, Deer
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
