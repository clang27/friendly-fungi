/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Mushroom : MonoBehaviour, Highlightable {
	#region Serialized Data
		[SerializeField] private bool known;
		[SerializeField] private int index;
	
		[SerializeField] private Texture[] headTextures;
		[SerializeField] private Color[] bodyTints;
		
		[SerializeField] private MeshRenderer headTopMeshRenderer;
		[SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;

		[Header("Accessories")]
		[SerializeField] private bool requiresNet;
		[SerializeField] private bool requiresHat, requiresNoHat, requiresGlasses;
		[SerializeField] private Color[] accessoryTints;
		[SerializeField] private MeshRenderer fannyPack, butterflyNet, glasses, hat, headband;
	#endregion
	
	#region Attributes
		public bool WalkingOnGrass { get; private set; }
		public bool WalkingOnWood { get; private set; }
		public int Index => index;
		public bool Known => known;
	    public static List<Mushroom> All { get; } = new();
		public MushroomData Data => MushroomData.AllData[Index];
		public HeadshotCamera HeadshotCamera => _headshotCamera;
		private bool VocalCooldown { get; set; }
	#endregion
	
	#region Components
		private Transform _transform;
		private HeadshotCamera _headshotCamera;
		private QuickOutline _outline;
		private Animator _animator;
	#endregion
	
	#region Private Data
		private Renderer[] _meshRenderers;
		private RaycastHit[] _hits = new RaycastHit[2];
		private static readonly int Cheer = Animator.StringToHash("Cheer");
		private static readonly int Pose = Animator.StringToHash("Pose");
		private Vector3 lastPosition = Vector3.zero;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_transform = transform;
			_meshRenderers = GetComponentsInChildren<Renderer>();
			_outline = GetComponent<QuickOutline>();
			_animator = GetComponent<Animator>();
			_headshotCamera = GetComponent<HeadshotCamera>();
			
			All.Add(this);
		}

		private void FixedUpdate() {
			if (Vector3.Distance(lastPosition, _transform.position) < 1f) {
				WalkingOnWood = false;
				WalkingOnGrass = false;
				return;
			}
			
			WalkingOnWood = Physics.RaycastNonAlloc(_transform.position + Vector3.up, Vector3.down, _hits, 1.2f, Utility.RoadMask) > 0;
			WalkingOnGrass = !WalkingOnWood &&
			                 Physics.RaycastNonAlloc(_transform.position + Vector3.up, Vector3.down, _hits, 1.2f, Utility.GroundMask) > 0;

			lastPosition = _transform.position;
		}
        
		private void OnDestroy() {
			All.Remove(this);
		}
	#endregion
	
	#region Other Methods
		public void ApplyUniqueMaterials() {
			//Debug.Log(Data);
			
			headTopMeshRenderer.materials[1].mainTexture = headTextures[Data.HeadColorIndex];
			bodyMeshRenderer.material.color = bodyTints[Data.BodyColorIndex];
			
			if (Data.GlassesColorIndex > 0 && !requiresGlasses)
				glasses.material.color = accessoryTints[Data.GlassesColorIndex - 1];
			if (Data.HatColorIndex > 0 && !requiresHat)
				hat.materials[1].color = accessoryTints[Data.HatColorIndex - 1];
			if (Data.HeadbandColorIndex > 0)
				headband.materials[0].color = accessoryTints[Data.HeadbandColorIndex - 1];
		}
		public void EnableRenderers(bool b) {
			foreach (var mr in _meshRenderers)
				mr.enabled = b;
			
			fannyPack.gameObject.SetActive(b && requiresNet);
			butterflyNet.gameObject.SetActive(b && requiresNet);

			glasses.gameObject.SetActive(b && (Data.GlassesColorIndex > 0 || requiresGlasses));
			var hasHat = (Data.HatColorIndex > 0 || requiresHat) && !requiresNoHat;
			hat.gameObject.SetActive(b && hasHat);
			headband.gameObject.SetActive(b && Data.HeadbandColorIndex > 0 && !hasHat);
		}
		public IEnumerator TakeHeadshot(float waitTime) {
			yield return new WaitForSeconds(waitTime);
			
			//Debug.Log("Taking " + Data.Name + "'s headshot.");
			StartCoroutine(HeadshotCamera.TakeHeadshot());
		}

		public void MoveAside(float gap) {
			var pos = new Vector3(gap, transform.position.y, gap);
			
			//Debug.Log("Moving aside to " + pos);
			_transform.position = pos;
		}

		public void Click() {
			AudioManager.Instance.PlayUiSound(UiSound.Mushroom);
			GameManager.Instance.OpenJournalToMushroomPage(this);
		}
		
		public void Highlight(bool b) {
			if (b)
				AudioManager.Instance.PlayUiSound(UiSound.Hover);
			_outline.enabled = b;
		}

		public bool IsOnScreen(Camera c) {
			if (!c)
				return false;
			
			var sp = c.WorldToScreenPoint(_transform.position);
			var vp = c.ScreenToViewportPoint(sp);
			
			return (vp.x is > 0f and < 1f) && (vp.y is > 0f and < 1f);
		}

		public void LevelCompleteAnimation(bool b) {
			_animator.SetTrigger(b ? Cheer : Pose);
		}
		
		public void PlayVocalSound(int i) {
			if (VocalCooldown) return;
			if (!IsOnScreen(CameraController.Camera)) return;

			AudioManager.Instance.PlayShrooSound(i);
			VocalCooldown = true;
			DOVirtual.DelayedCall(2f / TimeManager.SecondMultiplier, () => VocalCooldown = false);
		}

	#endregion
}
