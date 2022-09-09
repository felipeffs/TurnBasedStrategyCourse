using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        UpdateBusyUI(false);
    }

    public void UpdateBusyUI(bool isBusy)
    {
        gameObject.SetActive(isBusy);
    }

    public void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        UpdateBusyUI(isBusy);
    }
}
