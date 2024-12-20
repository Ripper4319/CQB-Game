using UnityEngine;

public class Enemy : MonoBehaviour
{
    public IEnemyState currentState; // Current state of the enemy
    public Transform playerlastseen; // Reference to the playerlastseen
    public Transform player;
    public float detectionRange = 10f; // Distance at which the enemy starts detecting the player
    public float attackRange = 2f; // Distance at which the enemy attacks the player
    public float moveSpeed = 1f; // Movement speed of the enemy
    public Transform Enemybody;
    public Transform head;
    internal object playerlastSeen;
    public float playerLastSeenSpeed;
    public Animator Animator;

    public GameObject muzzleFlashPrefab;
    public GameObject shot;
    public Transform gunTransform;

    [Header("Enemy damage")]
    [SerializeField] private float HeadMultiplier = 2;
    [SerializeField] private float TorsoMultiplier = 1;
    [SerializeField] private float LeftArmMultiplier = 0.5f;
    [SerializeField] private float RightArmMultiplier = 0.5f;
    [SerializeField] private float LeftLegMultiplier = 0.75f;
    [SerializeField] private float RightLegMultiplier = 0.75f;
    [SerializeField] private float BellyMultiplier = 1;
    //[SerializeField] public BodyPart[] bodyPartMapping;

    [Header("Enemy ragdoll")]
    [SerializeField] private Rigidbody[] rb;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Animator character;

    public string CurrentStateName => currentState?.GetType().Name ?? "None";


    private void Start()
    {
        currentState = new PatrolState(); // Default state is PatrolState
        currentState.Enter(this); // Enter the Patrol state at the beginning

        foreach (Rigidbody rb in rb)
        {
            rb.isKinematic = true;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

    }

    private void Update()
    {
        currentState.Execute(this); // Execute the current state's behavior

        currentState?.Execute(this);


        // Determine next action based on distance to the player
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - head.position).normalized;
            float angleToPlayer = Vector3.Angle(head.forward, directionToPlayer);

            if (angleToPlayer < 50f) // Field of view check
            {
                if (Physics.Raycast(head.position, directionToPlayer, out RaycastHit hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Update playerLastSeen position
                        if (playerlastseen != null)
                        {
                            playerlastseen.position = player.position; // Assign the player's current position
                        }

                        // Assign the player's speed to a variable on playerLastSeen
                        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
                        if (playerRigidbody != null)
                        {
                            // Calculate speed as magnitude of velocity
                            float playerSpeed = playerRigidbody.velocity.magnitude;
                            playerLastSeenSpeed = playerSpeed; // Store the speed in a variable (add this field to Enemy)
                        }

                        // Switch to attack state
                        SwitchState(new AttackState(muzzleFlashPrefab, shot, gunTransform));

                        return;
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

    private void DetectPlayer()
    {
        if (player == null || head == null) return;

        Vector3 directionToPlayer = (player.position - head.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        float angleToPlayer = Vector3.Angle(head.forward, directionToPlayer);

        if (distanceToPlayer <= detectionRange && angleToPlayer < 50f)
        {
            if (Physics.Raycast(head.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerlastseen.position = player.position; // Update player last seen
                    SwitchState(new AttackState(muzzleFlashPrefab, shot, gunTransform));
                }
            }
        }
    }

    // Method to switch to a new state
    public void SwitchState(IEnemyState newState)
    {
        Debug.Log($"Switching state from {currentState?.GetType().Name ?? "None"} to {newState.GetType().Name}");
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

}



