using UnityEngine;

public class InvestigateState : IEnemyState
{
    public Vector3 investigationPoint; 
    private float rotationSpeed = 2f;  
    private bool interrupted = false;

    public InvestigateState(Vector3 point)
    {
        investigationPoint = point;
    }

    public void Enter(Enemy enemy)
    {
        interrupted = false;

        Debug.Log("Enemy has entered InvestigateState.");
    }

    public void Execute(Enemy enemy)
    {
        if (interrupted) return; 

        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, investigationPoint, enemy.moveSpeed * Time.deltaTime);

        Vector3 direction = (investigationPoint - enemy.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(enemy.transform.position, investigationPoint) < 0.5f &&
            Vector3.Angle(enemy.transform.forward, direction) < 5f)
        {
            enemy.SwitchState(new IdleState());
        }
    }

    public void Exit(Enemy enemy)
    {
        interrupted = true; 
    }
}
