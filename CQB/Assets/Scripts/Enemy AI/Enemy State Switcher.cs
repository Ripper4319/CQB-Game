using UnityEngine;

public class Enemy : MonoBehaviour
{
    public IEnemyState currentState; // Current state of the enemy
    public Transform player; // Reference to the player
    public float detectionRange = 10f; // Distance at which the enemy starts detecting the player
    public float attackRange = 2f; // Distance at which the enemy attacks the player
    public float moveSpeed = 3f; // Movement speed of the enemy
    public Transform head;

    private void Start()
    {
        currentState = new PatrolState(); // Default state is PatrolState
        currentState.Enter(this); // Enter the Patrol state at the beginning
    }

    private void Update()
    {
        currentState.Execute(this); // Execute the current state's behavior

        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
            return;
        }


        // Get the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - head.position).normalized;
            float angleToPlayer = Vector3.Angle(head.forward, directionToPlayer);

            if (angleToPlayer < 50f) // Adjust based on your FOV
            {
                if (Physics.Raycast(head.position, directionToPlayer, out RaycastHit hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        SwitchState(new AttackState());

                        //playerLastSeen.transform.position = hit.point;
                        //playerLastSeenTimer = 0f; // Reset timer on player sight
                    }
                }
            }
        }
    
        // If the player is within detection range but not in attack range, switch to ChaseState
        else if (distanceToPlayer <= detectionRange)
        {
            SwitchState(new ChaseState()); // Switch to ChaseState if player is detected but not close enough to attack
        }
        // If the player is out of range, switch to IdleState
        else
        {
            SwitchState(new IdleState()); // Switch to IdleState when the player is out of range
        }
    }

    // Method to switch to a new state
    public void SwitchState(IEnemyState newState)
    {
        currentState.Exit(this); // Exit the current state
        currentState = newState; // Set the new state
        currentState.Enter(this); // Enter the new state
    }
}



