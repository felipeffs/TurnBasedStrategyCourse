using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Vector3 targetPosition;
    Coroutine currentMove;

    //Temporary
    Vector3 newPosition = new Vector3(3, 0, 3);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (currentMove != null)
            {
                StopCoroutine(currentMove);
            }
            currentMove = StartCoroutine(Move(newPosition));
        }
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        float stoppingDistance = .01f;
        this.targetPosition = targetPosition;

        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
