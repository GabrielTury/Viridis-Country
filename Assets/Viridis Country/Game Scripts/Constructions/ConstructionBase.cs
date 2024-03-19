using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstructionBase : MonoBehaviour
{
    protected bool isBeingDragged;

    [SerializeField,Tooltip("Range from which it can gather resources")]
    protected int gatherRadius;

    protected int resourcesInRange;

    protected GridCell currentCell;

    public void SetDragging(bool newValue)
    {
        isBeingDragged = newValue;
        
        if(!isBeingDragged)
        {
            SnapToGrid();
        }
        else if(currentCell != null && isBeingDragged != currentCell.isAvailable)
        {
            currentCell.SetAvailability(true);
        }
    }
    /// <summary>
    /// Set Object X and Z position to the nearest Cell
    /// </summary>
    protected void SnapToGrid()
    {
        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position, out currentCell);
        
        currentCell.SetAvailability(false);

        transform.position = new Vector3(cellPos.x, transform.position.y, cellPos.z);
    }

    /// <summary>
    /// Iterate through all blocks in range and see how many resources there are
    /// </summary>
    /// <param name="resourceToGather"></param>
    /// <param name="resourceAmount">out the amount fo resources in range</param>
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


    [ContextMenu("Teste")] //Método a ser removido
    protected void Teste()
    {
        GetResourcesInRange("Wood",out resourcesInRange);
        Debug.Log(resourcesInRange);
    }
}
