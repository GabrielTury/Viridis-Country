using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridCell;

[DefaultExecutionOrder(-1)]
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


        for(int i = 0; i < gridCells.Length; i++)
        {
            gridCells[i].gameObject.name = "Cell " + i;

        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <param name="closestCell">out the nearest available GridCell</param>
    /// <returns>Returns the Position of the nearest available gridCell</returns>
    public Vector3 NearestCellPosition(Vector3 currentPosition, out GridCell closestCell, GridCell.TileType tileType)
    {
        Vector3 nearestCellPosition = new Vector3();
        float distance = 0;

        closestCell = null;
        foreach(GridCell cell in gridCells)
        {
            float cellDistance = Vector3.Distance(cell.transform.position, currentPosition);

            if (distance == 0 && cell.isAvailable && cell.tileType == tileType)
            {
                nearestCellPosition = cell.transform.position;
                distance = cellDistance;

                closestCell = cell;
            }
            else if (cellDistance < distance && cell.isAvailable && cell.tileType == tileType) 
            {
                nearestCellPosition = cell.transform.position;
                distance = cellDistance;

                closestCell = cell;
            }
            
        }

        return nearestCellPosition;
    }

    public Vector3 CheckNearestCell(Vector3 currentPosition, GridCell.TileType tileType)
    {
        Vector3 nearestCellPosition = new Vector3();
        float distance = 0;

        foreach (GridCell cell in gridCells)
        {
            float cellDistance = Vector3.Distance(cell.transform.position, currentPosition);

            if (distance == 0 && cell.isAvailable && cell.tileType == tileType)
            {
                nearestCellPosition = cell.transform.position;
                distance = cellDistance;
            }
            else if (cellDistance < distance && cell.isAvailable && cell.tileType == tileType)
            {
                nearestCellPosition = cell.transform.position;
                distance = cellDistance;
            }
        }
        return nearestCellPosition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius">How many blocks will it search in X and Z axis</param>
    /// <returns>Returns an array with all cells in radius</returns>
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
