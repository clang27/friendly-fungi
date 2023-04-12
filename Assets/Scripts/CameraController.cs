#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
            
            _x = _initialPosition.x;
            _y = _initialPosition.y;
            _z = _initialPosition.z;
            
        }
    }
    
    private Camera _camera;
    private Transform _transform;
    private Vector3 _goalRotation;
    private Transform _worldTransform;
    private float _initialYaw, _initialPitch;
    private readonly CameraState _targetCameraState = new(), _interpolatingCameraState = new();
    
    public bool AutoRotate { get; set; }
    public bool Enabled { get; set; }

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
    public float zoomTranslationMultiplier = 30f;

    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

#if ENABLE_INPUT_SYSTEM
    InputAction movementAction;
    InputAction verticalMovementAction;
    InputAction lookAction;
    InputAction zoomAction;
    bool        mouseRightButtonPressed;

    private void Start() {
        var map = new InputActionMap("Simple Camera Controller");

        lookAction = map.AddAction("look", binding: "<Mouse>/delta");
        movementAction = map.AddAction("move", binding: "<Gamepad>/leftStick");
        verticalMovementAction = map.AddAction("Vertical Movement");
        zoomAction = map.AddAction("Boost Factor", binding: "<Mouse>/scroll");

        lookAction.AddBinding("<Gamepad>/rightStick").WithProcessor("scaleVector2(x=15, y=15)");
        movementAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
        verticalMovementAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/pageUp")
            .With("Down", "<Keyboard>/pageDown")
            .With("Up", "<Keyboard>/e")
            .With("Down", "<Keyboard>/q")
            .With("Up", "<Gamepad>/rightshoulder")
            .With("Down", "<Gamepad>/leftshoulder");
        zoomAction.AddBinding("<Gamepad>/Dpad").WithProcessor("scaleVector2(x=1, y=4)");

        movementAction.Enable();
        lookAction.Enable();
        verticalMovementAction.Enable();
        zoomAction.Enable();
    }

#endif
    private void Awake() {
        _transform = transform;
        _camera = GetComponent<Camera>();
    }

    private void Start() {
        _targetCameraState.Init(_transform, _camera.orthographicSize);
        _interpolatingCameraState.Init(_transform, _camera.orthographicSize);
    }

    public void Init() {
        _worldTransform = GameObject.FindGameObjectWithTag("Map").transform;
        _goalRotation = _worldTransform.localRotation.eulerAngles;
    }

    private Vector3 GetInputRotationDirection() {
        var direction = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
        var moveDelta = movementAction.ReadValue<Vector2>();
        direction.x = moveDelta.x;
        direction.z = moveDelta.y;
        direction.y = verticalMovementAction.ReadValue<Vector2>().y;
#else
        // if (Input.GetKey(KeyCode.W))
        //     direction += Vector3.up;
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.up;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.down;
        // } if (Input.GetKey(KeyCode.S)) {
        //     direction += Vector3.right;
        // } 
        //
        // if (Input.GetKey(KeyCode.Q)) {
        //     direction += Vector3.down;
        // } if (Input.GetKey(KeyCode.E)) {
        //     direction += Vector3.up;
        // }
#endif
        return direction;
    }

    private void Update() {
        if (!Enabled) return;
        
        if (IsRightMouseButtonDown()) {
            _targetCameraState.Translate(GetMouseLocation());
            _targetCameraState.AddZoom(-18f, minZoomIn, maxZoomIn);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (IsRightMouseButtonUp()) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _targetCameraState.Reset();
        }

        if (RightMouseHeld()) {
            var mouseMovement = GetInputLookRotation() * Settings.MouseRotateSensitivity;
            if (Settings.InvertCameraY)
                mouseMovement.y = -mouseMovement.y;

            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            _targetCameraState.Translate(
                new Vector3(mouseMovement.x * mouseSensitivityFactor, mouseMovement.y * mouseSensitivityFactor, 0f)
            );
            // _targetCameraState.Yaw += mouseMovement.x * mouseSensitivityFactor;
            // _targetCameraState.Pitch += mouseMovement.y * mouseSensitivityFactor;
        }
    }

    private void LateUpdate() {
        //_targetCameraState.AddZoom(GetZoom(), minZoomIn, maxZoomIn);

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
                _goalRotation += Vector3.down * (Settings.RotateSpeed * (Settings.InvertWorldRotateX ? -1f : 1f) * 0.2f);
            } else {
                _goalRotation += GetInputRotationDirection() * (
                    Settings.RotateSpeed * (IsBoostPressed() ? Settings.BoostMultiplier : 1f) * 
                    (RightMouseHeld() ? 0.2f : 1f) * (Settings.InvertWorldRotateX ? -1f : 1f)
                );
            }
            
            _worldTransform.localRotation = Quaternion.Lerp(_worldTransform.localRotation, Quaternion.Euler(_goalRotation), worldRotationLerpPct);
        }
    }

    // private float GetZoom() {
    //     #if ENABLE_INPUT_SYSTEM
    //         return zoomAction.ReadValue<Vector2>().y * -mouseZoomSensitivity;
    //     #else
    //         //return Input.mouseScrollDelta.y * -mouseZoomSensitivity;
    //         return Input.mouseScrollDelta.y * -mouseZoomSensitivity;
    //     #endif
    // }

    private Vector2 GetInputLookRotation() {
        // try to compensate the diff between the two input systems by multiplying with empirical values
#if ENABLE_INPUT_SYSTEM
            var delta = lookAction.ReadValue<Vector2>();
            delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
            delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.
            return delta;
#else
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
    }

    private bool IsBoostPressed() {
#if ENABLE_INPUT_SYSTEM
            bool boost = Keyboard.current != null ? Keyboard.current.leftShiftKey.isPressed : false;
            boost |= Gamepad.current != null ? Gamepad.current.xButton.isPressed : false;
            return boost;
#else
        return Input.GetKey(KeyCode.LeftShift);
#endif
    }

    private bool RightMouseHeld() {
#if ENABLE_INPUT_SYSTEM
            bool canRotate = Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
            canRotate |= Gamepad.current != null ? Gamepad.current.rightStick.ReadValue().magnitude > 0 : false;
            return canRotate;
#else
        return Input.GetMouseButton(1);
#endif
    }

    private Vector2 GetMouseLocation() {
        var camPoint = (_camera.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f)) *
                       zoomTranslationMultiplier;
        return new Vector2(camPoint.x * Screen.width, camPoint.y * Screen.height);
    }

    private bool IsRightMouseButtonDown() {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
#else
        return Input.GetMouseButtonDown(1);
#endif
    }

    private bool IsRightMouseButtonUp() {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? !Mouse.current.rightButton.isPressed : false;
#else
        return Input.GetMouseButtonUp(1);
#endif
    }
}