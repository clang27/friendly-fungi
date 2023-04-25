/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;

[Serializable]
public abstract class Question {
	public abstract string Header { get; }
	public abstract string Template { get; }
	public string Phrase => Header + " " + Template;
	public abstract bool IsRightAnswer(string s);

	private string ReplaceNameTemplate(string n) {
		return Template.Replace("{name}", n);
	}
}