using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveAction : BaseAction
{
    private const float STOPPING_DISTANCE = .05f;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;

    private Coroutine currentCO_DoPath;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        DoPath(pathGridPositionList);

        ActionStart(onActionComplete);
    }

    private void DoPath(List<GridPosition> pathGridPositionList)
    {
        List<Vector3> positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        if (currentCO_DoPath != null)
        {
            StopCoroutine(currentCO_DoPath);
        }

        currentCO_DoPath = StartCoroutine(CO_DoPath(positionList));
    }

    private IEnumerator CO_DoPath(List<Vector3> positionList)
    {
        int currentPositionIndex = 0;

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        while (currentPositionIndex < positionList.Count)
        {
            Vector3 targetPosition = positionList[currentPositionIndex];
            yield return CO_Move(targetPosition);
            currentPositionIndex++;
        }

        OnStopMoving?.Invoke(this, EventArgs.Empty);
        ActionComplete();
    }

    private IEnumerator CO_Move(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > STOPPING_DISTANCE)
        {
            float rotateSpeed = 10f;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            yield return null;
        }
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (var x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (var z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                if (unitGridPosition == testGridPosition) continue; // Same Grid Position where unit is already at

                if (LevelGrid.Instance.HasAnyUnityOnGridPosition(testGridPosition)) continue;

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) continue;

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    // Path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }
}
