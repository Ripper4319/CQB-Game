using UnityEngine;

public class IdleState : IEnemyState
{
    private float idleTime = 2f; // Time to stay idle before doing something (optional)
    private float idleTimer = 0f;

    public void Enter(Enemy enemy)
    {
        idleTimer = 0f; // Reset the idle timer

        // Add any idle-specific setup (e.g., animation triggers)
        // For example: enemy.GetComponent<Animator>().SetTrigger("Idle");
    }

    public void Execute(Enemy enemy)
    {
        idleTimer += Time.deltaTime; // Count up the idle time



        // Optional: Add idle behavior (e.g., random head movement)
        float randomAngle = Mathf.Sin(Time.time) * 15f; // Small oscillation effect
        enemy.head.rotation = Quaternion.Euler(0, randomAngle, 0);
    }

    public void Exit(Enemy enemy)
    {
        // Cleanup or reset any idle-specific behavior
        // For example: enemy.GetComponent<Animator>().ResetTrigger("Idle");
    }
}


