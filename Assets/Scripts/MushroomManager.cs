/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
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
		public static List<MushroomData> AllActiveData => AllActive.Select(s => s.Data).ToList();
		public static List<string> AllActiveNames => AllActiveData.Select(md => md.Name).ToList();
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		//private static ObjectPool<Mushroom> Pool;
	#endregion

	#region Other Methods
		public void Init() {
			AllActive = FindObjectsOfType<Mushroom>().ToList();

			foreach (var mushroom in AllActive) {
				var model = mushroomModels.First(model => model.Type == mushroom.Data.Type);
				mushroom.SetMesh(model);
				mushroom.MeshRenderer.enabled = true;
			}
				
		}
		
		public void Clear() {
			foreach (var mr in AllActive.Select(m => m.MeshRenderer))
				mr.enabled = false;
			
			AllActive.Clear();
		}
	#endregion
}
