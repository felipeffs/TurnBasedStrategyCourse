using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private readonly int walkingAnimationHash = Animator.StringToHash("IsWalking");
    private readonly int shootAnimationHash = Animator.StringToHash("Shoot");

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool(walkingAnimationHash, true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool(walkingAnimationHash, false);
    }

    private void ShootAction_OnShoot(object sender, EventArgs e)
    {
        animator.SetTrigger(shootAnimationHash);
    }
}
