/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;

[Serializable]
public abstract class Question {
	public abstract string DependentName { get; }
	public abstract string Header { get; }
	public abstract string Template { get; }
	public abstract bool IsRightAnswer(string s);

	public string ReplaceNameTemplate() {
		return !string.IsNullOrEmpty(DependentName) ? 
			Template.Replace("{name}", DependentName) : 
			Template;
	}
}