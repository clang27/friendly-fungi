/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class Question {
	public abstract GameObject[] Dependencies { get; }
	public abstract string Header { get; }
	public abstract string Template { get; }
	public abstract bool IsRightAnswer(string s);

	public string ReplaceNameTemplate() {
		if (Dependencies == null || Dependencies.Length == 0) 
			return Template;
		
		var replacedTemplate = Template;
		
		var hasMushroom = Dependencies.Any(d => d.TryGetComponent(typeof(Mushroom), out _));
		var hasLocation = Dependencies.Any(d => d.TryGetComponent(typeof(Location), out _));

		if (hasMushroom) {
			var mushroom = Dependencies
				.First(d => d.TryGetComponent(typeof(Mushroom), out _)).GetComponent<Mushroom>();

			replacedTemplate = replacedTemplate.Replace("{name}", mushroom.Data.Name);
		} if (hasLocation) {
			var location = Dependencies
				.First(d => d.TryGetComponent(typeof(Location), out _)).GetComponent<Location>();

			replacedTemplate = replacedTemplate.Replace("{location}", location.Name);
		}

		return replacedTemplate;
	}
}