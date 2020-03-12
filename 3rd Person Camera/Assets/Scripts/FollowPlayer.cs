using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour
{
    [Header("Camera follow target adjustment")]
    [SerializeField] private Transform _target = default;
    [SerializeField] private float _distanceToTarget = 5f;
    [SerializeField, Range(0.5f, 25.0f)] private float _smoothSpeed = 6.0f;

    [Header("Mouse rotate camera view")]
    [SerializeField, Range(10.0f, 70.0f)] private float _mouseSensitivity = 45.0f;
    [SerializeField, Range(-89f, 89f)] private float _minVerticalAngle = -10.0f, _maxVerticalAngle = 40.0f;

    [Header("Camera zoom")]
    [SerializeField] private float _zoomSpeed = 80.0f;
    [SerializeField] private float _maxZoom = 15.0f;
    [SerializeField] private float _minZoom = 2.0f;

    [Header("Camera view gets blocked")]
    [SerializeField] private LayerMask _collisionMask = default;
    [SerializeField, Min(0f)] float _minDistance = 1.8f;
    
    private Vector3 _cameraRotation = Vector3.zero;
    private Vector3 _lookDirection = Vector3.zero;
    private float _zoomInput = 0.0f;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private float _adjustedDistance;
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
       Vector3 lookPosition = _target.position - _lookDirection * _adjustedDistance;
       transform.position = Vector3.Slerp(transform.position, lookPosition, _smoothSpeed * Time.unscaledDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_target.position, _minDistance);
    }

    private void MouseRotation()
    {
        _cameraRotation.y += Input.GetAxis(_rotateHorizontally) * _mouseSensitivity * Time.deltaTime;
        _cameraRotation.x -= Input.GetAxis(_rotateVertically) * _mouseSensitivity * Time.deltaTime;
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
        float camDistance = _distanceToTarget;
        _ray.origin = _target.position;
        _ray.direction = - _lookDirection;

        Debug.DrawLine(_ray.origin, _ray.direction, Color.red);

        if (Physics.Raycast(_ray, out _raycastHit, camDistance, _collisionMask))
        {
            if (!_isCameraAdjusted)
            {
                if (Vector3.Distance(_ray.origin, _raycastHit.point) < _minDistance)
                {
                    _adjustedDistance = _minDistance;
                    _isCameraAdjusted = true;
                }
            }
        }
        else
        {
            _isCameraAdjusted = false;
            _adjustedDistance = _distanceToTarget;
        }
    }
}
