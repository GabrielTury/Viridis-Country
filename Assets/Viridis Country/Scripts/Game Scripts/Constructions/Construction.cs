
using GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Construction : MonoBehaviour
{
    private ResourceTouchHandler touchHandler;

    private MeshRenderer meshRenderer;

    private bool isBeingDragged;

    public ConstructionTemplate construcion;

    private int gatherRadius;

    private int[] resourcesInRange;

    private GameManager.GameResources[] resourceToGather;

    [Tooltip("Atualmente tem o maximo de 2 recursos, tem que trocar no gridcell para alterar")]
    private GameManager.GameResources[] secondaryResource = new GameManager.GameResources[2];

    private GridCell currentCell;

    private GridCell previousCell;

    private GridCell.TileType tileType;

    private AudioManager.ConstructionAudioTypes cType;

    private List<GridCell> cellCollected = new List<GridCell>();

    private bool isDestroying = false;

    private bool placedOnStart = false;

    private GameObject canvas;

    [SerializeField]
    private Image renderImage;

    private Image renderClone;

    private Animator anim;

    private PlayableGraph playableGraph;


    private void OnEnable()
    {
        canvas = FindFirstObjectByType<Canvas>().gameObject;

        meshRenderer = GetComponent<MeshRenderer>();
        GetComponent<MeshFilter>().mesh = construcion.constructionMesh;
        meshRenderer.material = construcion.material;
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

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //SetDragging(false);
        touchHandler = ResourceTouchHandler.Instance;
    }

    public IEnumerator CheckFuturePosition()
    {
        while(isBeingDragged)
        {
            if(Vector3.Distance(GridManager.Instance.CheckNearestCell(this.transform.position, tileType), transform.position) > 0.8f)
            {
                if (meshRenderer.material.color != Color.red)
                    meshRenderer.material.color = Color.red;
                
                if(renderClone != null)
                {
                    renderClone.gameObject.SetActive(false);
                }
            }
            else if(Vector3.Distance(GridManager.Instance.CheckNearestCell(this.transform.position, tileType), transform.position) < 0.8f)
            {
                if(meshRenderer.material.color != Color.white)
                    meshRenderer.material.color = Color.white;
                if(renderClone == null)
                {
                    renderClone = Instantiate(renderImage, canvas.transform);
                    renderClone.sprite = construcion.renderSprite;
                }
                else if (!renderClone.gameObject.activeInHierarchy)
                {
                    renderClone.gameObject.SetActive(true);
                }
                Vector3 a = GridManager.Instance.CheckNearestCell(this.transform.position, tileType) + new Vector3(0.5f, 0, 0.5f);
                    renderClone.rectTransform.position = Camera.main.WorldToScreenPoint(a);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void PlacedOnStart()
    {
        placedOnStart = true; //tem que estar antes do Set Dragging
        SetDragging(false);
    }

    public void SetDragging(bool newValue)
    {
        isBeingDragged = newValue;

        if (!isBeingDragged) //chama quando ele é largado
        {
            ResourceTouchHandler.Instance.LowerTrash();

            if (ResourceTouchHandler.Instance.isOnTrash == true)
            {
                RemoveConstruction();
            }

/*            if (!isDestroying && !placedOnStart)
                GameEvents.OnConstructionPlaced(cType);
            else if (placedOnStart)
                placedOnStart = false;*/

            SnapToGrid();

            for(int i = 0; i < resourceToGather.Length; i++)
            {
                resourcesInRange[i] = GetResourcesInRange(resourceToGather[i], i);
                //Debug.Log("Chamou com: " + resourceToGather[i]);

            }

            Destroy(renderClone);

            if(isBeingDragged)
                SetDragging(false);
        }
        else if (currentCell != null && isBeingDragged && !currentCell.isAvailable) //chamado quando ele tem um celula porém é retirado dessa célula
        {
            currentCell.SetAvailability(true);
            //Debug.Log("Levantou");

            if (touchHandler)
            {
                touchHandler.RaiseTrash(this.gameObject);
            } else
            {
                touchHandler = ResourceTouchHandler.Instance;
                touchHandler.RaiseTrash(this.gameObject);
            }
            

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
        if(currentCell != null)
            previousCell = currentCell;

        Vector3 cellPos = GridManager.Instance.NearestCellPosition(transform.position, out currentCell, tileType);

        if(currentCell == null)
        {
            RemoveConstruction();
            return;
        }

        if(currentCell.tileType != tileType)
        {
            RemoveConstruction();
            return;
        }
        if(Vector3.Distance(currentCell.transform.position, this.transform.position) > 0.8f && currentCell != null)
        {
            RemoveConstruction();
            return;
        }

        if (!isDestroying && !placedOnStart)
            GameEvents.OnConstructionPlaced(cType);
        else if (placedOnStart)
            placedOnStart = false;

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
            PlayClip(anim, construcion.animClip);
            Debug.Log("Should Play animation");
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

        for (int i = 0; i < currentCell?.resource.Length; i++)
            currentCell?.SetResource(GameManager.GameResources.None, i);

        currentCell?.SetAvailability(true);
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

        for (int i = 0; i < currentCell?.resource.Length; i++)
        {
            currentCell?.SetResource(GameManager.GameResources.None, i);
        }

        GameEvents.OnConstructionRemoved(cType);

        Destroy(gameObject);
    }

    private void PlayClip(Animator animator, AnimationClip clip)
    {

        playableGraph = PlayableGraph.Create();

        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", anim);
        var mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);
        playableOutput.SetSourcePlayable(mixerPlayable);

        AnimatorControllerPlayable controllerPlayable = AnimatorControllerPlayable.Create(playableGraph, anim.runtimeAnimatorController);
        mixerPlayable.ConnectInput(0, controllerPlayable, 0);
        mixerPlayable.SetInputWeight(1, 1);


        AnimationClipPlayable playableClip = AnimationClipPlayable.Create(playableGraph, clip);
        mixerPlayable.ConnectInput(1, playableClip, 0);


        playableGraph.Play();
        Debug.Log(clip.name);
    }

    private void OnDestroy()
    {
        if(playableGraph.IsValid())
            playableGraph.Destroy();
    }
}
