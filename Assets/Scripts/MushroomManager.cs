/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Linq;
using UnityEngine;

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

		public void FixedUpdate() {
			if (!TimeManager.Running) 
				return;

			foreach (var m in Mushroom.All.Where(m => m.IsOnScreen(CameraController.Camera))) {
				if ((!m.WalkingOnGrass && !m.WalkingOnWood)) continue;
				
				AudioManager.Instance.PlayRandomFootstep(m.WalkingOnGrass);
			}
		}
	#endregion

	#region Other Methods
		public void LevelCompleteAnimations(bool b) {
			foreach (var m in Mushroom.All)
				m.LevelCompleteAnimation(b);
		}
		
		public void Init() {
			var index = 1f;
			foreach (var mushroom in Mushroom.All) {
				mushroom.EnableRenderers(true);
				mushroom.ApplyUniqueMaterials();
				mushroom.MoveAside(index * 100f);
				StartCoroutine(mushroom.TakeHeadshot(0.7f * index));
				index++;
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
