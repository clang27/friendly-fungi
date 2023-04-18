/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour {
	#region Serialized Fields
		// [SerializeField] private float fieldOne, fieldTwo, fieldThree;
	#endregion
	
	#region Attributes
		public static List<MushroomData> AllShrooms { get; private set; }
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		// Temporary
		private List<MushroomData> _mushroomDatas = new();
	#endregion
	
	#region Unity Methods
		private void Awake() {
			AllShrooms = _mushroomDatas;
		}
		private void Start() {
			_mushroomDatas.Add(MushroomData.RandomMushroom());
			_mushroomDatas.Add(MushroomData.RandomMushroom());
			_mushroomDatas.Add(MushroomData.RandomMushroom());
		}
	#endregion
	
	#region Other Methods
		//private void MethodOne() {}
	#endregion
}
