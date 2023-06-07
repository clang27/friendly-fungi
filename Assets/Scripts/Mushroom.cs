/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class Mushroom : MonoBehaviour, Highlightable {
	#region Serialized Data
		[SerializeField] private Texture[] headTextures;
		[SerializeField] private Color[] bodyTints;
		
		[SerializeField] private MeshRenderer headTopMeshRenderer;
		[SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;

		[Header("Accessories")] 
		[SerializeField] private bool IsCatcher;
		[SerializeField] private Color[] accessoryTints;
		[SerializeField] private MeshRenderer fannyPack, butterflyNet, glasses, hat, headband;
	#endregion
	
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
		private Vector3 _startingPosition;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_transform = transform;
			_meshRenderers = GetComponentsInChildren<Renderer>();
			_outline = GetComponent<QuickOutline>();
			_playableDirector = GetComponent<PlayableDirector>();
			_headshotCamera = GetComponent<HeadshotCamera>();

			_startingPosition = _transform.localPosition;
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
				currY, goalY, ((TimeManager.Running) ? 12f : 24f) * Time.fixedDeltaTime) - currY;
			_transform.Translate(Vector3.up * distanceToTranslate);
			
		}

		#endregion
	
	#region Other Methods
		public void ApplyUniqueMaterials() {
			Debug.Log(Data);
			
			headTopMeshRenderer.materials[1].mainTexture = headTextures[Data.HeadColorIndex];
			bodyMeshRenderer.material.color = bodyTints[Data.BodyColorIndex];
			
			if (Data.GlassesColorIndex > 0)
				glasses.material.color = accessoryTints[Data.GlassesColorIndex - 1];
			if (Data.HatColorIndex > 0)
				hat.materials[1].color = accessoryTints[Data.HatColorIndex - 1];
			if (Data.HeadbandColorIndex > 0)
				headband.materials[0].color = accessoryTints[Data.HeadbandColorIndex - 1];
		}
		public void EnableRenderers(bool b) {
			foreach (var mr in _meshRenderers)
				mr.enabled = b;
			
			fannyPack.gameObject.SetActive(b && IsCatcher);
			butterflyNet.gameObject.SetActive(b && IsCatcher);

			glasses.gameObject.SetActive(b && !IsCatcher && Data.GlassesColorIndex > 0);
			hat.gameObject.SetActive(b && !IsCatcher && Data.HatColorIndex > 0);
			headband.gameObject.SetActive(b && !IsCatcher && Data.HeadbandColorIndex > 0);
		}
		public void SetTimeline(float f) {
			//Debug.Log("Setting time to " + f);
			_playableDirector.time = f;
			_playableDirector.DeferredEvaluate();
		}

		public void ResetPosition() {
			_transform.localPosition = _startingPosition;
		}
		
		public IEnumerator TakeHeadshot() {
			yield return new WaitForSeconds(1f);
			
			Debug.Log("Taking " + Data.Name + "'s headshot.");
			StartCoroutine(HeadshotCamera.TakeHeadshot());
		}

		public void MoveAside(float gap) {
			var pos = new Vector3(gap, transform.position.y, gap);
			
			Debug.Log("Moving aside to " + pos);
			_transform.position = pos;
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
