using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;
    private Vector3 targetPosition;
    Coroutine currentMove;
    int IsWalkingHash;

    private void Awake()
    {
        IsWalkingHash = Animator.StringToHash("IsWalking");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (currentMove != null)
            {
                StopCoroutine(currentMove);
            }
            currentMove = StartCoroutine(Move(MouseWorld.GetPosition()));
        }
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        float stoppingDistance = .01f;
        this.targetPosition = targetPosition;

        unitAnimator.SetBool(IsWalkingHash, true);

        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed); ;

            yield return null;
        }

        unitAnimator.SetBool(IsWalkingHash, false);
    }
}
