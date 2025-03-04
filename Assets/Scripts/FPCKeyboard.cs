using UnityEngine;
using UnityEngine.AI;

public class FPCKeyboard : MonoBehaviour
{
    public Camera playerCamera;
    public float moveSpeed = 3.5f;
    public float sprintSpeed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = 9.8f;
    public float rotationSpeed = 2f;

    private NavMeshAgent agent;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isJumping = false;
    private float verticalVelocity = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.autoTraverseOffMeshLink = false; // Disable automatic jumping over small gaps
        agent.updatePosition = false; // Prevent conflicts between NavMeshAgent and Transform
        agent.updateRotation = false; // We handle rotation manually

        characterController = GetComponent<CharacterController>();

        LockCursor(true);
    }

    private void Update()
    {
        HandleMouseLook();
        HandleKeyboardMovement();
        HandleJumping();
    }

    private void HandleKeyboardMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        agent.speed = currentSpeed;

        moveDirection = transform.right * moveX + transform.forward * moveZ;

        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 targetPosition = transform.position + moveDirection * currentSpeed * Time.deltaTime;
            agent.SetDestination(targetPosition); // Move using NavMeshAgent destination
        }
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                isJumping = true;
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 jumpMovement = new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        characterController.Move(jumpMovement);

        if (characterController.isGrounded)
        {
            isJumping = false;
            verticalVelocity = 0f;
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.Rotate(Vector3.left * mouseY);
    }

    private void LockCursor(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}
