/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;


[Serializable]
public enum QuestionType {
	Who, What, When, Where, Why
}

[Serializable]
public enum Difficulty {
	Easy, Medium, Hard
}

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Question", order = 1)]
public class Question : ScriptableObject {
	public QuestionType Type;
	public Difficulty Difficulty;
	public string Verse;
}
