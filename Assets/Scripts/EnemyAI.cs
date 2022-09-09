using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    private const float SECONDS_TO_WAIT = 2f;
    private WaitForSeconds waitSomeTime = new WaitForSeconds(SECONDS_TO_WAIT);
    private Coroutine currentCO_PassTurnAfterSomeTime;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private IEnumerator CO_PassTurnAfterSomeTime()
    {
        yield return waitSomeTime;

        TurnSystem.Instance.NextTurn();
    }

    private void PassTurnAfterSomeTime()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) return;

        if (currentCO_PassTurnAfterSomeTime != null)
        {
            StopCoroutine(currentCO_PassTurnAfterSomeTime);
        }
        currentCO_PassTurnAfterSomeTime = StartCoroutine(CO_PassTurnAfterSomeTime());
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        PassTurnAfterSomeTime();
    }
}
