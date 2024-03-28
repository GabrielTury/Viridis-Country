
using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionBase : MonoBehaviour
{
    private bool isBeingDragged;

    [SerializeField]
    private ConstructionTemplate construcion;

    private int gatherRadius;

    private int resourcesInRange;

    private GameManager.GameResources resourceToGather;

    private GridCell currentCell;

    private void OnEnable()
    {
        GetComponent<MeshFilter>().mesh = construcion.constructionMesh;
        GetComponent<MeshRenderer>().material = construcion.material;   
        gatherRadius = construcion.gatherRadius;
        resourceToGather = construcion.resourceToGather;

    }

    public void SetDragging(bool newValue)
    {
        isBeingDragged = newValue;

        if (!isBeingDragged)
        {
            SnapToGrid();
            resourcesInRange = GetResourcesInRange(resourcesInRange);
            //GameEvents.OnResourceGathered(resourceToGather, resourcesInRange);
        }
        else if (currentCell != null && isBeingDragged != currentCell.isAvailable)
        {
            currentCell.SetAvailability(true);
        }
    }
    /// <summary>
    /// Set Object X and Z position to the nearest Cell
    /// </summary>
    private void SnapToGrid()
    {
        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position, out currentCell);

        currentCell.SetAvailability(false);

        transform.position = new Vector3(cellPos.x, transform.position.y, cellPos.z);
    }

    /// <summary>
    /// Iterate through all blocks in range and see how many resources there are
    /// </summary>
    /// <param name="resourceAmount">out the amount fo resources in range</param>
    private int GetResourcesInRange(int resourceAmount)
    {
        resourceAmount = 0;
        //Debug.Log(resourcesInRange + "Resources In Range");

        GridCell[] cellsInRadius = GridManager.Instance.GetCellFromRadius(transform.position, gatherRadius);


        foreach (GridCell cell in cellsInRadius)
        {
            if (cell.resource == resourceToGather)
            {
                Debug.Log(cell.resource);
                resourceAmount++;
            }
        }

        int diff = resourceAmount - resourcesInRange;

        if(diff != 0)
            GameEvents.OnResourceGathered(resourceToGather, diff);

        return resourceAmount;
    }
}
