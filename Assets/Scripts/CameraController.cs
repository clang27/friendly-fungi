/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private class CameraState {
        private float _roll;
        private float _x, _y, _z;
        public float Pitch;
        public float Yaw;
        public float Zoom;

        private float _initialZoom;
        private Vector3 _initialPosition, _initialAngle;

        public void Init(Transform t, float i) {
            _initialPosition = t.position;
            _initialAngle = t.eulerAngles;
            _initialZoom = i;

            Reset();
        }

        public void Translate(Vector3 translation) {
            var rotatedTranslation = Quaternion.Euler(Pitch, Yaw, _roll) * translation;

            _x += rotatedTranslation.x;
            _y += rotatedTranslation.y;
            _z += rotatedTranslation.z;
        }

        public void AddZoom(float f, float minZoom, float maxZoom) {
            Zoom += f;
            Zoom = Mathf.Clamp(Zoom, maxZoom, minZoom); // Higher the number, the more zoomed out
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct, float zoomLerpPct) {
            Zoom = Mathf.Lerp(Zoom, target.Zoom, zoomLerpPct);

            Yaw = Mathf.Lerp(Yaw, target.Yaw, rotationLerpPct);
            Pitch = Mathf.Lerp(Pitch, target.Pitch, rotationLerpPct);
            _roll = Mathf.Lerp(_roll, target._roll, rotationLerpPct);

            _x = Mathf.Lerp(_x, target._x, positionLerpPct);
            _y = Mathf.Lerp(_y, target._y, positionLerpPct);
            _z = Mathf.Lerp(_z, target._z, positionLerpPct);
        }

        public void UpdateTransform(Transform t) {
            t.eulerAngles = new Vector3(Pitch, Yaw, _roll);
            t.position = new Vector3(_x, _y, _z);
        }

        public void UpdateZoom(Camera c) {
            c.orthographicSize = Zoom;
        }

        public void Reset() {
            Zoom = _initialZoom;
            
            Pitch = _initialAngle.x;
            Yaw = _initialAngle.y;
            _roll = _initialAngle.z;

            ResetPosition();
        }
        
        public void ResetPosition() {
            _x = _initialPosition.x;
            _y = _initialPosition.y;
            _z = _initialPosition.z;
            
        }
    }

    private Highlightable _highlightedObject;
    private Coroutine _resettingPositionToPlay;
    private Camera _camera;
    public static Camera Camera;
    private ClickableObject _clickableObject;
    private Transform _transform;
    private Vector3 _goalRotation, _startingRotation, _startingPosition;
    private Transform _worldTransform;
    private float _initialYaw, _initialPitch;
    private readonly CameraState _targetCameraState = new(), _interpolatingCameraState = new();
    
    public bool AutoRotate { get; set; }
    public static bool Rotating { get; private set; }
    public bool Enabled { get; set; }
    public bool Ready => _resettingPositionToPlay == null;

    [Header("Menu Settings")]
    [Range(0.1f, 1f)] public float startingZoomOutPercent = 0.7f;
    [Range(0.1f, 1f)] public float victoryZoomOutPercent = 0.9f;
    public Vector3 startingTranslation = new Vector3(-10f, -2f, 0f);
    
    [Header("Lerp Settings")]
    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target.")]
    [Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;

    [Range(0.001f, 1f)]
    public float zoomLerpTime = 0.2f;
    
    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target.")] [Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;
    
    [Tooltip("Time it takes to interpolate world rotation 99% of the way to the target.")] [Range(0.001f, 1f)]
    public float worldRotationLerpTime = 0.001f;

    [Header("Zoom Settings")]
    public float maxZoomIn = 4f;
    public float minZoomIn = 17f;

    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
    private void Awake() {
        _transform = transform;
        _camera = GetComponent<Camera>();
        Camera = _camera;
        _clickableObject = GetComponent<ClickableObject>();
    }

    private void Start() {
        _targetCameraState.Init(_transform, _camera.orthographicSize);
        _interpolatingCameraState.Init(_transform, _camera.orthographicSize);
        _worldTransform = GameObject.FindGameObjectWithTag("Map").transform;
        _goalRotation = _worldTransform.localRotation.eulerAngles;
        _startingRotation = _worldTransform.localRotation.eulerAngles;
        _startingPosition = _worldTransform.localPosition;
        
        _worldTransform.localScale = Vector3.one * startingZoomOutPercent;
        _targetCameraState.Translate(startingTranslation);
        _interpolatingCameraState.Translate(startingTranslation);
    }

    private Vector3 GetInputRotationDirection() {
        var direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.up;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.down;
        
        return direction;
    }

    private void Update() {
        if (Enabled) {
            if (IsRightMouseButtonDown()) {
                _highlightedObject?.Highlight(false);
            
                _targetCameraState.Translate(GetMouseLocation());
                _targetCameraState.AddZoom(-18f, minZoomIn, maxZoomIn);
                Cursor.lockState = CursorLockMode.Locked;
            
                GameManager.Instance.ShowBinoculars();
            } else if (IsRightMouseButtonUp()) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _targetCameraState.Reset();
            
                GameManager.Instance.HideBinoculars();
            } 

            if (RightMouseHeld()) {
                var mouseMovement = GetInputLookRotation() * Settings.MouseRotateSensitivity;
                if (Settings.InvertLookY)
                    mouseMovement.y = -mouseMovement.y;

                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                _targetCameraState.Translate(
                    new Vector3(mouseMovement.x * mouseSensitivityFactor, mouseMovement.y * mouseSensitivityFactor, 0f)
                );
                // _targetCameraState.Yaw += mouseMovement.x * mouseSensitivityFactor;
                // _targetCameraState.Pitch += mouseMovement.y * mouseSensitivityFactor;
            } else if (_highlightedObject != null && IsLeftMouseButtonDown()) {
                _highlightedObject.Highlight(false);
                _highlightedObject.Click();
                _highlightedObject = null;
            }
        }

        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var worldRotationLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / worldRotationLerpTime * Time.deltaTime);
        var positionLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / positionLerpTime * Time.deltaTime);
        var cameraRotationLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / rotationLerpTime * Time.deltaTime);
        var zoomLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / zoomLerpTime * Time.deltaTime);

        _interpolatingCameraState.LerpTowards(_targetCameraState, positionLerpPct, cameraRotationLerpPct, zoomLerpPct);
        _interpolatingCameraState.UpdateTransform(_transform);
        _interpolatingCameraState.UpdateZoom(_camera);

        if (_worldTransform) {
            if (AutoRotate) {
                _goalRotation += Vector3.up * (Settings.RotateSpeed * (Settings.InvertWorldRotation ? -1f : 1f) * 0.2f);
            } else if (Enabled) {
                _goalRotation += GetInputRotationDirection() * (
                    Settings.RotateSpeed * (IsBoostPressed() ? Settings.BoostMultiplier : 1f) * 
                    (RightMouseHeld() ? 0.2f : 1f) * (Settings.InvertWorldRotation ? 1f : -1f)
                );
            }
            
            _worldTransform.localRotation = Quaternion.Lerp(_worldTransform.localRotation, Quaternion.Euler(_goalRotation), worldRotationLerpPct);
        }
        
        Rotating = _worldTransform.localRotation != Quaternion.Euler(_goalRotation);
    }

    private void FixedUpdate() {
        if (!Enabled) return;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var objTrans = _clickableObject.TouchingRay(ray);
        var wasHighlighted = _highlightedObject != null;

        if (wasHighlighted && !objTrans) {
            _highlightedObject?.Highlight(false);
            _highlightedObject = null;
        }
            
        if (!objTrans)
            return;
        
        var newHighlightedObject = objTrans.GetComponent<Highlightable>();
        
        if (newHighlightedObject != null) {
            _highlightedObject = newHighlightedObject;
            if (!wasHighlighted)
                _highlightedObject.Highlight(!RightMouseHeld());
        }
        
    }
    
    public void ResetWorldPositionToPlay() {
        _targetCameraState.ResetPosition();
        
        _worldTransform.DOKill();
        _worldTransform.DOScale(Vector3.one, 1f);
        
        _resettingPositionToPlay = StartCoroutine(MoveTowardsReset());
    }
    
    public void ResetWorldPositionToMenu() {
        if (_resettingPositionToPlay != null) {
            StopCoroutine(_resettingPositionToPlay);
            _resettingPositionToPlay = null;
        } 
        
        _targetCameraState.Translate(startingTranslation);
        
        _worldTransform.DOKill();
        _worldTransform.DOScale(Vector3.one * startingZoomOutPercent, 1f);
        _worldTransform.DOLocalMove(_startingPosition, 1f);
    }
    
    public void ResetWorldPositionToVictory() {
        _worldTransform.DOKill();
        _worldTransform.DOScale(Vector3.one * victoryZoomOutPercent, 3f);
        _worldTransform.DOLocalMoveY(_startingPosition.y + 3f, 2f);
    }

    private IEnumerator MoveTowardsReset() {
        while (Mathf.Abs(Mathf.DeltaAngle(_goalRotation.y, _startingRotation.y)) > 0.5f) {
            _goalRotation = new Vector3(
                _startingRotation.x, 
                Mathf.LerpAngle(_goalRotation.y, _startingRotation.y, Time.deltaTime * 3f),
                _startingRotation.z);
            
            yield return new WaitForEndOfFrame();
        }

        _resettingPositionToPlay = null;
    }

    private Vector2 GetInputLookRotation() { 
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private bool IsBoostPressed() {
        return Input.GetKey(KeyCode.LeftShift);
    }

    private bool RightMouseHeld() {
        return Input.GetMouseButton(1);
    }

    private Vector2 GetMouseLocation() {
        var camPoint = (_camera.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f)) *
                       GetZoomTranslationMultiplier();
        return new Vector2(camPoint.x * 96f, camPoint.y * 54f);
    }

    private float GetZoomTranslationMultiplier() {
        return 0.038f * Mathf.Pow(_camera.orthographicSize, 0.985f);
    }

    private bool IsRightMouseButtonDown() {
        return Input.GetMouseButtonDown(1);
    }

    private bool IsRightMouseButtonUp() {
        return Input.GetMouseButtonUp(1);
    }
    
    private bool IsLeftMouseButtonDown() {
        return Input.GetMouseButtonDown(0);
    }
}