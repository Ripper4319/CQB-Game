using UnityEngine;
using System.Collections;

public class ReloadState : IEnemyState
{
    private float reloadTime = 2f; // Time to reload
    private bool isReloading = false;

    public void Enter(Enemy enemy)
    {
        isReloading = true;
        enemy.StartCoroutine(Reload(enemy));
    }

    public void Execute(Enemy enemy)
    {
        // While reloading, do nothing. Transition back to AttackState happens after reload.
    }

    public void Exit(Enemy enemy)
    {
        isReloading = false; // Mark reloading as complete
    }

    private IEnumerator Reload(Enemy enemy)
    {
        yield return new WaitForSeconds(reloadTime);

        // Assuming the clip size is available through AttackState or passed as a parameter
        AttackState attackState = enemy.currentState as AttackState;
        if (attackState != null)
        {
            attackState.RefillAmmo(); // Refill the clip
        }

        enemy.SwitchState(new IdleState());
    }
}

