#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;

public class CameraController : MonoBehaviour {
    private class CameraState {
        public float Yaw;
        public float Pitch;
        public float Zoom = 20f;

        private float _roll;
        private float _x, _y, _z;
        
        public void Init(Transform t) {
            Pitch = t.eulerAngles.x;
            Yaw = t.eulerAngles.y;
            _roll = t.eulerAngles.z;
            
            _x = t.position.x;
            _y = t.position.y;
            _z = t.position.z;
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

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct) {
            Zoom = Mathf.Lerp(Zoom, target.Zoom, positionLerpPct);
            
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
    }

    [SerializeField] private Transform _worldTransform;
    private const float MouseSensitivityMultiplier = 0.01f;
    private Camera _camera;
    private Transform _transform;
    private readonly CameraState _targetCameraState = new (), _interpolatingCameraState = new ();

    [Header("Lerp Settings")]
    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;
    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;

    [Header("Zoom Settings")] 
    public float maxZoomIn = 4f;
    public float minZoomIn = 17f;

    [Header("Mouse Settings")]
    public float mouseRotateSensitivity = 60.0f;
    public float mouseZoomSensitivity = 1f;

    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

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
    private void OnEnable() {
        _targetCameraState.Init(_transform);
        _interpolatingCameraState.Init(_transform);
    }

    private Vector3 GetInputTranslationDirection() {
        var direction = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
        var moveDelta = movementAction.ReadValue<Vector2>();
        direction.x = moveDelta.x;
        direction.z = moveDelta.y;
        direction.y = verticalMovementAction.ReadValue<Vector2>().y;
#else
        // if (Input.GetKey(KeyCode.W)) {
        //     direction += Vector3.up;
        // } if (Input.GetKey(KeyCode.S)) {
        //     direction += Vector3.down;
        // } if (Input.GetKey(KeyCode.A)) {
        //     direction += Vector3.left;
        // } if (Input.GetKey(KeyCode.D)) {
        //     direction += Vector3.right;
        // } 
        
        // if (Input.GetKey(KeyCode.Q)) {
        //     direction += Vector3.down;
        // } if (Input.GetKey(KeyCode.E)) {
        //     direction += Vector3.up;
        // }
#endif
        return direction;
    }

    private void Update() {
        if (IsRightMouseButtonDown())  // Hide and lock cursor when right mouse button pressed
            Cursor.lockState = CursorLockMode.Locked;
        else if (IsRightMouseButtonUp()) {  // Unlock and show cursor when right mouse button released
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        if (IsCameraRotationAllowed()) {  // Rotation
            var mouseMovement = GetInputLookRotation() * (MouseSensitivityMultiplier * mouseRotateSensitivity);
            if (invertY)
                mouseMovement.y = -mouseMovement.y;

            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            if (_worldTransform) {
                _worldTransform.Rotate(Vector3.down, mouseMovement.x * mouseSensitivityFactor, Space.Self);
                //_worldTransform.Rotate(Vector3.right * (Mathf.Sqrt(2)/2f), mouseMovement.y * mouseSensitivityFactor, Space.Self);
            } else {
                _targetCameraState.Yaw += mouseMovement.x * mouseSensitivityFactor;
                _targetCameraState.Pitch += mouseMovement.y * mouseSensitivityFactor;
            }
        }
        
        var translation = GetInputTranslationDirection() * Time.deltaTime;
        translation *= (IsBoostPressed()) ? 40f : 8f;
        
        _targetCameraState.Translate(translation);
        _targetCameraState.AddZoom(GetZoom(), minZoomIn, maxZoomIn);
        
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        _interpolatingCameraState.LerpTowards(_targetCameraState, positionLerpPct, rotationLerpPct);

        _interpolatingCameraState.UpdateTransform(_transform);
        _interpolatingCameraState.UpdateZoom(_camera);
    }

    private float GetZoom() {
        #if ENABLE_INPUT_SYSTEM
            return zoomAction.ReadValue<Vector2>().y * -mouseZoomSensitivity;
        #else
            //return Input.mouseScrollDelta.y * -mouseZoomSensitivity;
            return Input.mouseScrollDelta.y * -mouseZoomSensitivity;
        #endif
    }

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

    private bool IsCameraRotationAllowed() {
        #if ENABLE_INPUT_SYSTEM
            bool canRotate = Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
            canRotate |= Gamepad.current != null ? Gamepad.current.rightStick.ReadValue().magnitude > 0 : false;
            return canRotate;
        #else
            return Input.GetMouseButton(1);
        #endif
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
