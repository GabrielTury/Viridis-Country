using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstructionBase : MonoBehaviour
{
    protected bool isBeingDragged;
    [SerializeField,Tooltip("Range from which it can gather resources")]
    protected int gatherRadius;

    protected int resourcesInRange;

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

    /*[ContextMenu("GetResourcesInRange")]*/
    protected void GetResourcesInRange(string resourceToGather, out int resourceAmount)
    {
        resourceAmount = 0;
        GridCell[] cellsInRadius = GridManager.Instance.GetCellFromRadius(transform.position, gatherRadius);
        
        
        foreach(GridCell cell in cellsInRadius) 
        {
           if(cell.resource == resourceToGather)
            {
                resourceAmount++;
            }
        }
        
    }
    [ContextMenu("Teste")]
    protected void Teste()
    {
        GetResourcesInRange("Wood",out resourcesInRange);
        Debug.Log(resourcesInRange);
    }
}
