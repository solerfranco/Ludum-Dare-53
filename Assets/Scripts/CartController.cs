using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
    private Transform _kartModel, _cargoModel;
    [SerializeField]
    private LayerMask _roadLayerMask;
    [SerializeField]
    private GameObject _wheelPivot;

    [SerializeField]
    private ParticleSystem _leftWheelPS, _rightWheelPS;

    [SerializeField]
    private List<Rigidbody> _cargo;

    [SerializeField]
    private Speedometer _speedometer;

    [SerializeField]
    private GameObject _winPanel, _tutorialPanel;

    [SerializeField]
    private TextMeshProUGUI _timeText;

    private float _timeElapsed;

    private float _speed, _currentSpeed;
    private float _rotation, _currentRotation;
    private float _stuckAcceleration;
    private Vector2 _movementInput;
    private PlayerInputActions _playerInputActions;
    private bool _checkRoad, _won;

    [SerializeField]
    private Animator _anim;


    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _stuckAcceleration = _acceleration * 0.4f;
    }

    private void Start()
    {
        _playerInputActions.Player.Jump.performed += Tutorial;
        _playerInputActions.Player.Jump.performed += Drift;
        _tutorialPanel.SetActive(true);
    }

    private void Drift(InputAction.CallbackContext context)
    {
        _anim.SetTrigger("Drifting");
        _anim.SetFloat("Drift", -1);
    }

    private void Tutorial(InputAction.CallbackContext obj)
    {
        if (!_tutorialPanel.activeSelf) return;
        _tutorialPanel.SetActive(false);
        _playerInputActions.Enable();
        _playerInputActions.Player.Jump.performed -= Tutorial;
    }

    private void DropCargo()
    {
        if (_cargo.Count <= 0) return;
        _cargo[0].isKinematic = false;
        _cargo[0].transform.SetParent(null);
        _cargo[0].AddForce(-_kartModel.forward * Mathf.Clamp(_sphere.velocity.sqrMagnitude, 0, 90) * 0.1f + Vector3.up * 3, ForceMode.Impulse);
        _cargo.RemoveAt(0);
    }

    void Update()
    {
        if (_tutorialPanel.activeSelf) return;
        if (!_won)
        {
            _timeElapsed += Time.deltaTime;
        }
        else
        {
            _timeText.text = "Delivery time: " + _timeElapsed.ToString("00.00") + "s <br> Packages delivered: " + _cargo.Count.ToString();
        }

        transform.position = _sphere.transform.position - Vector3.up * 0.5f;
        _movementInput = _playerInputActions.Player.Movement.ReadValue<Vector2>();

        _speedometer.Speed = _sphere.velocity.sqrMagnitude;

        _speed = -_movementInput.y * (_checkRoad ? _acceleration : _stuckAcceleration);
        print(_sphere.velocity.sqrMagnitude);

        var leftEmission = _leftWheelPS.emission;
        leftEmission.enabled = _sphere.velocity.sqrMagnitude > 110;
        var rightEmission = _rightWheelPS.emission;
        rightEmission.enabled = _sphere.velocity.sqrMagnitude > 110;

        _wheelPivot.transform.Rotate(Time.deltaTime * (_sphere.velocity.x + _sphere.velocity.z) * 0.5f * 200, 0, 0);

        if (_movementInput.x != 0) Steer(_movementInput.x > 0 ? 1 : -1, Mathf.Abs(_movementInput.x));

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_kartModel.position + Vector3.up, _kartModel.position + Vector3.up + Vector3.down * 1.3f);
    }

    private void FixedUpdate()
    {
        _sphere.AddForce(_kartModel.transform.forward * _currentSpeed, ForceMode.Acceleration);

        _sphere.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + _currentRotation, 0), Time.deltaTime * 5f);

        //Check ground
        Physics.Raycast(_kartModel.position + Vector3.up, Vector3.down, out RaycastHit checkRoad, 1.3f, _roadLayerMask);
        _checkRoad = checkRoad.collider != null;

        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitOn, 1.1f);
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitNear, 2.0f);

        _kartModel.parent.up = Vector3.Lerp(_kartModel.parent.up, hitNear.normal, Time.deltaTime * 8.0f);
        _kartModel.parent.Rotate(0, transform.eulerAngles.y, 0);


        _currentSpeed = Mathf.SmoothStep(_currentSpeed, _speed, Time.deltaTime * 12f); _speed = 0;
        _currentRotation = Mathf.Lerp(_currentRotation, _rotation, Time.deltaTime * 4f); _rotation = 0;
    }

    private void Steer(int direction, float amount)
    {
        _rotation = (_steering * direction) * amount;
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Jump.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private IEnumerator Goal(Vector3 goalPos)
    {
        _won = true;
        _playerInputActions.Disable();
        _currentSpeed = 0;
        
        WaitForSeconds delayMove = new WaitForSeconds(0.25f);

        foreach (Rigidbody box in _cargo)
        {
            LeanTween.moveLocalY(box.gameObject, 1, 0.25f).setEaseOutCirc().setOnComplete(() =>
            {
                LeanTween.move(box.gameObject, goalPos, 0.5f).setOnComplete(() =>
                {
                    Destroy(box.gameObject);
                });
            });
            yield return delayMove;
        }
        _winPanel.SetActive(true);
    }

    public void TriggerEntered(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            DropCargo();
            return;
        }
        if (other.CompareTag("Goal"))
        {
            StartCoroutine(Goal(other.transform.parent.position));
            return;
        }
    }
}
