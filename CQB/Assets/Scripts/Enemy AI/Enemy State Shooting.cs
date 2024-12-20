using UnityEngine;

public class AttackState : IEnemyState
{
    // Prefabs
    private GameObject muzzleFlashPrefab;
    private GameObject shotPrefab;
    private Transform gunTransform;

    // Firing parameters
    public float shotVel = 10f;
    private int currentClip = 30;
    private int clipSize = 30;
    public float shotsFired;

    // Timing
    private float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private float rotationSpeed = 5f;

    public void start()
    {
        shotsFired = 0;
    }

    public AttackState(GameObject muzzleFlash, GameObject shot, Transform gun)
    {
        muzzleFlashPrefab = muzzleFlash;
        shotPrefab = shot;
        gunTransform = gun;
    }

    public void Enter(Enemy enemy)
    {
        lastAttackTime = Time.time; // Initialize cooldown timer
    }

    public void Execute(Enemy enemy)
    {
        if (enemy.playerlastseen == null) return;

        // Rotate towards the player
        Vector3 directionToPlayer = (enemy.playerlastseen.position - enemy.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Check if enough time has passed to attack (cooldown)
        if (Time.time - lastAttackTime > attackCooldown)
        {
            if (currentClip > 0 && shotsFired < 3) // Limit to 3 shots before switching state
            {
                Fire(enemy);
                shotsFired++;
            }
            else if (shotsFired >= 3)
            {
                enemy.SwitchState(new ChaseState()); // Switch to ChaseState after 3 shots
                shotsFired = 0;
            }
            else
            {
                enemy.SwitchState(new ReloadState()); // Switch to ReloadState when out of ammo
            }

            lastAttackTime = Time.time; // Reset cooldown
        }
    }


    private void Fire(Enemy enemy)
    {
        if (muzzleFlashPrefab == null || shotPrefab == null || gunTransform == null)
        {
            Debug.LogError("References are missing!");
            return;
        }

        // Instantiate muzzle flash and projectile
        GameObject muzzleFlash = Object.Instantiate(muzzleFlashPrefab, gunTransform.position, gunTransform.rotation);
        GameObject projectile = Object.Instantiate(shotPrefab, gunTransform.position, gunTransform.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(gunTransform.forward * shotVel, ForceMode.Impulse);
        }

        // Clean up
        Object.Destroy(projectile, 5f);
        Object.Destroy(muzzleFlash, 0.1f);

        currentClip--; // Reduce ammo count
    }

    public void Exit(Enemy enemy)
    {
        // Cleanup if necessary when exiting AttackState
    }

    public void RefillAmmo()
    {
        currentClip = clipSize; // Refill ammo to full clip size
    }
}



