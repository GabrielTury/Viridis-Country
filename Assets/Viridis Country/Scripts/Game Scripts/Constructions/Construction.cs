
using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    private bool isBeingDragged;

    [SerializeField]
    private ConstructionTemplate construcion;

    private int gatherRadius;

    private int resourcesInRange;

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

            resourcesInRange = GetResourcesInRange(resourcesInRange);//VER ERRO E ARRUMAR

            //GameEvents.OnResourceGathered(resourceToGather, resourcesInRange);
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
    private void SnapToGrid()
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
    private int GetResourcesInRange(int resourceAmount) //VER E ARRUMAR
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
        {
            for (int i = 0; i < currentCell.resource.Length; i++)
            {
                if (i < resourceToGather.Length)
                    GameEvents.OnResourceGathered(resourceToGather[i], diff);
            }
        }

        GameEvents.OnConstructionPlaced(cType);

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
