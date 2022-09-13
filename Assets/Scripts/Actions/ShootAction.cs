using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    private enum State
    {
        Initial,
        Aiming,
        Shooting,
        Cooloff
    }

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    [SerializeField] private float aimingStateDuration = 2f;
    [SerializeField] private float shootingStateDurationPerShot = .1f;
    [SerializeField] private float coolOffStateDuration = .5f;

    private int maxShootDistance = 7;
    private State currentState;
    private Unit targetUnit;
    private int currentBullets;
    [SerializeField] private int maxBullets = 2;
    private bool canShoot = true;
    private bool isStateFirstCycle = true;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        currentState = State.Initial;
        NextState();

        ActionStart(onActionComplete);
    }

    private void NextState()
    {
        isStateFirstCycle = true;

        switch (currentState)
        {
            case State.Initial:
                DoState(Aim, aimingStateDuration, State.Aiming);
                break;
            case State.Aiming:
                float shootingStateEndDelay = .05f;
                float shootingStateDuration = shootingStateDurationPerShot * maxBullets + shootingStateEndDelay;
                DoState(Shoot, shootingStateDuration, State.Shooting);
                break;
            case State.Shooting:
                DoState(() => { }, coolOffStateDuration, State.Cooloff);
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }

        Debug.Log(currentState);
    }

    private void Aim()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }

    private void Shoot()
    {
        if (isStateFirstCycle)
        {
            currentBullets = maxBullets;
            canShoot = true;
            isStateFirstCycle = false;
        }

        if (targetUnit == null) return;
        if (currentBullets == 0) return;
        if (!canShoot) return;

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = this.targetUnit,
            shootingUnit = unit
        });

        targetUnit.Damage(40);

        currentBullets--;
        canShoot = false;
        StartCoroutine(CO_Wait(shootingStateDurationPerShot, () => { canShoot = true; }));
    }

    private void DoState(Action stateBehaviour, float durationInSeconds, State newState)
    {
        currentState = newState;
        StartCoroutine(CO_DoState(stateBehaviour, durationInSeconds));
    }

    private IEnumerator CO_DoState(Action stateBehaviour, float durationInSeconds)
    {
        float currentTimer = durationInSeconds;
        while (currentTimer > 0f)
        {
            stateBehaviour();
            yield return null;
            currentTimer -= Time.deltaTime;
        }

        NextState();
    }

    private IEnumerator CO_Wait(float timeToWaitInSeconds, Action Method)
    {
        yield return new WaitForSeconds(timeToWaitInSeconds);
        Method();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (var x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (var z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance) continue;

                if (!LevelGrid.Instance.HasAnyUnityOnGridPosition(testGridPosition)) continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy()) continue; // Both Units on same 'team'

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }
}