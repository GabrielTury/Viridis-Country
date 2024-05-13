
using GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    private ResourceTouchHandler touchHandler;

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

    private List<GridCell> cellCollected = new List<GridCell>();

    private bool isDestroying = false;

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

        GameEvents.Select_Construction += ReCheckResources;
        GameEvents.Level_Start += PlacedOnStart;
    }

    private void OnDisable()
    {
        GameEvents.Select_Construction -= ReCheckResources;
        GameEvents.Level_Start -= PlacedOnStart;
    }

    private void Start()
    {
        //SetDragging(false);
        touchHandler = ResourceTouchHandler.Instance;
    }

    private void PlacedOnStart()
    {
        SetDragging(false);
    }

    public void SetDragging(bool newValue)
    {
        isBeingDragged = newValue;

        if (!isBeingDragged) //chama quando ele � largado
        {
            SnapToGrid();

            for(int i = 0; i < resourceToGather.Length; i++)
            {
                resourcesInRange[i] = GetResourcesInRange(resourceToGather[i], i);
                //Debug.Log("Chamou com: " + resourceToGather[i]);

            }

            if(!isDestroying)
                GameEvents.OnConstructionPlaced(cType);

            if(isBeingDragged)
                SetDragging(false);
        }
        else if (currentCell != null && isBeingDragged && !currentCell.isAvailable) //chamado quando ele tem um celula por�m � retirado dessa c�lula
        {
            currentCell.SetAvailability(true);
            Debug.Log("Levantou");

            touchHandler.RaiseTrash(this.gameObject);

            if (cellCollected.Count > 0)
            {
                foreach (GridCell cell in cellCollected)
                {
                    cell.SetColectability(true);
                }
                cellCollected.Clear();
            }
         
            for (int i = 0; i < resourceToGather.Length; i++)
            {
                int subtractAmount = -resourcesInRange[i];

                if (subtractAmount != 0)
                {
                    //Debug.Log("Diff: "+ diff);
                    GameEvents.OnResourceGathered(resourceToGather[i], subtractAmount);
                }
                resourcesInRange[i] = 0;
            }

            for (int i = 0; i < currentCell.resource.Length; i++)
            {
                currentCell.SetResource(GameManager.GameResources.None, i);

            }

            GameEvents.OnSelectConstruction(AudioManager.SoundEffects.Select);
        }
    }
    /// <summary>
    /// Set Object X and Z position to the nearest Cell
    /// </summary>
    public void SnapToGrid()
    {
        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position, out currentCell, tileType);
        if(Vector3.Distance(currentCell.transform.position, this.transform.position) > 2f && currentCell != null)
        {
            RemoveConstruction();
            return;
        }

        currentCell.SetAvailability(false);
        for (int i = 0; i < currentCell.resource.Length; i++)
        {
            if (i < secondaryResource.Length)
                currentCell.SetResource(secondaryResource[i], i); //troca o recurso daquela celula para o recurso que produz para secundarias e terciarias

        }

        transform.position = new Vector3(cellPos.x, 0.5f, cellPos.z);
    }

    /// <summary>
    /// Iterate through all blocks in range and see how many resources there are
    /// </summary>
    /// <param name="resourceAmount">out the amount fo resources in range</param>
    public int GetResourcesInRange(GameManager.GameResources resourceToCheck, int index)
    {
        int resourceAmount = 0;
        //Debug.Log(resourcesInRange + "Resources In Range");

        GridCell[] cellsInRadius = GridManager.Instance.GetCellFromRadius(transform.position, gatherRadius);

        //Debug.Log("Tamanho: " + cellsInRadius.Length);

        foreach (GridCell cell in cellsInRadius)
        {
            //Debug.Log("FOREACH: " + cell.resource);
            for (int i = 0; i < cell.resource.Length; i++)
            {
                //Debug.Log("FOR: "+cell.resource[i]);
                if (cell.resource[i] == resourceToCheck && cell.isColectible)
                {
                    //Debug.Log("RESOURCE CHECK: "+cell.resource[i]);                   
                    resourceAmount++;
                    cell.SetColectability(false);
                    cellCollected.Add(cell);
                    //Debug.Log("Coletou: " + cell.resource[i]);
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

    public void ReCheckResources(AudioManager.SoundEffects a)//FALTA TESTE
    {
        if (!isBeingDragged)
        {
            foreach(GridCell cell in cellCollected)
            {
                cell.SetColectability(true);
            }
            Debug.Log(gameObject.name + " Recheck");
            for (int i = 0; i < resourceToGather.Length; i++)
            {
                resourcesInRange[i] = GetResourcesInRange(resourceToGather[i], i);
                //Debug.Log("Chamou com: " + resourceToGather[i]);
            }
        }
    }

    public void RemoveConstruction()
    {
        isDestroying = true;

        for (int i = 0; i < currentCell.resource.Length; i++)
            currentCell.SetResource(GameManager.GameResources.None, i);

        currentCell.SetAvailability(true);
        if (cellCollected.Count > 0)
        {
            foreach (GridCell cell in cellCollected)
            {
                cell.SetColectability(true);
            }

            cellCollected.Clear();
        }

        for (int i = 0; i < resourceToGather.Length; i++)
        {
            int subtractAmount = -resourcesInRange[i];

            if (subtractAmount != 0)
            {
                //Debug.Log("Diff: "+ diff);
                GameEvents.OnResourceGathered(resourceToGather[i], subtractAmount);
            }
            resourcesInRange[i] = 0;
        }

        for (int i = 0; i < currentCell.resource.Length; i++)
        {
            currentCell.SetResource(GameManager.GameResources.None, i);
        }

        GameEvents.OnConstructionRemoved(cType);

        Destroy(gameObject);
    }
}
