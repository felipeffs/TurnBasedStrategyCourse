using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveAction : BaseAction
{
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 4;

    private Coroutine currentCO_Move;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        var targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        this.onActionComplete = onActionComplete;

        if (currentCO_Move != null)
        {
            StopCoroutine(currentCO_Move);
        }
        currentCO_Move = StartCoroutine(CO_Move(targetPosition));
    }

    private IEnumerator CO_Move(Vector3 targetPosition)
    {
        const float StoppingDistance = .05f;

        unitAnimator.SetBool(isWalkingHash, true);

        while (Vector3.Distance(transform.position, targetPosition) > StoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            yield return null;
        }

        unitAnimator.SetBool(isWalkingHash, false);
        onActionComplete();
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
