using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterInput : MonoBehaviour
{
    public CharacterController controller;
    private Vector2 movementInput;
    private PlayerInputActions playerInputActions;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private LayerMask interactableLM;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions.Player.Interact.performed += CheckInteractable;
    }

    private void GetMovementInput()
    {
        movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        GetMovementInput();
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        movement = Vector3.ClampMagnitude(movement, 1f);
        movement *= Time.deltaTime * movementSpeed;
        movement = transform.TransformDirection(movement);
        if (controller.isGrounded == false)
        {
            movement.y = -0.05f;
        }
        controller.Move(movement);


        float HorizontalSensitivity = 30.0f;
        float VerticalSensitivity = 30.0f;

        float RotationX = HorizontalSensitivity * playerInputActions.Player.RotationX.ReadValue<float>() * Time.deltaTime;
        float RotationY = VerticalSensitivity * playerInputActions.Player.RotationY.ReadValue<float>() * Time.deltaTime;
        print(Camera.main.transform.eulerAngles);
        Camera.main.transform.Rotate(RotationY, RotationX, 0f, Space.World);
        Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);
    }

    private void CheckInteractable(InputAction.CallbackContext ctx)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        print("Checking interactable");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLM))
        {
            if(hit.transform.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
            }
        }
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
