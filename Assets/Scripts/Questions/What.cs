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
	Stick, Bird, Tree, Squid, Butterfly, Rock, Snake, Mouse, Frog, Gecko, Hat, Fish, Sweatband, Glasses, Net, Deer
}

[Serializable]
public class What : Question {
	[SerializeField] private string template;
	[SerializeField] private WhatAnswer answer;
	public override string Header => "What";
	public override string Template => template;
	public static List<string> Choices => Enum.GetNames(typeof(WhatAnswer)).ToList();
	
	public override bool IsRightAnswer(string s) {
		Debug.Log($"{s} == {answer}");
		return s.Equals(answer.ToString());
	}
}
