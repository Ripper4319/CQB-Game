using UnityEngine;
using System.Collections;

public class AttackState : IEnemyState
{
    // -----------------------------------------------
    // UNMADE VARIABLES AREA
    // -----------------------------------------------

    // Prefabs
    public GameObject muzzleFlashPrefab;  // Prefab for the muzzle flash effect
    public GameObject shot;              // Prefab for the projectile or shot fired
    public Transform gunTransform;       // The transform of the gun or weapon firing the shot

    // Firing parameters
    public float shotVel = 10f;          // Shot velocity (speed of the fired projectile)
    private int currentClip = 30;        // Current number of shots in the clip (example: 30 rounds in the magazine)
    private int clipSize = 30;           // Max size of the clip
    private bool isReloading = false;    // Whether the gun is currently reloading

    // Timing
    private float attackCooldown = 1f;   // The cooldown time between attacks
    private float lastAttackTime = 0f;   // The time when the last attack happened (used for cooldown)

    // -----------------------------------------------
    // METHODS
    // -----------------------------------------------

    public void Enter(Enemy enemy)
    {
        // Reset or initialize any necessary values when entering the Attack state.
        lastAttackTime = Time.time; // Ensures the attack starts with the correct cooldown
    }

    public void Execute(Enemy enemy)
    {
        if (enemy.player == null) return;

        // Check if enough time has passed to attack (Cooldown)
        if (Time.time - lastAttackTime > attackCooldown)
        {
            if (currentClip > 0)
            {
                if (currentClip > 0)
                {
                    Fire(enemy); // Pass the enemy reference
                }
                // Fire a shot
            }
            else
            {
                // Reload if out of ammo
                if (!isReloading)
                {
                    // Start reloading if not already reloading
                    enemy.StartCoroutine(Reload());
                }
            }

            lastAttackTime = Time.time; // Update last attack time to the current time
        }
    }

    private void Fire(Enemy enemy)
    {
        if (isReloading) return;

        if (muzzleFlashPrefab == null || shot == null || gunTransform == null)
        {
            Debug.LogError("References are missing!");
            return;
        }

        // Use the enemy object to handle instantiation and destruction
        GameObject muzzleFlash = Object.Instantiate(muzzleFlashPrefab, gunTransform.position, gunTransform.rotation);
        GameObject projectile = Object.Instantiate(shot, gunTransform.position, gunTransform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(gunTransform.forward * shotVel, ForceMode.Impulse);
        }

        Object.Destroy(projectile, 5f);
        Object.Destroy(muzzleFlash, 0.1f);

        currentClip--; // Decrease ammo count
    }



    public IEnumerator Reload()
    {
        isReloading = true; // Mark as reloading

        // Simulate reload time (2 seconds for this example)
        yield return new WaitForSeconds(2f);

        // Reload the clip
        currentClip = clipSize; // Set the clip back to full size

        isReloading = false; // Mark as done reloading
    }

    private void Attack(Enemy enemy)
    {
        // The core attack logic goes here.
        Debug.Log("Attacking Player!");

        // Example: Apply damage to the player (you can define damage logic here)
        // player.GetComponent<PlayerHealth>().TakeDamage(10);
    }

    public void Exit(Enemy enemy)
    {
        // Cleanup or reset values when exiting the AttackState
    }
}


