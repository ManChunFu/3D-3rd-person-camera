using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6.0f;
    [SerializeField] private float _jumpHeight = 10.0f;
    [SerializeField] private float _gravity = 20.0f;
    [SerializeField] private float _rotationSpeed = 240.0f;

    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController _controller;
    private bool _isJumping = false;

    #region Input IDs
    private const string _turnAngle = "Horizontal";
    private const string _move = "Vertical";
    #endregion Input IDs
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis(_turnAngle);
        float verticalInput = Input.GetAxis(_move);

        transform.Rotate(0, horizontalInput * _rotationSpeed * Time.deltaTime, 0);

        if (_controller.isGrounded)
        {
            _isJumping = false;
            _moveDirection = Vector3.forward * verticalInput;
            _moveDirection = transform.TransformDirection(_moveDirection);
            _moveDirection *= _moveSpeed;

            if (Input.GetKey(KeyCode.Space))
            {
                _moveDirection.y += _jumpHeight;
                _isJumping = true;
            }
        }

        _moveDirection.y -= _gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}
