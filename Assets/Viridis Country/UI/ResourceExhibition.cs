using GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static UnityEngine.Rendering.DebugUI;

class VariableContainer
{
    private readonly Func<int> _getValue;
    private readonly Action<int> _setValue;

    public VariableContainer(Func<int> getValue, Action<int> setValue)
    {
        _getValue = getValue;
        _setValue = setValue;
    }

    public int GetValue() => _getValue();
    public void SetValue(int value) => _setValue(value);
}

public class ResourceExhibition : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    // precisa de uma lista de recursos que o player tem que pegar, provavelmente vindo do game manager
    // mas por enquanto deixa isso de exemplo
    // edit: ja tem no gamamanager, mas preferi deixar aqui por motivos de organização

    private int[] GameResourcesToGather = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // veja lista dos nomes abaixo
    // atualiza no start

    private VariableContainer[] resourceRedirector;

    private Dictionary<GameResources, int> resourceDict = new Dictionary<GameResources, int>();

    [SerializeField]
    private Image resourceBackground;

    [SerializeField]
    private Sprite[] resourceIcons;

    [SerializeField]
    private List<GameObject> resourceBoxes;

    [SerializeField]
    private GameObject resourceBoxHighlight;

    [SerializeField]
    private GameObject resourceBoxTemplate;

    [SerializeField]
    private List<int> resourceNames;

    private string[] resourceNamesText = { "None", "Wood", "Plank", "Stone", "ProcessedStone", "ConstructionMaterials", "Water", "Milk", "FermentedMilk", "Cheese", "Skin", "Leather", "Wool", "Cloth", "Clothes", "Wheat", "Flour", "Bread", "Gold" };

    [SerializeField]
    private Color[] resourceForegroundColors = {}; // Por algum motivo, definir as cores pelo codigo esta crashando a Unity. Definir as cores pelo inspetor

    private List<TextMeshProUGUI> resourceBoxText = new List<TextMeshProUGUI>();

    private int resourceBarSize = 0;

    [SerializeField]
    private GameObject pauseTemplate;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        GameResourcesToGather[0] = 0;
        GameResourcesToGather[1] = gameManager.objectiveWood;
        GameResourcesToGather[2] = gameManager.objectivePlank;
        GameResourcesToGather[3] = gameManager.objectiveStone;
        GameResourcesToGather[4] = gameManager.objectiveProcessedStone;
        GameResourcesToGather[5] = gameManager.objectiveConstructionMaterials;
        GameResourcesToGather[6] = gameManager.objectiveWater;
        GameResourcesToGather[7] = gameManager.objectiveMilk;
        GameResourcesToGather[8] = gameManager.objectiveFermentedMilk;
        GameResourcesToGather[9] = gameManager.objectiveCheese;
        GameResourcesToGather[10] = gameManager.objectiveSkin;
        GameResourcesToGather[11] = gameManager.objectiveLeather;
        GameResourcesToGather[12] = gameManager.objectiveWool;
        GameResourcesToGather[13] = gameManager.objectiveCloth;
        GameResourcesToGather[14] = gameManager.objectiveClothes;
        GameResourcesToGather[15] = gameManager.objectiveWheat;
        GameResourcesToGather[16] = gameManager.objectiveFlour;
        GameResourcesToGather[17] = gameManager.objectiveBread;
        GameResourcesToGather[18] = gameManager.objectiveGold;

        resourceRedirector = new VariableContainer[]
        {
            new VariableContainer(() => gameManager.zeroAmount, value => gameManager.zeroAmount = value),
            new VariableContainer(() => gameManager.woodAmount, value => gameManager.woodAmount = value),
            new VariableContainer(() => gameManager.plankAmount, value => gameManager.plankAmount = value),
            new VariableContainer(() => gameManager.stoneAmount, value => gameManager.stoneAmount = value),
            new VariableContainer(() => gameManager.processedStoneAmount, value => gameManager.processedStoneAmount = value),
            new VariableContainer(() => gameManager.constructionMaterialsAmount, value => gameManager.constructionMaterialsAmount = value),
            new VariableContainer(() => gameManager.waterAmount, value => gameManager.waterAmount = value),
            new VariableContainer(() => gameManager.milkAmount, value => gameManager.milkAmount = value),
            new VariableContainer(() => gameManager.fermentedMilkAmount, value => gameManager.fermentedMilkAmount = value),
            new VariableContainer(() => gameManager.cheeseAmount, value => gameManager.cheeseAmount = value),
            new VariableContainer(() => gameManager.skinAmount, value => gameManager.skinAmount = value),
            new VariableContainer(() => gameManager.leatherAmount, value => gameManager.leatherAmount = value),
            new VariableContainer(() => gameManager.woolAmount, value => gameManager.woolAmount = value),
            new VariableContainer(() => gameManager.clothAmount, value => gameManager.clothAmount = value),
            new VariableContainer(() => gameManager.clothesAmount, value => gameManager.clothesAmount = value),
            new VariableContainer(() => gameManager.wheatAmount, value => gameManager.wheatAmount = value),
            new VariableContainer(() => gameManager.flourAmount, value => gameManager.flourAmount = value),
            new VariableContainer(() => gameManager.breadAmount, value => gameManager.breadAmount = value),
            new VariableContainer(() => gameManager.goldAmount, value => gameManager.goldAmount = value),
        };

        resourceDict.Add(GameResources.None, 0);
        resourceDict.Add(GameResources.Wood, resourceRedirector[1].GetValue());
        resourceDict.Add(GameResources.Plank, resourceRedirector[2].GetValue());
        resourceDict.Add(GameResources.Stone, resourceRedirector[3].GetValue());
        resourceDict.Add(GameResources.ProcessedStone, resourceRedirector[4].GetValue());
        resourceDict.Add(GameResources.ConstructionMaterials, resourceRedirector[5].GetValue());
        resourceDict.Add(GameResources.Water, resourceRedirector[6].GetValue());
        resourceDict.Add(GameResources.Milk, resourceRedirector[7].GetValue());
        resourceDict.Add(GameResources.FermentedMilk, resourceRedirector[8].GetValue());
        resourceDict.Add(GameResources.Cheese, resourceRedirector[9].GetValue());
        resourceDict.Add(GameResources.Skin, resourceRedirector[10].GetValue());
        resourceDict.Add(GameResources.Leather, resourceRedirector[11].GetValue());
        resourceDict.Add(GameResources.Wool, resourceRedirector[12].GetValue());
        resourceDict.Add(GameResources.Cloth, resourceRedirector[13].GetValue());
        resourceDict.Add(GameResources.Clothes, resourceRedirector[14].GetValue());
        resourceDict.Add(GameResources.Wheat, resourceRedirector[15].GetValue());
        resourceDict.Add(GameResources.Flour, resourceRedirector[16].GetValue());
        resourceDict.Add(GameResources.Bread, resourceRedirector[17].GetValue());
        resourceDict.Add(GameResources.Gold, resourceRedirector[18].GetValue());

        for (int i = 0; i < GameResourcesToGather.Length; i++)
        {
            if (GameResourcesToGather[i] > 0) {
                resourceBarSize += 1;
                var clone = Instantiate(resourceBoxTemplate, this.transform);
                clone.name = "ResourceBox" + i;
                resourceBoxes.Add(clone);
                resourceNames.Add(i);
            }
        }

        int maxBoxesPerLine = 3;
        int numRows = (int)Mathf.Ceil((float)resourceBarSize / maxBoxesPerLine);
        int totalBoxes = Mathf.Min(resourceBarSize, maxBoxesPerLine * numRows);

        float totalWidth = maxBoxesPerLine * 240;
        float totalHeight = numRows * 100;

        float startX = -totalWidth / 2 + 120; // Meio da primeira caixa

        for (int i = 0; i < resourceBarSize; i++)
        {
            int row = i / maxBoxesPerLine;
            int col = i % maxBoxesPerLine;

            float xOffset = startX + (240 * col);
            float yOffset = -((totalHeight / 2) - (100 * numRows / 2) + (100 * row));

            resourceBoxes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, yOffset);

            /*if (i < 3)
            {
                resourceBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(240 - (240 * (Mathf.Min(resourceBarSize, 3) - 1)) / 2 + (240 * (i - 1)), 0);
            } else
            {
                resourceBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(240 - (240 * (resourceBarSize - 1)) / 2 + (240 * (i - 1)), -100);
            }*/


            GetChildWithName(resourceBoxes[i], "ResourceIcon").GetComponent<Image>().sprite = resourceIcons[resourceNames[i] - 1];

            resourceBoxText.Add(GetChildWithName(resourceBoxes[i], "ResourceNumber").GetComponent<TextMeshProUGUI>());

            resourceBoxText[i].text = resourceNamesText[resourceNames[i]];
        }

        

        resourceBackground.rectTransform.sizeDelta = new Vector2(280 * resourceBarSize, 130);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private int RedirectResourceGatherList(GameResources resource)
    {
        switch (resource)
        {
            case GameResources.None: return 0;
            case GameResources.Wood: return 1;
            case GameResources.Plank: return 2;
            case GameResources.Stone: return 3;
            case GameResources.ProcessedStone: return 4;
            case GameResources.ConstructionMaterials: return 5;
            case GameResources.Water: return 6;
            case GameResources.Milk: return 7;
            case GameResources.FermentedMilk: return 8;
            case GameResources.Cheese: return 9;
            case GameResources.Skin: return 10;
            case GameResources.Leather: return 11;
            case GameResources.Wool: return 12;
            case GameResources.Cloth: return 13;
            case GameResources.Clothes: return 14;
            case GameResources.Wheat: return 15;
            case GameResources.Flour: return 16;
            case GameResources.Bread: return 17;
            case GameResources.Gold: return 18;
            default: return 0;
        }
    }

    private int GetBoxIndex(GameResources resource)
    {
        for (int i = 0; resourceNames.Count > i; i++)
        {
            if (resourceNames[i] == RedirectResourceGatherList(resource))
            {
                return i;
            }
        }
        return 0;
    }

    private void UpdateResourceDict()
    {
        resourceDict[GameResources.None] = 0;
        resourceDict[GameResources.Wood] = resourceRedirector[1].GetValue();
        resourceDict[GameResources.Plank] = resourceRedirector[2].GetValue();
        resourceDict[GameResources.Stone] = resourceRedirector[3].GetValue();
        resourceDict[GameResources.ProcessedStone] = resourceRedirector[4].GetValue();
        resourceDict[GameResources.ConstructionMaterials] = resourceRedirector[5].GetValue();
        resourceDict[GameResources.Water] = resourceRedirector[6].GetValue();
        resourceDict[GameResources.Milk] = resourceRedirector[7].GetValue();
        resourceDict[GameResources.FermentedMilk] = resourceRedirector[8].GetValue();
        resourceDict[GameResources.Cheese] = resourceRedirector[9].GetValue();
        resourceDict[GameResources.Skin] = resourceRedirector[10].GetValue();
        resourceDict[GameResources.Leather] = resourceRedirector[11].GetValue();
        resourceDict[GameResources.Wool] = resourceRedirector[12].GetValue();
        resourceDict[GameResources.Cloth] = resourceRedirector[13].GetValue();
        resourceDict[GameResources.Clothes] = resourceRedirector[14].GetValue();
        resourceDict[GameResources.Wheat] = resourceRedirector[15].GetValue();
        resourceDict[GameResources.Flour] = resourceRedirector[16].GetValue();
        resourceDict[GameResources.Bread] = resourceRedirector[17].GetValue();
        resourceDict[GameResources.Gold] = resourceRedirector[18].GetValue();
    }

    private void UpdateUIInfo(GameResources resource, int amount)
    {
        var prevResource = resourceDict[resource];

        UpdateResourceDict();

        int boxIndex = GetBoxIndex(resource);

        resourceBoxText[boxIndex].text = resourceDict[resource] + "/" + GameResourcesToGather[RedirectResourceGatherList(resource)];
        
        string resourceChange = resourceDict[resource] > prevResource ? "+" + (resourceDict[resource] - prevResource) : (resourceDict[resource] - prevResource).ToString();

        GetChildWithName(resourceBoxes[boxIndex], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[boxIndex]];

        if (resourceDict[resource] > prevResource || resourceDict[resource] < prevResource)
        {
            BoxUpdate(boxIndex, resourceChange, resourceDict[resource] > prevResource ? true : false);
        }

        /*
        for (int i = 0; i < resourceBarSize; i++)
        {
            switch (resourceNamesText[resourceNames[i]])
            {
                case "Water":
                    resourceBoxText[i].text = gameManager.waterAmount + "/" + GameResourcesToGather[1];
                    resourceChange = gameManager.waterAmount > prevWater ? "+" + (gameManager.waterAmount - prevWater) : (gameManager.waterAmount - prevWater).ToString();
                    GetChildWithName(resourceBoxes[i], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[i]];
                    
                    if (gameManager.waterAmount > prevWater)
                    {
                        BoxUpdate(i, resourceChange, true);
                    }
                    prevWater = gameManager.waterAmount;
                    break;

                case "Wood":
                    resourceBoxText[i].text = gameManager.woodAmount + "/" + GameResourcesToGather[2];
                    resourceChange = gameManager.woodAmount > prevWood ? "+" + (gameManager.woodAmount - prevWood) : (gameManager.woodAmount - prevWood).ToString();
                    GetChildWithName(resourceBoxes[i], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[i]];

                    if (gameManager.woodAmount > prevWood)
                    {
                        BoxUpdate(i, resourceChange, true);
                    }
                    prevWood = gameManager.woodAmount;

                    UpdateResourceDict();
                    Debug.Log("please " + resourceDict[GameResources.Wood].ToString());
                    break;

                case "Stone":
                    resourceBoxText[i].text = gameManager.stoneAmount + "/" + GameResourcesToGather[3];
                    resourceChange = gameManager.waterAmount > prevStone ? "+" + (gameManager.stoneAmount - prevStone) : (gameManager.stoneAmount - prevStone).ToString();
                    GetChildWithName(resourceBoxes[i], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[i]];

                    if (gameManager.stoneAmount > prevWood)
                    {
                        BoxUpdate(i, resourceChange, true);
                    }
                    prevStone = gameManager.stoneAmount;
                    break;

            }
        }
        */
    }

    private void OnEnable()
    {
        GameEvents.Resource_Gathered += UpdateUIInfo;
    }

    private void OnDisable()
    {
        GameEvents.Resource_Gathered -= UpdateUIInfo;
    }

    private void BoxUpdate(int index, string resourceChange, bool positive)
    {
        GameObject boxHighlight = Instantiate(resourceBoxHighlight, this.transform);

        boxHighlight.GetComponent<Image>().rectTransform.position = resourceBoxes[index].GetComponent<Image>().rectTransform.position;

        GetChildWithName(boxHighlight, "ResourceNumber").GetComponent<TextMeshProUGUI>().text = resourceChange != "+0" || resourceChange != "0" ? resourceChange : "";

        StartCoroutine(FadeText(GetChildWithName(boxHighlight, "ResourceNumber").GetComponent<TextMeshProUGUI>(), 0.3f));

        if (positive)
        {
            StartCoroutine(HighlightBox(GetChildWithName(boxHighlight, "ResourceForeground").GetComponent<Image>(), 0.31f, boxHighlight, new Color32(255, 255, 255, 0)));
            StartCoroutine(InflateBoxXY(boxHighlight.GetComponent<Image>(), new Vector2(1.05f, 1.05f), 0.15f));
            StartCoroutine(InflateBoxXY(resourceBoxes[index].GetComponent<Image>(), new Vector2(1.05f, 1.05f), 0.15f));
        }
        else
        {
            GetChildWithName(boxHighlight, "ResourceForeground").GetComponent<Image>().color = new Color32(0, 0, 0, 100);

            StartCoroutine(HighlightBox(GetChildWithName(boxHighlight, "ResourceForeground").GetComponent<Image>(), 0.31f, boxHighlight, new Color32(150, 20, 20, 0)));
            StartCoroutine(InflateBoxXY(boxHighlight.GetComponent<Image>(), new Vector2(0.98f, 0.98f), 0.15f));
            StartCoroutine(InflateBoxXY(resourceBoxes[index].GetComponent<Image>(), new Vector2(0.98f, 0.98f), 0.15f));
        }
        
    }

    public void PauseGame()
    {
        Instantiate(pauseTemplate);
    }

    private IEnumerator InflateBoxXY(Image image, Vector2 targetSize, float time)
    {
        Vector2 prevSize = image.rectTransform.localScale;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && time > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / time);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            image.rectTransform.localScale = Vector3.Lerp(prevSize, targetSize, smoothLerp);
            yield return null;
        }

        lerp = 0;
        smoothLerp = 0;

        while (lerp < 1 && time > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / time);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            image.rectTransform.localScale = Vector3.Lerp(targetSize, prevSize, smoothLerp);
            yield return null;
        }

        image.rectTransform.localScale = new Vector2(1, 1);
    }

    private IEnumerator HighlightBox(Image image, float time, GameObject clone, Color32 targetColor)
    {
        float lerp = 0;
        float smoothLerp = 0;
        Color32 prevColor = image.color;
        while (lerp < 1 && time > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / time);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            image.color = Color.Lerp(prevColor, targetColor, smoothLerp);
            yield return null;
        }

        Destroy(clone);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float time)
    {
        float lerp = 0;
        float smoothLerp = 0;
        Color32 prevColor = new Color32(255, 255, 255, 255);
        while (lerp < 1 && time > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / time);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            text.color = Color.Lerp(prevColor, new Color32(255, 255, 255, 0), smoothLerp);
            yield return null;
        }
    }

    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }
}


