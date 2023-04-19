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
		[SerializeField] private GameObject mushroomPrefab;
		[SerializeField] private List<MushroomModel> mushroomModels;
		[SerializeField] private Transform mushroomBench;
	#endregion
	
	#region Attributes
		public static List<MushroomModel> Models { get; private set; }
		public static List<Mushroom> AllActive { get; } = new();
		public static List<MushroomData> AllActiveData => AllActive.Select(s => s.Data).ToList();
		public static List<string> AllActiveNames => AllActiveData.Select(md => md.Name).ToList();
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		private static ObjectPool<Mushroom> Pool;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			Models = mushroomModels;
			
			Pool = new ObjectPool<Mushroom>(
				CreateMushroom, TakeMushroom, ReleaseMushroom, null, true, 10, 10
			);
		}
	#endregion
	
	#region Other Methods
		private Mushroom CreateMushroom() {
			return Instantiate(mushroomPrefab).GetComponent<Mushroom>();
		}

		private void TakeMushroom(Mushroom m) {
			m.gameObject.SetActive(true);
			AllActive.Add(m);
		}
		
		private void ReleaseMushroom(Mushroom m) {
			m.transform.parent = mushroomBench;
			m.gameObject.SetActive(false);
			AllActive.Remove(m);
		}

		public void PopulateWorld(Transform t, Vector3[] spawnPoints) {
			foreach (var sp in spawnPoints) {
				Pool.Get(out var shroom);
				shroom.transform.parent = t;
				shroom.transform.localPosition = sp;
			}
		}
		public void UnpopulateWorld() {
			while(AllActive.Count > 0)
				Pool.Release(AllActive[0]);
		}
	#endregion
}
