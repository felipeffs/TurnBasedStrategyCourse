using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private readonly int doorAnimationHash = Animator.StringToHash("IsOpen");
    [SerializeField] private bool isOpen;
    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractComplete;
    private Coroutine currentCO_PlayAndWaitDoorAnimation;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetDoorAtGridPosition(gridPosition, this);
        OpenOrClose(isOpen);
    }

    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        OpenOrClose(!isOpen);
    }

    private void OpenOrClose(bool isOpen)
    {
        this.isOpen = isOpen;
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, isOpen);

        if (currentCO_PlayAndWaitDoorAnimation != null)
        {
            StopCoroutine(currentCO_PlayAndWaitDoorAnimation);
        }
        currentCO_PlayAndWaitDoorAnimation = StartCoroutine(CO_PlayAndWaitDoorAnimation(onInteractComplete));
    }

    public IEnumerator CO_PlayAndWaitDoorAnimation(Action onAnimationComplete)
    {
        animator.SetBool(doorAnimationHash, isOpen);

        //Wait until the current state is done playing
        while ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1 < 0.99f)
        {
            yield return null;
        }

        onAnimationComplete?.Invoke();
    }
}