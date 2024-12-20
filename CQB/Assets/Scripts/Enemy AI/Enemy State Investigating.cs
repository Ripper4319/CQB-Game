using UnityEngine;

public class InvestigateState : IEnemyState
{
    public Vector3 investigationPoint;  // Point where the enemy investigates
    private float turnSpeed = 2f;        // Speed of turning
    private bool isLookingAround = false;
    private Quaternion initialRotation; // Original head rotation
    private Quaternion leftRotation;    // Left turn rotation
    private Quaternion rightRotation;   // Right turn rotation
    private int rotationPhase = 0;      // Tracks the current phase of the head turning

    public InvestigateState(Vector3 point)
    {
        investigationPoint = point;
    }

    public void Enter(Enemy enemy)
    {
        // Move towards the investigation point
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, investigationPoint, enemy.moveSpeed * Time.deltaTime);

        // Setup the rotation points
        initialRotation = enemy.Enemybody.rotation;
        leftRotation = Quaternion.Euler(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y - 90f, initialRotation.eulerAngles.z);
        rightRotation = Quaternion.Euler(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y + 90f, initialRotation.eulerAngles.z);

        Debug.Log("Enemy has entered InvestigateState.");
    }

    public void Execute(Enemy enemy)
    {
        if (!isLookingAround)
        {
            isLookingAround = true;
            rotationPhase = 0;
        }

        // Rotate the head
        switch (rotationPhase)
        {
            case 0: // Turn left
                enemy.Enemybody.rotation = Quaternion.Slerp(enemy.Enemybody.rotation, leftRotation, turnSpeed * Time.deltaTime);
                if (Quaternion.Angle(enemy.Enemybody.rotation, leftRotation) < 1f)
                    rotationPhase++;
                break;

            case 1: // Turn right
                enemy.Enemybody.rotation = Quaternion.Slerp(enemy.Enemybody.rotation, rightRotation, turnSpeed * Time.deltaTime);
                if (Quaternion.Angle(enemy.Enemybody.rotation, rightRotation) < 1f)
                    rotationPhase++;
                break;

            case 2: // Return to initial rotation
                enemy.Enemybody.rotation = Quaternion.Slerp(enemy.Enemybody.rotation, initialRotation, turnSpeed * Time.deltaTime);
                if (Quaternion.Angle(enemy.Enemybody.rotation, initialRotation) < 1f)
                {
                    // Finish investigation
                    Debug.Log("Investigation complete. Returning to another state.");
                   // if (previousState is PatrolState)
                   //    enemy.SwitchState(new PatrolState());
                  //  else
                        enemy.SwitchState(new IdleState());

                }
                break;
        }



    }

    public void Exit(Enemy enemy)
    {
        // Cleanup or reset any investigation-specific values
        Debug.Log("Enemy exited InvestigateState.");
    }
}
