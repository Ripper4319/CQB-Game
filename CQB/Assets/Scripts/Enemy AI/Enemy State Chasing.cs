using UnityEngine;

public class ChaseState : IEnemyState
{
    private float stoppingDistance = 1.0f; // Distance at which the chase stops

    public void Enter(Enemy enemy)
    {
        
    }

    public void Execute(Enemy enemy)
    {
        if (enemy.playerlastseen == null) return;

        // Calculate the direction to the player's last seen position
        Vector3 direction = (enemy.playerlastseen.position - enemy.transform.position).normalized;
        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.playerlastseen.position);

        if (distanceToTarget > stoppingDistance)
        {
            // Move towards the player's last seen position
            enemy.transform.position += direction * enemy.moveSpeed * Time.deltaTime;

            // Trigger walking animation
            if (enemy.Animator != null)
            {
                enemy.Animator.SetBool("isWalking", true);
            }
        }
        else
        {
            // Stop moving and switch to InvestigateState
            if (enemy.Animator != null)
            {
                enemy.Animator.SetBool("isWalking", false);
            }

            Debug.Log("Switching to Investigate State");
            enemy.SwitchState(new InvestigateState(enemy.playerlastseen.position));
        }
    }

    public void Exit(Enemy enemy)
    {
     ;
    }
}

