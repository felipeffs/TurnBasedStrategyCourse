using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject;
    [SerializeField] private TextMeshPro gridText;

    private void Update()
    {
        gridText.text = gridObject.ToString();
    }

    public void setGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }
}
