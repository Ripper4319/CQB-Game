using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Speed")]
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 2f;
    public float CrouchSpeed = 1.1f;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    public Camera camera;
    public Transform Camera;


    [Header("Aiming")]
    public bool isAiming = false;
    public float normalFOV = 60f;
    public float zoomFOV = 30f;
    public Transform gunTransform;
    public Vector3 gunADSPosition;
    public Vector3 gunNormalPosition;

    [Header("Crouching")]
    public float crouchHeight = 0.5f;
    private float originalHeight;
    private bool isCrouching = false;
    private Vector3 targetPosition;


    [Header("leaning")]
    public float leanAmount = 100f;
    public float leanAmount1 = 100f;
    public float leanSpeed = 5f;
    private float targetLean = 30f;
    private float targetLean1 = .5f;
    private float currentLean = 0f;
    public Transform cameraParent;

    [Header("Interaction")]
    public float interactionRange = 3f;
    public LayerMask Weapons;
    public InteractableObject interactableObject;

    [Header("Ammo")]
    public int weapon1Ammo; 
    public int weapon2Ammo;
    public int weapon3Ammo;

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

        if (Input.GetMouseButtonDown(1))
        {
            StartADS();
        }

        if (Input.GetMouseButtonUp(1))
        {
            StopADS();
        }


        if (isAiming)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoomFOV, 0.1f);
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, gunADSPosition, 0.1f);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, normalFOV, 0.1f);
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, gunNormalPosition, 0.1f);
        }




        if (Input.GetKey(KeyCode.Q))
        {
            targetLean = leanAmount;
            targetLean1 = -leanAmount1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetLean = -leanAmount;
            targetLean1 = leanAmount1;
        }
        else
        {
            targetLean = 0f;
            targetLean1 = 0f;
        }

        cameraParent.localPosition = new Vector3(targetLean1, cameraParent.localPosition.y, cameraParent.localPosition.z);


        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
        camera.transform.localRotation = Quaternion.Euler(camera.transform.localRotation.eulerAngles.x, camera.transform.localRotation.eulerAngles.y, currentLean);


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

        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hitinfo;


            if (Physics.Raycast(ray, out hitinfo, interactionRange, Weapons))
            {
                InteractableObject interactableObject = hitinfo.collider.GetComponent<InteractableObject>();
                if (interactableObject != null)
                {
                    interactableObject.Interact();
                    hitinfo.collider.transform.SetParent(gunTransform);
                }
                else
                {
                }
            }
            else
            {
            }
        }

    }

    public int GetCurrentAmmo(int weaponID)
    {
        return weaponID switch
        {
            0 => weapon1Ammo,
            1 => weapon2Ammo,
            2 => weapon3Ammo,
            _ => 0 
        };
    }

    public void DecreaseAmmo(int weaponID, int amount)
    {
        switch (weaponID)
        {
            case 0:
                weapon1Ammo = Mathf.Max(0, weapon1Ammo - amount);
                break;
            case 1:
                weapon2Ammo = Mathf.Max(0, weapon2Ammo - amount);
                break;
            case 2:
                weapon3Ammo = Mathf.Max(0, weapon3Ammo - amount);
                break;
        }
    }

    private void StartADS()
    {
        isAiming = true;
    }

    private void StopADS()
    {
        isAiming = false;
    }
    


}

