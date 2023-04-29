using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    private Vector2 _movementInput;
    private PlayerInputActions _playerInputActions;
    private Rigidbody _rb;

    [SerializeField]
    private float _acceleration;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _torque;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _rb = GetComponent<Rigidbody>();
    }

    private void GetMovementInput()
    {
        _movementInput = _playerInputActions.Player.Movement.ReadValue<Vector2>();
    }

    void Update()
    {
        GetMovementInput();
        Vector3 movement = (Vector3.forward * -_movementInput.y).normalized * _acceleration;
        _rb.AddRelativeTorque(transform.up * _torque * _movementInput.x);
        _rb.AddRelativeForce(movement, ForceMode.Force);
        _rb.maxAngularVelocity = 3f;
        //_rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, Mathf.Clamp(_rb.velocity.z, -_maxSpeed, _maxSpeed));
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
}
