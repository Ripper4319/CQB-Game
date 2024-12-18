using UnityEngine;

public class IdleState : IEnemyState
{
    private float idleTime = 2f; // Time to stay idle before doing something (optional)
    private float idleTimer = 0f;

    public void Enter(Enemy enemy)
    {
        // Optionally reset the idle timer when entering the idle state
        idleTimer = 0f;

        // You can add animations or behavior specific to the idle state here
        // For example: enemy.GetComponent<Animator>().SetTrigger("Idle");
    }

    public void Execute(Enemy enemy)
    {
        // Count up the idle time
        idleTimer += Time.deltaTime;

        // After a certain idle time, you can make the enemy start patrolling again or do something else
        if (idleTimer >= idleTime)
        {
            // If the player is in range, start chasing, otherwise keep idling or transition to another behavior
            if (Vector3.Distance(enemy.player.position, enemy.transform.position) <= enemy.detectionRange)
            {
                enemy.SwitchState(new ChaseState());
            }
            else
            {
                // Optionally switch to Patrol or remain idle indefinitely
                enemy.SwitchState(new PatrolState());
            }

            // Reset the timer for the next idle state
            idleTimer = 0f;
        }

        // Here, you could add additional idle behavior like looking around
        // For example, you could randomize the enemy's rotation during idle time
        float randomAngle = Mathf.Sin(Time.time) * 15f; // Small oscillation effect
        enemy.transform.rotation = Quaternion.Euler(0, randomAngle, 0);
    }

    public void Exit(Enemy enemy)
    {
        // Reset or stop any idle-specific behavior
        // For example: enemy.GetComponent<Animator>().ResetTrigger("Idle");
    }
}


