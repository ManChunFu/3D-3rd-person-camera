using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour
{
    [Header("Camera follow target adjustment")]
    [SerializeField] private Transform _target = default;
    [Tooltip("Camera's height : affect its position.y value")]
    [SerializeField, Range(0.0f, 10.0f)] private float _attitude = 0.7f;
    [Tooltip("Affecting camera's rotaiton.x value")]
    [SerializeField, Range(0.0f, 20.0f)] private float _viewAngle = 8.0f;
    [SerializeField] private float _distanceToTarget = 4.2f;
    [SerializeField, Range(0.5f, 25.0f)] private float _smoothSpeed = 3.5f;

    [Header("Mouse rotate camera view")]
    [SerializeField, Range(10.0f, 70.0f)] private float _mouseSensitivity = 45.0f;
    [Tooltip("Prevent vertical rotation go around the target, max value can not be smaller than min value")]
    [SerializeField, Range(-89f, 89f)] private float _minVerticalAngle = -10.0f, _maxVerticalAngle = 40.0f;

    [Header("Camera zoom")]
    [SerializeField] private float _zoomSpeed = 90.0f;
    [SerializeField] private float _maxZoom = 15.0f;
    [SerializeField] private float _minZoom = 2.0f;

    [Header("Camera view gets blocked")]
    [Tooltip("Setup the layer that camera should react with")]
    [SerializeField] private LayerMask _collisionMask = default;
    [Tooltip("The distance betwen the target and the obstacle")]
    [SerializeField, Range(1.0f, 2.0f)] float _zoomIn = 1.8f;
    [SerializeField, Range(1.0f, 10.0f)] private float _smoothZoomToTarget = 6.0f;

    private Vector3 _cameraRotation = Vector3.zero;
    private Vector3 _lookDirection = Vector3.zero;
    private float _zoomInput = 0.0f;
    private float _adjustedDistance = 0;
    private bool _isCameraAdjusted = false;

    #region Input IDs
    private const string _rotateHorizontally = "Mouse X";
    private const string _rotateVertically = "Mouse Y";
    private const string _zoom = "Mouse ScrollWheel";
    #endregion Input IDs


    private void OnValidate()
    {
        if (_maxVerticalAngle < _minVerticalAngle)
        { _maxVerticalAngle = _minVerticalAngle; }
    }

    private void Awake()
    {
        Assert.IsNotNull(_target, "Did you forget to assign the target?");
        _cameraRotation = transform.localRotation.eulerAngles;
        _cameraRotation.x = _viewAngle;
    }


    private void Update()
    {
        CameraZoom();
    }

    private void LateUpdate()
    {
        MouseRotation();
        _lookDirection = Quaternion.Euler(_cameraRotation) * Vector3.forward;
        CameraCollision();
        Vector3 lookPosition = _target.position + (_isCameraAdjusted ? Vector3.zero : new Vector3(0.0f, _attitude, 0.0f)) - _lookDirection * _adjustedDistance;
        transform.position = Vector3.Slerp(transform.position, lookPosition, (_isCameraAdjusted ? _smoothZoomToTarget : _smoothSpeed) * Time.unscaledDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_target.position, _zoomIn);
    }

    private void MouseRotation()
    {
        _cameraRotation.y += Input.GetAxis(_rotateHorizontally) * _mouseSensitivity * Time.deltaTime;
        _cameraRotation.x += -Input.GetAxis(_rotateVertically) * _mouseSensitivity * Time.deltaTime;
        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, _minVerticalAngle, _maxVerticalAngle);

        transform.eulerAngles = _cameraRotation;
    }

    private void CameraZoom()
    {
        _zoomInput = Input.GetAxis(_zoom);
        _distanceToTarget -= _zoomInput * _zoomSpeed * Time.deltaTime;
        _distanceToTarget = Mathf.Clamp(_distanceToTarget, _minZoom, _maxZoom);
    }

    private void CameraCollision()
    {
        Ray ray = new Ray();
        ray.origin = _target.position;
        ray.direction = -_lookDirection;
        Debug.DrawLine(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, _distanceToTarget, _collisionMask))
        {
            if (!_isCameraAdjusted)
            {
                _adjustedDistance = _zoomIn;
                _isCameraAdjusted = true;
            }
        }
        else
        {
            _isCameraAdjusted = false;
            _adjustedDistance = _distanceToTarget;
        }
    }
}