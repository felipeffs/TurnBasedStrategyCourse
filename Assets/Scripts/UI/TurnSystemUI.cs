using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;

        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        UpdateTurnText();
        UpdateEnemyTurnVisualVisibility();
        UpdateEndTurnButtonVisibility(TurnSystem.Instance.IsPlayerTurn());
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        UpdateEndTurnButtonVisibility(!isBusy);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisualVisibility();
        UpdateEndTurnButtonVisibility(TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateTurnText()
    {
        turnNumberText.text = $"TURN {TurnSystem.Instance.GetTurnNumber()}";
    }

    private void UpdateEnemyTurnVisualVisibility()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility(bool isVisible)
    {
        endTurnButton.gameObject.SetActive(isVisible);
    }
}
