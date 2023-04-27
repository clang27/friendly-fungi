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


[Serializable]
public enum MushroomType {
	Amanita, Porcini, Shiitake, Morel
}
public class MushroomManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private List<MushroomModel> mushroomModels;
	#endregion
	
	#region Attributes
		public static List<Mushroom> AllActive { get; private set; } = new();
		public static Dictionary<string, Sprite> AllActiveOptions { get; } = new();
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
			AllActive = FindObjectsOfType<Mushroom>().ToList();
			AllActiveOptions.Clear();

			foreach (var mushroom in AllActive) {
				var model = mushroomModels.First(model => model.Type == mushroom.Data.Type);
				mushroom.SetMesh(model);
				mushroom.MeshRenderer.enabled = true;
			}
			
			StartCoroutine(TakeHeadshots());
		}

		//TODO: sus
		private IEnumerator TakeHeadshots() {
			// Wait for mesh renderer to start rendering
			yield return new WaitForEndOfFrame();

			var gap = 100f;
			foreach (var m in AllActive.Where(m => !m.HeadshotCamera.HeadshotTexture)) {
				StartCoroutine(m.HeadshotCamera.TakeHeadshot(new Vector3(gap, gap, gap)));
				gap += 100f;
			}

			// Wait for all headshots to be ready
			while (!AllActive.All(m => m.HeadshotCamera.HeadshotTexture))
				yield return null;
			
			foreach (var mushroom in AllActive) {
				AllActiveOptions.Add(
					mushroom.Data.Name, 
					Sprite.Create(mushroom.HeadshotCamera.HeadshotTexture, new Rect(0f, 0f, Utility.HeadshotDimension, Utility.HeadshotDimension), Vector2.zero)
				);
			}
			
			// Populate Journal data
			_journal.Init();
		}

		public void Clear() {
			foreach (var mr in AllActive.Select(m => m.MeshRenderer))
				mr.enabled = false;
			
			AllActive.Clear();
		}
	#endregion
}
