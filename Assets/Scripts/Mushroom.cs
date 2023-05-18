/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class Mushroom : MonoBehaviour, Highlightable {
	#region Attributes
		public MushroomData Data => MushroomData.AllData[MushroomManager.AllActiveMushrooms.IndexOf(this)];
		public HeadshotCamera HeadshotCamera => _headshotCamera;
		public bool Climbing { get; set; }
	#endregion
	
	#region Components
		private Transform _transform;
		private PlayableDirector _playableDirector;
		private HeadshotCamera _headshotCamera;
		private QuickOutline _outline;
	#endregion
	
	#region Private Data
		private Renderer[] _meshRenderers;
		private Material _clonedHeadMaterial, _clonedBodyMaterial;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_transform = transform;
			_meshRenderers = GetComponentsInChildren<Renderer>();
			_outline = GetComponent<QuickOutline>();
			_playableDirector = GetComponent<PlayableDirector>();
			_headshotCamera = GetComponent<HeadshotCamera>();
		}

		private void Start() {
			EnableRenderers(false);
		}

		private void FixedUpdate() {
			if (CameraController.Rotating && !TimeManager.Running && !Climbing) return;
			
			var hits = Physics.RaycastAll(_transform.position + new Vector3(0f, 2f, 0f), Vector3.down, 30f, 1 << 7);
			// Debug.DrawRay(_transform.position + new Vector3(0f, 3f, 0f), new Vector3(0f, -6f, 0f), Color.red);
			// Debug.Log(hitCount);

			if (hits.Length == 0) return;
			
			var goalY = hits.Max(hit => hit.point.y);
			var currY = _transform.position.y;



			var distanceToTranslate = Mathf.Lerp(
				currY, goalY, ((TimeManager.Running) ? 4f : 16f) * Time.fixedDeltaTime) - currY;
			_transform.Translate(Vector3.up * distanceToTranslate);
			
		}

		#endregion
	
	#region Other Methods
		public void EnableRenderers(bool b) {
			foreach (var mr in _meshRenderers)
				mr.enabled = b;
		}
		public void SetTimeline(float f) {
			_playableDirector.time = f;
			_playableDirector.DeferredEvaluate();
		}
		
		public IEnumerator TakeHeadshot(float gap) {
			transform.position = new Vector3(gap, transform.position.y, gap);
			
			while (!_meshRenderers.All(mr => mr.isVisible)) {
				yield return null;
			}

			StartCoroutine(HeadshotCamera.TakeHeadshot());
		}

		public void Click() {
			AudioManager.Instance.PlayUiSound(UiSound.ButtonClick);
			GameManager.Instance.OpenJournalToMushroomPage(this);
		}
		
		public void Highlight(bool b) {
			if (b)
				AudioManager.Instance.PlayUiSound(UiSound.Hover);
			_outline.enabled = b;
		}

	#endregion
}
