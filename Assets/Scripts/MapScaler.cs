/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pinwheel.Poseidon;
using UnityEngine;

public class MapScaler : MonoBehaviour {
    [SerializeField] [Range(0f, 100f)] private float scalingGroupsSize = 50f;
    [SerializeField] [Range(0f, 5f)] private float scalingTime = 0.2f;
    private float _a;
    private int _detailsInPair = 15;
    private int _objectsScaled, _detailsForNoTile, _detailsNow, _scalingTilesCount;

    private bool _reverseScaling, _startScaling, _generated;
    private List<Transform> _scalers = new();
    private List<Vector3> _targetScales;
    public bool Reversed { get; private set; }
    public bool MapReady => _generated && !Scaling;
    private bool Scaling => _reverseScaling || _startScaling;

    private MeshRenderer[] _meshes;
    private ParticleSystem[] _particles;
    private PWater _water;

    private void Awake() {
        _meshes = GetComponentsInChildren<MeshRenderer>()
            .Where(mr => !mr.gameObject.CompareTag("Mushroom")).ToArray();
        _particles = GetComponentsInChildren<ParticleSystem>();
        _water = GetComponentInChildren<PWater>();
    }

    private void Start() {
        EnableMeshes(false);
    }

    public void GenerateMap() {
        ScaleUp();
    }

    public void RemoveMap() {
        StartCoroutine(ScaleDown());
    }

    private void ScaleUp() {
        _generated = true;
        StartScaling(transform);
        EnableMeshes(true);
    }
    
    private IEnumerator ScaleDown() {
        _generated = false;
        ReverseScaling();
        while (!Reversed) 
            yield return new WaitForSeconds(0.1f);
        EnableMeshes(false);
    }

    private void EnableMeshes(bool b) {
        foreach (var mr in _meshes)
            mr.enabled = b;

        foreach (var p in _particles) {
            if (b) 
                p.Play();
            else
                p.Pause();
        }
        
        _water.enabled = b;
    }

    private void FixedUpdate() {
        if (_startScaling) {
            if (_scalers[0]) {
                _a += 1f / (scalingTime / Time.fixedDeltaTime);

                for (var i = 0; i < _detailsNow; i++)
                    if (_objectsScaled + i < _scalers.Count)
                        _scalers[_objectsScaled + i].transform.localScale =
                            Vector3.Lerp(Vector3.zero, _targetScales[_objectsScaled + i], _a);

                if (!(_a >= 1f)) return;

                _objectsScaled += _detailsNow;
                _a = 0f;

                if (_objectsScaled >= _scalers.Count) {
                    _startScaling = false;
                }

                if (_objectsScaled > _scalingTilesCount) _detailsNow = _detailsForNoTile;
            }
            else {
                _startScaling = false;
            }
        }
        else if (_reverseScaling) {
            if (_scalers != null && _scalers[0]) {
                _a += 1f / (scalingTime / Time.fixedDeltaTime) * 2f;

                for (var i = 0; i < _detailsNow; i++)
                    if (_objectsScaled + i < _scalers.Count)
                        _scalers[_objectsScaled + i].transform.localScale =
                            Vector3.Lerp(_targetScales[_objectsScaled + i], Vector3.zero, _a);

                if (!(_a >= 1f)) return;

                _objectsScaled += _detailsNow;
                _a = 0f;

                if (_objectsScaled >= _scalers.Count) {
                    _reverseScaling = false;
                    Reversed = true;
                }

                if (_objectsScaled > _scalers.Count - _scalingTilesCount) _detailsNow = _detailsInPair * 2;
            }
            else {
                _reverseScaling = false;
                Reversed = true;
            }
        }
    }

    private void StartScaling(Transform parent) {
        _objectsScaled = 0;

        if (_scalers.Count > 0) {
            foreach (var s in _scalers)
                s.localScale = Vector3.one;
            
            _scalers.Clear();
        }
        
        var tilesTransforms = new List<Transform>();
        var otherTransforms = new List<Transform>();
        for (var i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name.Contains("Tile"))
                tilesTransforms.Add(parent.GetChild(i));
            else
                otherTransforms.Add(parent.GetChild(i));

        switch (Random.Range(0, 2)) {
            case 0: {
                switch (Random.Range(0, 4)) {
                    case 0:
                        tilesTransforms = tilesTransforms.OrderBy(obj =>
                            obj.transform.position.x + obj.transform.position.z + obj.transform.position.y).ToList();
                        otherTransforms = otherTransforms.OrderBy(obj =>
                            obj.transform.position.x + obj.transform.position.z + obj.transform.position.y).ToList();
                        break;
                    case 1:
                        tilesTransforms = tilesTransforms.OrderBy(obj =>
                                0 - obj.transform.position.x - obj.transform.position.z - obj.transform.position.y)
                            .ToList();
                        otherTransforms = otherTransforms.OrderBy(obj =>
                                0 - obj.transform.position.x - obj.transform.position.z - obj.transform.position.y)
                            .ToList();
                        break;
                    case 2:
                        tilesTransforms = tilesTransforms.OrderBy(obj => obj.transform.position.x).ToList();
                        otherTransforms = otherTransforms.OrderBy(obj => obj.transform.position.x).ToList();
                        break;
                    case 3:
                        tilesTransforms = tilesTransforms.OrderBy(obj => obj.transform.position.y).ToList();
                        otherTransforms = otherTransforms.OrderBy(obj => obj.transform.position.y).ToList();
                        break;
                }

                _detailsInPair = (int)(tilesTransforms.Count * (scalingGroupsSize / 100f));
                _detailsForNoTile = (int)(otherTransforms.Count * (scalingGroupsSize / 100f));
                if (_detailsForNoTile <= 0)
                    _detailsForNoTile = 1;
                _detailsNow = _detailsInPair;
                _scalingTilesCount = tilesTransforms.Count;

                _scalers.AddRange(tilesTransforms);
                _scalers.AddRange(otherTransforms);
                break;
            }
            case 1: {
                tilesTransforms = tilesTransforms.OrderBy(_ => Random.value).ToList();
                otherTransforms = otherTransforms.OrderBy(_ => Random.value).ToList();
                _detailsInPair = (int)(tilesTransforms.Count * (scalingGroupsSize / 100f));
                _detailsForNoTile = (int)(otherTransforms.Count * (scalingGroupsSize / 100f));
                if (_detailsForNoTile <= 0) _detailsForNoTile = 1;
                _detailsNow = _detailsInPair;
                _scalingTilesCount = tilesTransforms.Count;

                _scalers.AddRange(tilesTransforms);
                _scalers.AddRange(otherTransforms);
                break;
            }
        }

        _targetScales = new List<Vector3>();
        foreach (var t in _scalers) {
            _targetScales.Add(t.transform.localScale);
            t.transform.localScale = Vector3.zero;
        }

        _a = 0f;
        _startScaling = true;
    }

    private void ReverseScaling() {
        _objectsScaled = 0;

        _detailsNow = _detailsForNoTile * 2;
        _scalers?.Reverse();

        Reversed = false;
        _reverseScaling = true;
    }
}