using UnityEngine;

public class InvestigateState : IEnemyState
{
    public Vector3 investigationPoint; // Point where the enemy investigates
    private float rotationSpeed = 2f;  // Speed of turning
    private bool interrupted = false;

    public InvestigateState(Vector3 point)
    {
        investigationPoint = point;
    }

    public void Enter(Enemy enemy)
    {
        // Reset interrupted flag when entering the state
        interrupted = false;

        Debug.Log("Enemy has entered InvestigateState.");
    }

    public void Execute(Enemy enemy)
    {
        if (interrupted) return; // Stop processing if interrupted

        // Move towards the investigation point first
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, investigationPoint, enemy.moveSpeed * Time.deltaTime);

        // Investigate by rotating towards the investigation point
        Vector3 direction = (investigationPoint - enemy.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smooth rotation towards the target point
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Check if the enemy has reached the investigation point or close to it
        if (Vector3.Distance(enemy.transform.position, investigationPoint) < 0.5f &&
            Vector3.Angle(enemy.transform.forward, direction) < 5f)
        {
            // Once the enemy has arrived and facing the point, switch to Idle or Patrol
            enemy.SwitchState(new IdleState());
        }
    }

    public void Exit(Enemy enemy)
    {
        interrupted = true; // Stop the rotation when exiting the state
    }
}
