using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    private const float TIME_BEFORE_FIRST_ACTION = 2f;
    private const float TIME_BETWEEN_ACTIONS = .5f;

    private Coroutine currentCO_WaitBeforeNextAction;
    private bool isFirstActionTurn;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn()) return;

        isFirstActionTurn = true;

        WaitBeforeNextAction();
    }

    private void WaitBeforeNextAction()
    {
        if (currentCO_WaitBeforeNextAction != null)
        {
            StopCoroutine(currentCO_WaitBeforeNextAction);
        }
        currentCO_WaitBeforeNextAction = StartCoroutine(CO_WaitBeforeNextAction());
    }

    private IEnumerator CO_WaitBeforeNextAction()
    {
        WaitForSeconds waitTime = new WaitForSeconds(TIME_BETWEEN_ACTIONS);

        if (isFirstActionTurn)
        {
            waitTime = new WaitForSeconds(TIME_BEFORE_FIRST_ACTION);
            isFirstActionTurn = false;
        }

        yield return waitTime;

        DoNextAction();
    }

    private void DoNextAction()
    {
        if (!TryTakeEnemyAIAction(WaitBeforeNextAction))
        {
            TurnSystem.Instance.NextTurn();
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        Debug.Log("Take Enemy AI Action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        SpinAction spinAction = enemyUnit.GetSpinAction();

        if (spinAction == null) return false;

        GridPosition actionGridPosition = enemyUnit.GetGridPosition();

        if (!spinAction.IsValidActionGridPosition(actionGridPosition)) return false;

        if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction)) return false;

        Debug.Log("Spin Action!");
        spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
        return true;
    }

}