using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _sphere;
    [SerializeField]
    private float _acceleration = 30f;
    [SerializeField]
    private float _steering = 80f;
    [SerializeField]
    private float _gravity = 10f;
    [SerializeField]
    private Transform _kartModel;
    [SerializeField]
    private LayerMask _roadLayerMask;
    [SerializeField]
    private GameObject _wheelPivot;

    private float _speed, _currentSpeed;
    private float _rotation, _currentRotation;
    private float _stuckAcceleration;
    private Vector2 _movementInput;
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _stuckAcceleration = _acceleration * 0.2f;
    }

    void Update()
    {
        transform.position = _sphere.transform.position - Vector3.up * 0.5f;
        _movementInput = _playerInputActions.Player.Movement.ReadValue<Vector2>();


        Physics.Raycast(_kartModel.position + Vector3.up, Vector3.down, out RaycastHit checkRoad, 2f, _roadLayerMask);

        if (_movementInput.y != 0) _speed = -_movementInput.y * (checkRoad.collider ? _acceleration : _stuckAcceleration);
        _wheelPivot.transform.Rotate(Time.deltaTime * (_sphere.velocity.x + _sphere.velocity.z) * 0.5f * 200, 0, 0);

        if (_movementInput.x != 0) Steer(_movementInput.x > 0 ? 1 : -1, Mathf.Abs(_movementInput.x));

        _currentSpeed = Mathf.SmoothStep(_currentSpeed, _speed, Time.deltaTime * 12f); _speed = 0;
        _currentRotation = Mathf.Lerp(_currentRotation, _rotation, Time.deltaTime * 4f); _rotation = 0;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_kartModel.position + Vector3.up, _kartModel.position + Vector3.down * 2);
    }

    private void FixedUpdate()
    {
        _sphere.AddForce(_kartModel.transform.forward * _currentSpeed, ForceMode.Acceleration);

        _sphere.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + _currentRotation, 0), Time.deltaTime * 5f);


        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitOn, 1.1f);
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitNear, 2.0f);

        _kartModel.parent.up = Vector3.Lerp(_kartModel.parent.up, hitNear.normal, Time.deltaTime * 8.0f);
        _kartModel.parent.Rotate(0, transform.eulerAngles.y, 0);
    }

    private void Steer(int direction, float amount)
    {
        _rotation = (_steering * direction) * amount;
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
