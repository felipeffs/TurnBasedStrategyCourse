using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private const float ONE_LAP_ROTATION = 360f;

    private float totalSpinAmount;
    private bool isActive;

    private void Update()
    {
        if (!isActive) return;

        float spinAddAmount = ONE_LAP_ROTATION * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount >= ONE_LAP_ROTATION)
        {
            isActive = false;
            onActionComplete();
        }
    }

    public void Spin(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;
        totalSpinAmount = 0f;
    }

    public override string GetActionName()
    {
        return "Spin";
    }
}
