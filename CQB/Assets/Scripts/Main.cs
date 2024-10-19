using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 2f;
    public float CrouchSpeed = 1.1f;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    public Camera camera;

    public float crouchHeight = 0.5f;
    private float originalHeight;
    private bool isCrouching = false;
    private Vector3 targetPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        Cursor.lockState = CursorLockMode.Locked;
        targetPosition = transform.localPosition;
    }

    void Update()
    {
        if (controller == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(0, mouseX, 0);
        camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            velocity.y = jumpForce;
        }

        if (Input.GetKeyDown(KeyCode.C) && !isCrouching)
        {
            isCrouching = true;
            controller.height = crouchHeight;
            targetPosition = new Vector3(transform.localPosition.x, crouchHeight / 2, transform.localPosition.z);
        }
        else if (Input.GetKeyUp(KeyCode.C) && isCrouching)
        {
            isCrouching = false;
            controller.height = originalHeight;
            targetPosition = new Vector3(transform.localPosition.x, originalHeight / 2, transform.localPosition.z);
        }

        transform.localPosition = Vector3.Slerp(transform.localPosition, targetPosition, Time.deltaTime * CrouchSpeed);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }



}

