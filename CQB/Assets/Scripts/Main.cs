using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public LayerMask Interactables;
    public InteractableObject interactableObject;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI secondaryInteractionText;

    [Header("Ammo")]
    public int M1911Ammo = 21; 
    public int M4A1Ammo = 60;
    public int FNSCARAmmo = 60;
    public int HK416Ammo = 60;
    public int SIGMPXAmmo = 60;
    public int M906Ammo;

    [Header("Shake")]
    public Transform mainCameraTransform;
    public Transform secondaryCameraTransform;
    public float gunShakeIntensity = 2f;
    public float shakeDuration = 0.5f;
    private Vector3 originalMainCameraPosition;
    private Quaternion originalMainCameraRotation;
    private Vector3 originalSecondaryCameraPosition;
    private Quaternion originalSecondaryCameraRotation;
    private Coroutine currentShakeRoutine;


    [Header("Guns")]
    public Firearm firearm;
    public int weaponIndexToUnlock;

    void Start()
    {
        originalMainCameraPosition = mainCameraTransform.localPosition;
        originalMainCameraRotation = mainCameraTransform.localRotation;
        originalSecondaryCameraPosition = secondaryCameraTransform.localPosition;
        originalSecondaryCameraRotation = secondaryCameraTransform.localRotation;

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

            if (Physics.Raycast(ray, out hitinfo, interactionRange, Interactables))
            {
                InteractableObject interactableObject = hitinfo.collider.GetComponent<InteractableObject>();
                if (interactableObject != null)
                {
                    interactableObject.Interact();
                    interactionText.gameObject.SetActive(false);
                }
            }
            else
            {
                interactionText.gameObject.SetActive(false);
            }
        }
        else
        {
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hitinfo;

            if (Physics.Raycast(ray, out hitinfo, interactionRange, Interactables))
            {
                InteractableObject interactableObject = hitinfo.collider.GetComponent<InteractableObject>();
                if (interactableObject != null)
                {
                    interactionText.gameObject.SetActive(true);
                    interactionText.text = "F";
                    secondaryInteractionText.gameObject.SetActive(true); 
                    secondaryInteractionText.text = "Use: " + hitinfo.collider.name;
                }
            }
            else
            {
                interactionText.gameObject.SetActive(false);
                secondaryInteractionText.gameObject.SetActive(false);
            }
        }


    }

    public int GetCurrentAmmo(int weaponID)
    {
        return weaponID switch
        {
            0 => M1911Ammo,      // M1911
            1 => M4A1Ammo,      // M4
            2 => FNSCARAmmo,     // FN SCAR
            3 => HK416Ammo,      // HK416
            4 => SIGMPXAmmo,     // SIG MPX
            7 => M906Ammo,       // M-906
            _ => 0
        };
    }

    public void DecreaseAmmo(int weaponID, int amount)
    {
        switch (weaponID)
        {
            case 0:
                M1911Ammo = Mathf.Max(0, M1911Ammo - amount);
                break;
            case 1:
                M4A1Ammo = Mathf.Max(0, M4A1Ammo - amount);
                break;
            case 2:
                FNSCARAmmo = Mathf.Max(0, FNSCARAmmo - amount);
                break;
        }
    }

    public void TriggerShake(Transform cameraTransform, float intensity, float duration)
    {
        if (currentShakeRoutine != null) StopCoroutine(currentShakeRoutine);
        currentShakeRoutine = StartCoroutine(Shake(cameraTransform, intensity, duration));
    }

    private IEnumerator Shake(Transform cameraTransform, float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            cameraTransform.localPosition = (cameraTransform == mainCameraTransform) ?
                originalMainCameraPosition + randomOffset :
                originalSecondaryCameraPosition + randomOffset;
            yield return null;
        }

        ResetShake(cameraTransform);
    }

    private void ResetShake(Transform cameraTransform)
    {
        if (cameraTransform == mainCameraTransform)
        {
            cameraTransform.localPosition = originalMainCameraPosition;
            cameraTransform.localRotation = originalMainCameraRotation;
        }
        else if (cameraTransform == secondaryCameraTransform)
        {
            cameraTransform.localPosition = originalSecondaryCameraPosition;
            cameraTransform.localRotation = originalSecondaryCameraRotation;
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

