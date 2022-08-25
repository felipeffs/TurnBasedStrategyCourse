using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;

    private Vector3 targetPosition;
    private GridPosition gridPosition;

    private Coroutine currentCO_Move;
    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public void Move(Vector3 targetPosition)
    {
        if (currentCO_Move != null)
        {
            StopCoroutine(currentCO_Move);
        }
        currentCO_Move = StartCoroutine(CO_Move(targetPosition));
    }

    private IEnumerator CO_Move(Vector3 targetPosition)
    {
        float stoppingDistance = .01f;
        this.targetPosition = targetPosition;

        unitAnimator.SetBool(isWalkingHash, true);

        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed); ;

            yield return null;
        }

        unitAnimator.SetBool(isWalkingHash, false);
    }
}
