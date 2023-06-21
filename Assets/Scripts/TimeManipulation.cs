/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class TimeManipulation : MonoBehaviour {
	#region Serialize Fields
		[SerializeField] private int priority;
		[SerializeField] private LayerMask groundLayers;
	#endregion
	
	#region Attributes
		public Vector3 RaycastPosition { get; set; }
		public int Priority => priority;
		public bool UseGravity { get; set; }
		public bool WalkingOnGrass { get; set; }
		public bool WalkingOnWood { get; set; }
	#endregion
	
	#region Components
		private PlayableDirector _playableDirector;
		private Transform _transform;
	#endregion

	#region Unity Methods
		private void Awake() {
			_transform = transform;
			_playableDirector = GetComponent<PlayableDirector>();
		}
		
		private void FixedUpdate() {
			if (!UseGravity) return;
			if (!TimeManager.Running && CameraController.Rotating) return;
			
			var hits = Physics.RaycastAll(RaycastPosition + new Vector3(0f, 2f, 0f), Vector3.down, 30f, groundLayers);
			// Debug.DrawRay(_transform.position + new Vector3(0f, 3f, 0f), new Vector3(0f, -6f, 0f), Color.red);
			// Debug.Log(hitCount);

			if (hits.Length == 0) return;
			
			var goalY = hits.Max(hit => hit.point.y);
			var currY = _transform.position.y;

			var distanceToTranslate = Mathf.Lerp(
				currY, goalY, ((TimeManager.Running) ? 12f : 48f) * Time.fixedDeltaTime) - currY;
			_transform.Translate(Vector3.up * distanceToTranslate);
			
		}
	#endregion
	
	#region Other Methods
		public void SetTimeline(float f) {
			//Debug.Log("Setting time to " + f);
			_playableDirector.time = f;
			_playableDirector.DeferredEvaluate();
		}
	#endregion
}
