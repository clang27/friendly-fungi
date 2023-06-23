/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MushroomManager : MonoBehaviour {
	#region Serialized Fields
		//[SerializeField] private List<MushroomModel> mushroomModels;
	#endregion
	
	#region Components
		private Journal _journal;
	#endregion
	
	#region Private Data
		private bool[] _footstepCooldowns;
	#endregion
	
	#region Unity Events
		private void Awake() {
			_journal = FindObjectOfType<Journal>();
		}

		public void FixedUpdate() {
			if (!TimeManager.Running) 
				return;

			foreach (var m in Mushroom.All.Where(m => m.IsOnScreen(CameraController.Camera))) {
				if (_footstepCooldowns[m.Index] || (!m.WalkingOnGrass && !m.WalkingOnWood)) continue;
				
				AudioManager.Instance.PlayRandomFootstep(m.WalkingOnGrass);
				StartCoroutine(FootstepCooldown(m.Index));
			}
		}
	#endregion

	#region Other Methods
		public IEnumerator FootstepCooldown(int i) {
			_footstepCooldowns[i] = true;
			yield return new WaitForSeconds(Random.Range(0.75f, 1f));
			_footstepCooldowns[i] = false;
		}
		
		public void Init() {
			_footstepCooldowns = new bool[Mushroom.All.Count];
				
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
