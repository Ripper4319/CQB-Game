using UnityEngine;

public class ChaseState : IEnemyState
{
    public void Enter(Enemy enemy) { }

    public void Execute(Enemy enemy)
    {
        if (enemy.player == null) return;

        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        enemy.transform.position += direction * enemy.moveSpeed * Time.deltaTime;

        // If within attack range, switch to Attack state
        if (Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.attackRange)
        {
            enemy.SwitchState(new AttackState());
        }
    }

    public void Exit(Enemy enemy) { }
}

