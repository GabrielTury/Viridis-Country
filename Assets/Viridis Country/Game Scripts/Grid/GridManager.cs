using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private GridCell[] gridCells;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        gridCells = FindObjectsOfType<GridCell>();
    }

    public Vector3 NearestCellPosition(Vector3 currentPosition)
    {
        Vector3 nearestCellPosition = new Vector3();
        float distance = 0;
        foreach(GridCell cell in gridCells)
        {
            float cellDistance = Vector3.Distance(cell.transform.position, currentPosition);
            if (distance == 0)
            {
                nearestCellPosition = cell.transform.position;
                distance = cellDistance;
            }
            else if (cellDistance < distance) 
            {
                nearestCellPosition = cell.transform.position;
                distance = cellDistance;
            }
            
        }

        return nearestCellPosition;
    }

    public GridCell[] GetCellFromRadius(Vector3 position, int radius)
    {
        List<GridCell> cellsInRadius = new List<GridCell>();
        foreach(GridCell cell in gridCells)
        {
            for (int i = radius; i >= -radius; i--)
            {
                for(int j = radius; j >= -radius; j--)
                {
                    if (cell.simplePosX == position.x + i && cell.simplePosZ == position.z + j)
                    {
                        cellsInRadius.Add(cell);
                    }
                }

            }
           
        }

        return cellsInRadius.ToArray();
    }
}
