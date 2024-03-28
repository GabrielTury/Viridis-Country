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
    // precisa de uma lista de recursos que o player tem que pegar, provavelmente vindo do game manager
    // mas por enquanto deixa isso de exemplo

    private int[] GameResourcesToGather = { 0, 4, 2, 2 }; // 0 = None, 1 = Water, 2 = Wood, 3 = Stone

    private VariableContainer[] resourceRedirector;

    private Dictionary<GameResources, int> resourceDict = new Dictionary<GameResources, int>();

    [SerializeField]
    private Image resourceBackground;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Sprite[] resourceIcons;

    [SerializeField]
    private List<GameObject> resourceBoxes;
    private Coroutine resourceBoxInflation;

    [SerializeField]
    private GameObject resourceBoxHighlight;

    [SerializeField]
    private GameObject resourceBoxTemplate;

    [SerializeField]
    private List<int> resourceNames;

    private string[] resourceNamesText = { "None", "Water", "Wood", "Stone" };
    private int prevWater = 0, prevWood = 0, prevStone = 0;

    [SerializeField]
    private Color[] resourceForegroundColors =
    {
        //new Color(255, 255, 255, 255), // None
        //new Color(148, 227, 255, 255), // Water
        //new Color(255, 187, 147, 255), // Wood
        //new Color(153, 167, 180, 255), // Stone
    }; // Por algum motivo, definir as cores pelo codigo esta crashando a Unity. Definir as cores pelo inspetor

    private List<Image> resourceBoxIcon = new List<Image>();
    private List<TextMeshProUGUI> resourceBoxText = new List<TextMeshProUGUI>();

    private int resourceBarSize = 0;

    private List<int[]> resourceBoxPositions = new List<int[]>
    {
        new int[] { 0, 0 },
        new int[] { 0, 0 },
        new int[] { -120, 0, 120, 0},
        new int[] { -240, 0, 0, 0, 240, 0},
        new int[] { -360, 0, -120, 0, 120, 0, 360, 0}
    };

    // Start is called before the first frame update
    void Start()
    {
        resourceBoxInflation = StartCoroutine(EmptyCoroutine());

        gameManager = GameManager.Instance;

        resourceRedirector = new VariableContainer[]
        {
            new VariableContainer(() => gameManager.zeroAmount, value => gameManager.zeroAmount = value),
            new VariableContainer(() => gameManager.waterAmount, value => gameManager.waterAmount = value),
            new VariableContainer(() => gameManager.woodAmount, value => gameManager.woodAmount = value),
            new VariableContainer(() => gameManager.waterAmount, value => gameManager.waterAmount = value),
        };

        resourceDict.Add(GameResources.None, 0);
        resourceDict.Add(GameResources.Water, resourceRedirector[1].GetValue());
        resourceDict.Add(GameResources.Wood, resourceRedirector[2].GetValue());
        resourceDict.Add(GameResources.Stone, resourceRedirector[3].GetValue());

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

        var j = 0;
        for (int i = 0; i < resourceBarSize; i++)
        {
            resourceBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(resourceBoxPositions[resourceBarSize][i + j], resourceBoxPositions[resourceBarSize][i + 1 + j]);

            resourceBoxIcon.Add(GetChildWithName(resourceBoxes[i], "ResourceIcon").GetComponent<Image>());
            resourceBoxText.Add(GetChildWithName(resourceBoxes[i], "ResourceNumber").GetComponent<TextMeshProUGUI>());
            resourceBoxIcon[i].sprite = resourceIcons[resourceNames[i]-1];
            resourceBoxText[i].text = resourceNamesText[resourceNames[i]];
            
            j++;
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
            case GameResources.Water: return 1;
            case GameResources.Wood: return 2;
            case GameResources.Stone: return 3;
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
        resourceDict[GameResources.Water] = resourceRedirector[1].GetValue();
        resourceDict[GameResources.Wood] = resourceRedirector[2].GetValue();
        resourceDict[GameResources.Stone] = resourceRedirector[3].GetValue();
    }

    private void UpdateUIInfo(GameResources resource, int amount)
    {
        var prevResource = resourceDict[resource];

        UpdateResourceDict();

        int boxIndex = GetBoxIndex(resource);

        resourceBoxText[boxIndex].text = resourceDict[resource] + "/" + GameResourcesToGather[RedirectResourceGatherList(resource)];
        
        string resourceChange = resourceDict[resource] > prevResource ? "+" + (resourceDict[resource] - prevResource) : (resourceDict[resource] - prevResource).ToString();

        GetChildWithName(resourceBoxes[boxIndex], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[boxIndex]];

        if (resourceDict[resource] > prevResource)
        {
            BoxUpdate(boxIndex, resourceChange, true);
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

        StartCoroutine(HighlightBox(GetChildWithName(boxHighlight, "ResourceForeground").GetComponent<Image>(), 0.31f, boxHighlight));

        StartCoroutine(FadeText(GetChildWithName(boxHighlight, "ResourceNumber").GetComponent<TextMeshProUGUI>(), 0.3f));

        StartCoroutine(InflateBoxXY(boxHighlight.GetComponent<Image>(), new Vector2(1.05f, 1.05f), 0.15f));
        StartCoroutine(InflateBoxXY(resourceBoxes[index].GetComponent<Image>(), new Vector2(1.05f, 1.05f), 0.15f));
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

    private IEnumerator HighlightBox(Image image, float time, GameObject clone)
    {
        float lerp = 0;
        float smoothLerp = 0;
        Color32 prevColor = new Color32(255, 255, 255, 255);
        while (lerp < 1 && time > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / time);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            image.color = Color.Lerp(prevColor, new Color32(255, 255, 255, 0), smoothLerp);
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

    private IEnumerator EmptyCoroutine()
    {
        yield return null;
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


