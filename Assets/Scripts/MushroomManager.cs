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
			var gap = 100f;
			foreach (var mushroom in Mushroom.All) {
				mushroom.EnableRenderers(true);
				mushroom.ApplyUniqueMaterials();
				mushroom.MoveAside(gap);
				StartCoroutine(mushroom.TakeHeadshot());
				gap += 100f;
			}

			StartCoroutine(InitJournal());
		}
		private IEnumerator InitJournal() {
			// Wait for all headshots to be ready
			while (!Mushroom.All.All(m => m.HeadshotCamera.HeadshotTexture))
				yield return null;

			// Populate Journal data
			_journal.Init();
		}
		public void Clear() {
			foreach (var m in Mushroom.All) {
				m.HeadshotCamera.ClearHeadshot();
				m.EnableRenderers(false);
			}
		}
	
	#endregion
}
