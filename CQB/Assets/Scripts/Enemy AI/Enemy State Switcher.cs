using UnityEngine;

public class Enemy : MonoBehaviour
{
    public IEnemyState currentState; 
    public Transform playerlastseen; 
    public Transform player;
    public float detectionRange = 10f; 
    public float attackRange = 2f; 
    public float moveSpeed = 1f; 
    public Transform Enemybody;
    public Transform head;
    internal object playerlastSeen;
    public float playerLastSeenSpeed;
    public Animator Animator;

    public bool isshooting = false;

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

    [Header("Enemy ragdoll")]
    [SerializeField] private Rigidbody[] rb;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Animator character;

    public string CurrentStateName => currentState?.GetType().Name ?? "None";


    private void Start()
    {
        currentState = new PatrolState(); 
        currentState.Enter(this); 

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
        currentState.Execute(this); 

        currentState?.Execute(this);


        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        DetectPlayer();


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
                    playerlastseen.position = player.position; 
                   

                    if (!isshooting)
                    {
                        SwitchState(new AttackState(muzzleFlashPrefab, shot, gunTransform));
                        isshooting = true;
                    }


                    Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
                    if (playerRigidbody != null)
                    {
                        float playerSpeed = playerRigidbody.velocity.magnitude;
                        playerLastSeenSpeed = playerSpeed;
                    }
                }
            }

        }
    }

    public void SwitchState(IEnemyState newState)
    {
        Debug.Log($"Switching state from {currentState?.GetType().Name ?? "None"} to {newState.GetType().Name}");
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

}



