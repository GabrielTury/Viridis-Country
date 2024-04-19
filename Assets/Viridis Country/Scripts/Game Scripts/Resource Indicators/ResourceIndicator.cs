
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIndicator : MonoBehaviour
{
    [SerializeField]
    private GameManager.GameResources resource;

    [SerializeField]
    private GridCell.TileType tileType;


    private void Start()
    {
        SnapToGridAndSetResource();
    }
    
    private void SnapToGridAndSetResource()
    {
        GridCell cell;

        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position, out cell, tileType);

        cell.SetAvailability(false);

        for(int i = 0; i < cell.resource.Length; i++)
            cell.SetResource(resource, i);

        transform.position = new Vector3(cellPos.x, transform.position.y, cellPos.z);
    }
}
