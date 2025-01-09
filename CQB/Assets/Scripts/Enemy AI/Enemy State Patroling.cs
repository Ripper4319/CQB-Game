using UnityEngine;

public class PatrolState : IEnemyState
{
    private float patrolTime = 0f;
    private Vector3 patrolPoint;

    public void Enter(Enemy enemy)
    {
        patrolPoint = enemy.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
    }

    public void Execute(Enemy enemy)
    {
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, patrolPoint, enemy.moveSpeed * Time.deltaTime);

        if (Vector3.Distance(enemy.transform.position, patrolPoint) < 0.5f)
        {
            
            patrolPoint = enemy.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        }
       
        enemy.SwitchState(new PatrolState());
        return;

    }

    public void Exit(Enemy enemy) { }
}
