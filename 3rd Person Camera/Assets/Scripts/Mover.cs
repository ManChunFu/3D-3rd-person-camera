using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Mover : MonoBehaviour
{
    [SerializeField] float speed = 20f;

    private Rigidbody rigidBD = default;

    #region Input IDs
    private const string moveHorizontally = "Horizontal";
    private const string moveVertically = "Vertical";
    #endregion
    private void Awake()
    {
        rigidBD = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis(moveHorizontally), 0.0f, Input.GetAxis(moveVertically));
        rigidBD.AddForce(movement * speed);
    }





}
