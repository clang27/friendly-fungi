/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;


// [Serializable]
// public enum MushroomType {
// 	Amanita, Porcini, Shiitake, Morel
// }
public class MushroomManager : MonoBehaviour {
	#region Serialized Fields
		//[SerializeField] private List<MushroomModel> mushroomModels;
	#endregion
	
	#region Attributes
		public static List<Mushroom> AllActiveMushrooms { get; private set; } = new();
	#endregion
	
	#region Components
		private Journal _journal;
	#endregion
	
	#region Unity Events
		private void Awake() {
			_journal = FindObjectOfType<Journal>();
		}
	#endregion

	#region Other Methods
		public void Init() {
			AllActiveMushrooms = FindObjectsOfType<Mushroom>().ToList();

			var gap = 100f;
			foreach (var mushroom in AllActiveMushrooms) {
				mushroom.EnableRenderers(true);
				if(!mushroom.HeadshotCamera.HeadshotTexture)
					StartCoroutine(mushroom.TakeHeadshot(gap));
				gap += 100f;
			}

			StartCoroutine(InitJournal());
		}
		private IEnumerator InitJournal() {
			// Wait for all headshots to be ready
			while (!AllActiveMushrooms.All(m => m.HeadshotCamera.HeadshotTexture))
				yield return null;

			// Populate Journal data
			_journal.Init();
		}

		public void Clear() {
			foreach (var m in AllActiveMushrooms)
				m.EnableRenderers(false);
			
			AllActiveMushrooms.Clear();
		}
	#endregion
}
