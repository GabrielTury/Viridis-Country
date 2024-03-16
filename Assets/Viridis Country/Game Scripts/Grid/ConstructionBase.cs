using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstructionBase : MonoBehaviour
{
    protected bool isBeingDragged;

    public void SetDragging(bool newValue)
    {
        isBeingDragged = newValue;
        
        if(!isBeingDragged)
        {
            SnapToGrid();
        }
    }
    protected void SnapToGrid()
    {
        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position);

        transform.position = new Vector3(cellPos.x, transform.position.y, cellPos.z);
    }
}
