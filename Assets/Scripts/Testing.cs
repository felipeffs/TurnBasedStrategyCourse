using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] Unit unit;
    private void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            unit.GetMoveAction().GetValidActionGridPositionList();
        }
    }
}
