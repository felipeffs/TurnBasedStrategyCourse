using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private const float ONE_LAP_ROTATION_DEGREES = 360f;

    private float totalSpinAmount;
    private Coroutine currentCO_Spin;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Spin();

        ActionStart(onActionComplete);
    }

    private void Spin()
    {
        totalSpinAmount = 0f;

        if (currentCO_Spin != null)
        {
            StopCoroutine(currentCO_Spin);
        }
        currentCO_Spin = StartCoroutine(CO_Spin());
    }

    private IEnumerator CO_Spin()
    {
        while (totalSpinAmount < ONE_LAP_ROTATION_DEGREES)
        {
            float spinAddAmount = ONE_LAP_ROTATION_DEGREES * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

            totalSpinAmount += spinAddAmount;

            yield return null;
        }
        ActionComplete();
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

}
