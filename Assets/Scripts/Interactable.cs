/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
	#region Serialized Fields
		// [SerializeField] private float fieldOne, fieldTwo, fieldThree;
	#endregion
	
	#region Attributes
		public string Name { get; private set; }
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_transform = transform;
			// _rigidbody = GetComponent<Rigidbody2D>();
			// _collider = GetComponent<Collider2D>();
		}
		
		private void Start() {
			
		}

		private void Update() {
			
		}
		
		private void FixedUpdate() {
			
		}
	#endregion
	
	#region Other Methods
		//private void MethodOne() {}
	#endregion
}
