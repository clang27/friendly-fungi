/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		public bool WalkingOnGrass => _timeManipulation.WalkingOnGrass;
		public bool WalkingOnWood => _timeManipulation.WalkingOnWood;
		public int Index => All.IndexOf(this);
	    public static List<Mushroom> All { get; } = new();
		public MushroomData Data => MushroomData.AllData[Index];
		public HeadshotCamera HeadshotCamera => _headshotCamera;
	#endregion
	
	#region Components
		private Transform _transform;
		private HeadshotCamera _headshotCamera;
		private QuickOutline _outline;
		private TimeManipulation _timeManipulation;
	#endregion
	
	#region Private Data
		private Renderer[] _meshRenderers;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_transform = transform;
			_meshRenderers = GetComponentsInChildren<Renderer>();
			_outline = GetComponent<QuickOutline>();
			_headshotCamera = GetComponent<HeadshotCamera>();
			_timeManipulation = GetComponent<TimeManipulation>();
			
			All.Add(this);
		}
        
		private void OnDestroy() {
			All.Remove(this);
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

		public bool IsOnScreen(Camera c) {
			var sp = c.WorldToScreenPoint(_transform.position);
			var vp = c.ScreenToViewportPoint(sp);
			
			return (vp.x is > 0f and < 1f) && (vp.y is > 0f and < 1f);
		}

	#endregion
}
