/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapEditor : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Vector3 grid;
		[SerializeField] private LayerMask plateLayerMask;
		[Header("Blocks")] 
			[SerializeField] private LayerMask groundLayerMask;
			[SerializeField] private List<GameObject> blockPrefabs;
		[Header("Trees")] 
			[SerializeField] private LayerMask treeLayerMask;
			[SerializeField] private List<GameObject> treePrefabs;
		[Header("Grass")] 
			[SerializeField] private LayerMask grassLayerMask;
			[SerializeField] private List<GameObject> grassPrefabs;
		[Header("Stones")] 
			[SerializeField] private LayerMask stoneLayerMask;
			[SerializeField] private List<GameObject> bigStonePrefabs;
			[SerializeField] private List<GameObject> littleStonePrefabs;
		[Header("Bushes")] 
			[SerializeField] private LayerMask bushLayerMask;
			[SerializeField] private List<GameObject> bushPrefabs;
		[Header("Branches")] 
			[SerializeField] private LayerMask branchLayerMask;
			[SerializeField] private List<GameObject> branchPrefabs;
		[Header("Logs")] 
			[SerializeField] private LayerMask logLayerMask;
			[SerializeField] private List<GameObject> logPrefabs;
		[Header("Roads")] 
			[SerializeField] private LayerMask roadLayerMask;
			[SerializeField] private List<GameObject> stoneRoads, woodRoads, stairs;
	#endregion
	
	#region Components
		private Transform _transform, _plateTransform;
		private Camera _camera;
	#endregion
	
	#region Private Data
		private RaycastHit[] _plateHits = new RaycastHit[1];
		private int _placeFlag = 1;
		private int _height = 1;
		private float _spaceBetweenMisc = 0.5f;
		private int _roadNumber = 0;
		private bool _cooldownMisc = false;
		private bool _cooldownBlock = false;
		private int _blockCount = 0;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_transform = transform;
			_plateTransform = _transform.GetChild(0);
			_camera = FindObjectOfType<Camera>();
		}

		private void Update() {
			var leftClick = Input.GetMouseButton(0);
			var rightClick = Input.GetMouseButton(1);
			var middleClick = Input.GetMouseButtonDown(2);

			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			var hitGround = Physics.Raycast(ray, out var hitGroundInfo, 100f, groundLayerMask);
			var plateHits = Physics.RaycastNonAlloc(ray, _plateHits, 1000f, plateLayerMask);

			if (_placeFlag == 1 && leftClick) {
				PlaceBlock(hitGround, plateHits, hitGroundInfo);
			} else if ((leftClick && hitGround) || rightClick || middleClick) {
				switch (_placeFlag) {
					case 2:
						PlaceMisc(treePrefabs[Random.Range(0, treePrefabs.Count)], treeLayerMask, ray, hitGroundInfo, 1f, rightClick);
						break;
					case 3:
						PlaceMisc(grassPrefabs[Random.Range(0, grassPrefabs.Count)], grassLayerMask, ray, hitGroundInfo, 1f, rightClick);
						break;
					case 4:
						PlaceMisc(bigStonePrefabs[Random.Range(0, bigStonePrefabs.Count)], stoneLayerMask, ray, hitGroundInfo, 1f, rightClick);
						break;
					case 5:
						PlaceMisc(littleStonePrefabs[Random.Range(0, littleStonePrefabs.Count)], stoneLayerMask, ray, hitGroundInfo, 1f, rightClick);
						break;
					case 6:
						PlaceMisc(bushPrefabs[Random.Range(0, bushPrefabs.Count)], bushLayerMask, ray, hitGroundInfo, 1.2f, rightClick);
						break;
					case 7:
						PlaceMisc(branchPrefabs[Random.Range(0, branchPrefabs.Count)], branchLayerMask, ray, hitGroundInfo, 1.2f, rightClick);
						break;
					case 8:
						PlaceMisc(logPrefabs[Random.Range(0, logPrefabs.Count)], logLayerMask, ray, hitGroundInfo, 1.1f,rightClick);
						break;
					case 9:
						PlaceRoad(stoneRoads[Mathf.Clamp(_roadNumber, 0, stoneRoads.Count-1)], roadLayerMask, ray, hitGroundInfo, 0f, rightClick, middleClick);
						break;
					case 10:
						PlaceRoad(woodRoads[Mathf.Clamp(_roadNumber, 0, woodRoads.Count-1)], roadLayerMask, ray, hitGroundInfo, 0f, rightClick, middleClick);
						break;
					case 11:
						PlaceRoad(stairs[Mathf.Clamp(_roadNumber, 0, stairs.Count-1)], roadLayerMask, ray, hitGroundInfo, 2f, rightClick, middleClick);
						break;
				}
			}
		}

	#endregion
	
	#region Other Methods
		private IEnumerator CooldownMisc() {
			_cooldownMisc = true;
			yield return new WaitForSeconds(0.5f);
			_cooldownMisc = false;
		}
		
		private IEnumerator CooldownBlock() {
			_cooldownBlock = true;
			yield return new WaitForSeconds(0.5f);
			_cooldownBlock = false;
		}
		private void PlaceRoad(GameObject prefab, LayerMask lm, Ray r, RaycastHit groundInfo, float offset, bool d, bool rotate) {
			if (_cooldownMisc && !rotate) return;
			StartCoroutine(CooldownMisc());
				
			var hitRoad = Physics.Raycast(r, out var roadInfo, 100f, lm);
			var previousRotation = Quaternion.identity;
				
			if (hitRoad) {
				if (d || rotate)
					Destroy(roadInfo.transform.gameObject);
				if (rotate)
					previousRotation = Quaternion.Euler(0f, roadInfo.transform.localRotation.eulerAngles.y + 90f, 0f);
			}
				
			if (d || groundInfo.transform.name.Contains("Bot")) 
				return;
			if (!rotate && Physics.OverlapBox(groundInfo.point, Vector3.one/2f, _transform.rotation, lm).Length > 0)
				return;

			var road = Instantiate(prefab);

			// Set to mouse point
			var bTransform = road.transform;
			bTransform.parent = groundInfo.transform;
			bTransform.localPosition = new Vector3(0f, offset, 0f);
			bTransform.localRotation = previousRotation;
		}

		private void PlaceMisc(GameObject prefab, LayerMask lm, Ray r, RaycastHit groundInfo, float offset, bool d) {
			if (_cooldownMisc) return;
			StartCoroutine(CooldownMisc());
			
			var hitMisc = Physics.Raycast(r, out var miscInfo, 100f, lm);
			var previousRotation = Quaternion.identity;
			
			if (hitMisc) {
				if (d)
					Destroy(miscInfo.transform.gameObject);
			}
			
			if (d || groundInfo.transform.name.Contains("Bot")) return;
			
			if (!hitMisc) {
				if (Physics.OverlapBox(groundInfo.point, Vector3.one * _spaceBetweenMisc, _transform.rotation, lm).Length > 0) {
					return;
				}

				var misc = Instantiate(prefab);

				// Set to mouse point
				var bTransform = misc.transform;
				bTransform.SetPositionAndRotation(groundInfo.point, groundInfo.transform.rotation);
				bTransform.parent = groundInfo.transform;
				bTransform.localPosition = new Vector3(bTransform.localPosition.x, offset, bTransform.localPosition.z);
				bTransform.localRotation = previousRotation;
			} 
		}
	
		private void PlaceBlock(bool hg, int ph, RaycastHit rh) {
			if (_cooldownBlock) return;
			
			if (ph > 0 && _height > 0 && !hg) {
				Debug.Log("Didn't hit ground, placing block.");
				var block = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Count)]);

				_blockCount++;
				block.name = _blockCount.ToString();

				// Set to mouse point
				var bTransform = block.transform;
				bTransform.SetPositionAndRotation(_plateHits[0].point, _transform.rotation);
				bTransform.parent = _transform;

				// Snap to grid
				var lp = bTransform.localPosition;
				var p = new Vector3(
					(int)(lp.x + AddClosestDivisor(lp.x, grid.x)), 
					(int)(0f + (grid.y * (_height-1))),
					(int)(lp.z + AddClosestDivisor(lp.z, grid.z))
				);
				
				bTransform.GetChild(1).localPosition = Vector3.down * (0.7f * (_height-1));
				bTransform.GetChild(1).localScale = new Vector3(1f, 1f + ((_height-1) * 1.2f), 1f);
				bTransform.SetLocalPositionAndRotation(p, Quaternion.identity);

				var colliders = Physics.OverlapBox(bTransform.position, Vector3.one * 0.6f, _transform.rotation, groundLayerMask);
				if (colliders.Length > 0 && !colliders.All(c => c.transform.parent.name.Equals(block.name))) {
					Debug.Log(colliders[0].transform.parent.name + " near " + p + ". Destroying this new block.");
					_blockCount--;
					Destroy(block);
				} else {
					StartCoroutine(CooldownBlock());
				}
			} else if (_height > 0 && hg) {
				var bTransform = rh.collider.transform.parent;
				var lp = bTransform.localPosition;
				
				if (Mathf.Approximately(lp.y, grid.y * (_height - 1))) return;

				Debug.Log("Hit ground, adjust height to " + _height);
				bTransform.GetChild(1).localPosition = Vector3.down * (0.7f * (_height-1));
				bTransform.GetChild(1).localScale = new Vector3(1f, 1f + ((_height-1) * 1.2f), 1f);
				bTransform.localPosition = new Vector3(lp.x, grid.y * (_height - 1), lp.z);
			} else if (_height == 0 && hg) {
				Debug.Log("Hit ground, destroying block.");
				var bTransform = rh.collider.transform.parent;
				Destroy(bTransform.gameObject);
			}
		}

		private float AddClosestDivisor(float value, float modulo) {
			// ex: 3.2 % 2 > 1, so add 0.8
			if (value % modulo > modulo / 2f) {
				return modulo - (value % modulo);
			} 
			// ex: 2.2 % 2 < 1, so add -0.2
			else {
				return -(value % modulo);
			}
		}
		
		public void ChangeFlag(int i) { 
			_placeFlag = i;
		}

		public void UpdateHeight(Single i) {
			_height = (int)i;
		}
		
		public void UpdateRoadNumber(Single i) {
			_roadNumber = (int)i;
		}

		public void UpdateMapSize(Single i) {
			var width = ((int)i * 4) - 6;
			_plateTransform.localScale = new Vector3(width, 0.1f, width);
		}
		
		public void UpdateSpacing(Single f) {
			_spaceBetweenMisc = f/10f;
		}
	#endregion
}
