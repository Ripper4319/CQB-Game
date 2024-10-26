using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public PlayerMovement player;
    public Rigidbody playerRigidbody;
    public NavMeshAgent agent;
    public Transform head;

    [Header("Enemy Stats")]
    public float health = 3;
    public int maxHealth = 5;
    public int damageGiven = 1;
    public int damageReceived = 1;
    public float pushBackForce = 10000;
    public bool hit = false;
    public Transform player1;
    public float detectionRange = 10f;
    public float bulletsfired;

    [Header("Enemy damage")]
    [SerializeField] private float HeadMultiplier = 2;
    [SerializeField] private float TorsoMultiplier = 1;
    [SerializeField] private float LeftArmMultiplier = 0.5f;
    [SerializeField] private float RightArmMultiplier = 0.5f;
    [SerializeField] private float LeftLegMultiplier = 0.75f;
    [SerializeField] private float RightLegMultiplier = 0.75f;
    [SerializeField] private float BellyMultiplier = 1;
    [SerializeField] private BodyPart[] bodyPartMapping;

    [Header("Enemy ragdoll")]
    [SerializeField] private Rigidbody[] rb;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Animator character;

    [Header("Player Last Seen")]
    public GameObject playerLastSeen;
    private float playerLastSeenTimer = 0f; // Timer for player position
    private float stationaryDuration = 0.5f; // Duration to wait before firing

    [Header("Weapon Stats")]
    public GameObject shot;
    public Transform gunTransform;
    public GameObject muzzleFlashPrefab;
    public float shotVel = 10f;
    public int bulletsToFire = 1;
    public float fireRate = 1.5f;
    public Transform FirePoint;
    public GameObject Firepoint;
    public float bulletLifespan;

    [Header("Field of View")]
    public float normalFOV = 50f;
    public float engageFOV = 270f;

    [Header("Ammo Stats")]
    public int maxAmmo = 300;
    public int clipSize = 30;
    private int currentClip;
    private bool isReloading = false;
    private bool isFiring = false;

    public enum BodyPart
    {
        Head,
        Torso,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
        Belly
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        agent = GetComponent<NavMeshAgent>();
        character.enabled = true;

        currentClip = clipSize;

        isReloading = false;

        foreach (Rigidbody rb in rb)
        {
            rb.isKinematic = true;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player1.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player1.position - head.position).normalized;
            float angleToPlayer = Vector3.Angle(head.forward, directionToPlayer);

            float currentFOV = character.GetBool("isEngaged") ? engageFOV : normalFOV;

            if (angleToPlayer < currentFOV)
            {
                if (Physics.Raycast(head.position, directionToPlayer, out RaycastHit hit, detectionRange))
                {
                    Debug.DrawRay(head.position, directionToPlayer * hit.distance, Color.red);

                    if (hit.collider.CompareTag("Player"))
                    {
                        playerLastSeen.transform.position = hit.point;
                        playerLastSeen.GetComponent<Rigidbody>().velocity = playerRigidbody.velocity;

                        // Reset the timer
                        playerLastSeenTimer = 0f;
                    }
                }
                else
                {
                    Debug.DrawRay(head.position, directionToPlayer * detectionRange, Color.green);
                }
            }
        }
        else
        {
            character.SetBool("isWalking", false);
        }

        // Update the timer if the player has been seen
        if (playerLastSeen.transform.position != Vector3.zero) // Make sure playerLastSeen has been set
        {
            playerLastSeenTimer += Time.deltaTime;

            // If the timer exceeds the stationary duration, engage the player
            if (playerLastSeenTimer >= stationaryDuration)
            {
                if (!isFiring) // Ensure not already firing
                {
                    StartCoroutine(EngagePlayer(playerLastSeen.transform.position));
                }
            }
        }

        if (health <= 0)
            Destroy(gameObject);
    }

    private IEnumerator FireAtPlayerLastSeen()
    {
        isFiring = true;
        bulletsfired = 0;

        while (currentClip > 0 && bulletsfired < 30)
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }

        if (currentClip == 0 && maxAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        isFiring = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("shot"))
        {
            Collider hitCollider = collision.contacts[0].thisCollider;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == hitCollider)
                {
                    TakeDamage(collision.relativeVelocity.magnitude, bodyPartMapping[i]);
                    break;
                }
            }
        }
    }

    public void Fire()
    {
        if (Time.timeScale != 1 || isReloading) return;

        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, gunTransform.position, gunTransform.rotation);
        GameObject bulletInstantiator = Firepoint;
        GameObject projectile = Instantiate(shot, bulletInstantiator.transform.position, bulletInstantiator.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(FirePoint.transform.forward * shotVel, ForceMode.Impulse);
        }

        Destroy(projectile, bulletLifespan);
        Destroy(muzzleFlash, 0.1f);

        bulletsfired++;
        currentClip--;
    }

    public void TakeDamage(float damage, BodyPart bodypart)
    {
        float actualDamage = damage;

        switch (bodypart)
        {
            case BodyPart.Head:
                actualDamage *= HeadMultiplier;
                break;
            case BodyPart.Torso:
                actualDamage *= TorsoMultiplier;
                break;
            case BodyPart.LeftArm:
                actualDamage *= LeftArmMultiplier;
                break;
            case BodyPart.RightArm:
                actualDamage *= RightArmMultiplier;
                break;
            case BodyPart.LeftLeg:
                actualDamage *= LeftLegMultiplier;
                break;
            case BodyPart.RightLeg:
                actualDamage *= RightLegMultiplier;
                break;
            case BodyPart.Belly:
                actualDamage *= BellyMultiplier;
                break;
        }

        health -= actualDamage;
        if (health <= 0F)
        {
            Die();
        }
    }

    private Coroutine fireCoroutine;

    private IEnumerator EngagePlayer(Vector3 targetPosition)
    {
        Vector3 lookDirection = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Stop any existing firing coroutine
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }

        // Start the firing coroutine
        fireCoroutine = StartCoroutine(FireAtPlayerLastSeen());

        yield return new WaitForSeconds(4f);

        if (!isReloading)
        {
            agent.destination = playerLastSeen.transform.position;
            character.SetBool("isWalking", true);
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        character.SetBool("isreloading", true);

        yield return new WaitForSeconds(.1f);

        int reloadCount = clipSize - currentClip;

        if (maxAmmo < reloadCount)
        {
            currentClip += maxAmmo;
            maxAmmo = 0;
        }
        else
        {
            currentClip += reloadCount;
            maxAmmo -= reloadCount;
        }

        yield return new WaitForSeconds(1.9f);

        isReloading = false;
        character.SetBool("isReloading", false);

        bulletsfired = 0;
    }

    private void Die()
    {
        character.enabled = false;

        foreach (Rigidbody rb in rb)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }
}


