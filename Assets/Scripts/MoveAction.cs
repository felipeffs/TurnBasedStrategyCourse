using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;
    private Coroutine currentCO_Move;
    private Unit unit;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    private void Awake()
    {
        unit = GetComponent<Unit>();
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

    public List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (var x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (var z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                Debug.Log(testGridPosition);
            }
        }

        return validGridPositionList;
    }
}
