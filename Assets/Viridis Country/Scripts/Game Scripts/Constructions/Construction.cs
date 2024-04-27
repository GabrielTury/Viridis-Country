
using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    private bool isBeingDragged;

    public ConstructionTemplate construcion;

    private int gatherRadius;

    private int[] resourcesInRange;

    private GameManager.GameResources[] resourceToGather;

    [Tooltip("Atualmente tem o maximo de 2 recursos, tem que trocar no gridcell para alterar")]
    private GameManager.GameResources[] secondaryResource = new GameManager.GameResources[2];

    private GridCell currentCell;

    private GridCell.TileType tileType;

    private AudioManager.ConstructionAudioTypes cType;

    private void OnEnable()
    {
        GetComponent<MeshFilter>().mesh = construcion.constructionMesh;
        GetComponent<MeshRenderer>().material = construcion.material;   
        gatherRadius = construcion.gatherRadius;
        resourceToGather = construcion.resourceToGather;
        tileType = construcion.tileType;
        secondaryResource = construcion.secondaryResource;
        cType = construcion.constructionType;
        resourcesInRange = new int[resourceToGather.Length];

    }

    private void Start()
    {
        SnapToGrid();
    }

    public void SetDragging(bool newValue)
    {
        isBeingDragged = newValue;

        if (!isBeingDragged) //chama quando ele é largado
        {
            SnapToGrid();

            for(int i = 0; i < resourceToGather.Length; i++)
            {
                resourcesInRange[i] = GetResourcesInRange(resourceToGather[i], i);
                //Debug.Log("Chamou com: " + resourceToGather[i]);
            }

            GameEvents.OnConstructionPlaced(cType);

            if(isBeingDragged)
                SetDragging(false);
        }
        else if (currentCell != null && isBeingDragged != currentCell.isAvailable) //chamado quando ele tem um celula porém é retirado dessa célula
        {
            currentCell.SetAvailability(true);
            for(int i = 0; i < currentCell.resource.Length; i++)
            {
                currentCell.SetResource(GameManager.GameResources.None, i);

            }
        }
    }
    /// <summary>
    /// Set Object X and Z position to the nearest Cell
    /// </summary>
    public void SnapToGrid()
    {
        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position, out currentCell, tileType);

        currentCell.SetAvailability(false);
        for (int i = 0; i < currentCell.resource.Length; i++)
        {
            if (i < secondaryResource.Length)
                currentCell.SetResource(secondaryResource[i], i); //troca o recurso daquela celula para o recurso que produz para secundarias e terciarias

        }

        transform.position = new Vector3(cellPos.x, transform.position.y, cellPos.z);
    }

    /// <summary>
    /// Iterate through all blocks in range and see how many resources there are
    /// </summary>
    /// <param name="resourceAmount">out the amount fo resources in range</param>
    public int GetResourcesInRange(GameManager.GameResources resourceToCheck, int index) //VER E ARRUMAR
    {
        int resourceAmount = 0;
        //Debug.Log(resourcesInRange + "Resources In Range");

        GridCell[] cellsInRadius = GridManager.Instance.GetCellFromRadius(transform.position, gatherRadius);


        foreach (GridCell cell in cellsInRadius)
        {
            //Debug.Log("FOREACH: " + cell.resource);
            for (int i = 0; i < cell.resource.Length; i++)
            {
                //Debug.Log("FOR: "+cell.resource[i]);
                if (cell.resource[i] == resourceToCheck)
                {
                    //Debug.Log("RESOURCE CHECK: "+cell.resource[i]);                   
                    resourceAmount++;
                }
            }

        }

        int diff = resourceAmount - resourcesInRange[index];

        if(diff != 0)
        {
            //Debug.Log("Diff: "+ diff);
            GameEvents.OnResourceGathered(resourceToCheck, diff);          
        }

       
        return resourceAmount;
    }

    public void RemoveConstruction()
    {
        for (int i = 0; i < currentCell.resource.Length; i++)
            currentCell.SetResource(GameManager.GameResources.None, i);

        currentCell.SetAvailability(true);

        GameEvents.OnConstructionRemoved(cType);

        Destroy(gameObject);
    }
}
