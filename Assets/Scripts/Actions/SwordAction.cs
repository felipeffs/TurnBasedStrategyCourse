using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State
    {
        Initial,
        SwingingSword,
        AfterSwingSword
    }

    [SerializeField] private float aimingDuration = .7f;
    [SerializeField] private float SwingingSwordDuration = .7f;

    private int maxSwordDistance = 1;
    private Coroutine currentCO_SwordAttack;
    private State currentState;
    private Unit targetUnit;

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

    private void NextState()
    {
        switch (currentState)
        {
            case State.Initial:
                DoState(Aim, aimingDuration, State.SwingingSword);
                break;
            case State.SwingingSword:
                SwingSword();
                DoState(() => { }, SwingingSwordDuration, State.AfterSwingSword);
                break;
            case State.AfterSwingSword:
                targetUnit.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
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

    private void SwingSword()
    {
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (var x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (var z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                if (!LevelGrid.Instance.HasAnyUnityOnGridPosition(testGridPosition)) continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy()) continue; // Both Units on same 'team'

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        currentState = State.Initial;

        NextState();
        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }

    public override int GetActionPointsCost()
    {
        return 4;
    }
}
