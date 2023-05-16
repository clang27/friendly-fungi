using UnityEngine;

public class EditorCameraController : MonoBehaviour {
    private Vector3 _goalRotation;
    private Transform _worldTransform;
    private Camera _camera;
    
    public bool AutoRotate { get; set; }
    public static bool Rotating { get; private set; }

    [Tooltip("Time it takes to interpolate world rotation 99% of the way to the target.")] [Range(0.001f, 1f)]
    public float worldRotationLerpTime = 0.001f;

    public float rotateSpeed = 2f;
    public float scrollSpeed = 0.1f;
    public float zoomSpeed = 0.8f;

    private void Start() {
        _worldTransform = GameObject.FindGameObjectWithTag("Map").transform;
        _goalRotation = _worldTransform.localRotation.eulerAngles;
        _camera = GetComponent<Camera>();
    }

    private Vector3 GetInputRotationDirection() {
        var direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.LeftArrow))
            direction += Vector3.down;
        if (Input.GetKey(KeyCode.RightArrow))
            direction += Vector3.up;
        if (Input.GetKey(KeyCode.UpArrow))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.DownArrow))
            direction += Vector3.right;

        return direction;
    }
    
    private Vector3 GetInputScrollDirection() {
        var direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;
        if (Input.GetKey(KeyCode.S))
            direction += Vector3.down; 
        if (Input.GetKey(KeyCode.W))
            direction += Vector3.up;

        return direction;
    }

    private void Update() {
        _camera.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        _camera.transform.position += GetInputScrollDirection() * scrollSpeed;
        
        var worldRotationLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / worldRotationLerpTime * Time.deltaTime);

        if (_worldTransform) {
            if (AutoRotate) {
                _goalRotation += Vector3.up * (rotateSpeed * 0.2f);
            } else {
                _goalRotation += GetInputRotationDirection() * rotateSpeed;
            }
            
            _worldTransform.localRotation = Quaternion.Lerp(_worldTransform.localRotation, Quaternion.Euler(_goalRotation), worldRotationLerpPct);
        }
        
        Rotating = _worldTransform.localRotation != Quaternion.Euler(_goalRotation);
    }
    
}