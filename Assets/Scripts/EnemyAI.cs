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
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }

}