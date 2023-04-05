using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScalerSystem : MonoBehaviour {
    [SerializeField] [Range(0f,100f)] private float scalingGroupsSize = 50f;
    [SerializeField] [Range(0f, 5f)] private float scalingTime = 0.2f;
    public bool Reversed { get; private set; }
    public bool MapReady { get; private set; }

    private bool _reverseScaling, _startScaling;
    private int _objectsScaled, _detailsForNoTile, _detailsNow, _scalingTilesCount;
    private int _detailsInPair = 15;
    private float _a = 0f;
    private List<Transform> _scalers;
    private List<Vector3> _targetScales;
    public void StartScaling(Transform parent) {
        _objectsScaled = 0;
        _scalers = new List<Transform>();
        
        var tilesTransforms = new List<Transform>();
        var otherTransforms = new List<Transform>();
        for (var i = 0; i < parent.childCount; i++) {
            if (parent.GetChild(i).name.Contains("Tile"))
                tilesTransforms.Add(parent.GetChild(i));
            else
                otherTransforms.Add(parent.GetChild(i));
        }

        switch (Random.Range(0, 2)) {
            case 0: {
                switch (Random.Range(0, 4)) {
                    case 0:
                        tilesTransforms = tilesTransforms.OrderBy(obj => obj.transform.position.x + obj.transform.position.z + obj.transform.position.y).ToList();
                        otherTransforms = otherTransforms.OrderBy(obj => obj.transform.position.x + obj.transform.position.z + obj.transform.position.y).ToList();
                        break;
                    case 1:
                        tilesTransforms = tilesTransforms.OrderBy(obj => 0-obj.transform.position.x - obj.transform.position.z - obj.transform.position.y).ToList();
                        otherTransforms = otherTransforms.OrderBy(obj => 0-obj.transform.position.x - obj.transform.position.z - obj.transform.position.y).ToList();
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
                tilesTransforms = tilesTransforms.OrderBy(obj => Random.value).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => Random.value).ToList();
                _detailsInPair = (int)((float)tilesTransforms.Count * (scalingGroupsSize / 100f));
                _detailsForNoTile = (int)((float)otherTransforms.Count * (scalingGroupsSize / 100f));
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
        MapReady = false;
    }
    public void ReverseScaling() {
        _objectsScaled = 0;

        _detailsNow = _detailsForNoTile*2;
        _scalers?.Reverse();

        Reversed = false;
        _reverseScaling = true;
    }
    private void FixedUpdate() {
        if (_startScaling) {
            if (_scalers[0] != null) {
                _a += (1f / (scalingTime / Time.fixedDeltaTime));

                for (var i = 0; i < _detailsNow; i++) {
                    if (_objectsScaled + i < _scalers.Count)
                        _scalers[_objectsScaled + i].transform.localScale = Vector3.Lerp(Vector3.zero, _targetScales[_objectsScaled + i], _a);
                }

                if (!(_a >= 1f)) return;
                
                _objectsScaled += _detailsNow;
                _a = 0f;

                if (_objectsScaled >= _scalers.Count) {
                    MapReady = true;
                    _startScaling = false;
                }
                if(_objectsScaled > _scalingTilesCount) {
                    _detailsNow = _detailsForNoTile;
                }
            } else {
                MapReady = true;
                _startScaling = false;
            }
        } else if(_reverseScaling) {
            if (_scalers != null && _scalers[0] != null) {
               _a += (1f / (scalingTime / Time.fixedDeltaTime))*2f;

                for (var i = 0; i < _detailsNow; i++) {
                    if (_objectsScaled + i < _scalers.Count)
                        _scalers[_objectsScaled + i].transform.localScale = Vector3.Lerp(_targetScales[_objectsScaled + i], Vector3.zero, _a);
                }

                if (!(_a >= 1f)) return;
                
                _objectsScaled += _detailsNow;
                _a = 0f;

                if (_objectsScaled >= _scalers.Count) {
                    _reverseScaling = false;
                    Reversed = true;
                }
                if (_objectsScaled > _scalers.Count - _scalingTilesCount) {
                    _detailsNow = _detailsInPair*2;
                }
            } else {
                _reverseScaling = false;
                Reversed = true;
            }
        }
    }
}
