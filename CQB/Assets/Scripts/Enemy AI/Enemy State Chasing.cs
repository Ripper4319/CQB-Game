using UnityEngine;

public class ChaseState : IEnemyState
{
    public void Enter(Enemy enemy) { }

    public void Execute(Enemy enemy)
    {
        if (enemy.playerlastseen == null) return;

        Vector3 direction = (enemy.playerlastseen.position - enemy.transform.position).normalized;
        enemy.transform.position += direction * enemy.moveSpeed * Time.deltaTime;

    }

    public void Exit(Enemy enemy) { }
}

