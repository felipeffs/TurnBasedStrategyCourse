using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    private int maxThrowDistance = 7;

    private void ThrowGrenade(GridPosition targetGridPosition)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();

        grenadeProjectile.Setup(targetGridPosition, OnGrenadeBehaviourComplete);
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (var x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (var z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance) continue;

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 targetAreaWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 throwDirection = (targetAreaWorldPosition - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;

                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    throwDirection,
                    Vector3.Distance(unitWorldPosition, targetAreaWorldPosition), obstaclesLayerMask)
                    ) continue; // Blocked by an Obstacle

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        ThrowGrenade(gridPosition);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

    public override int GetActionPointsCost()
    {
        return 6;
    }
}
