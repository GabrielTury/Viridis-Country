using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static UnityEngine.Rendering.DebugUI;

public class ResourceExhibition : MonoBehaviour
{
    // precisa de uma lista de recursos que o player tem que pegar, provavelmente vindo do game manager
    // mas por enquanto deixa isso de exemplo

    private int[] GameResourcesToGather = { 0, 4, 2, 2 }; // 0 = None, 1 = Water, 2 = Wood, 3 = Stone

    [SerializeField]
    private Image resourceBackground;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Sprite[] resourceIcons;

    [SerializeField]
    private List<GameObject> resourceBoxes;

    [SerializeField]
    private GameObject resourceBoxTemplate;

    [SerializeField]
    private List<int> resourceNames;

    private string[] resourceNamesText = { "None", "Water", "Wood", "Stone" };

    [SerializeField]
    private Color[] resourceForegroundColors =
    {
        //new Color(255, 255, 255, 255), // None
        //new Color(148, 227, 255, 255), // Water
        //new Color(255, 187, 147, 255), // Wood
        //new Color(153, 167, 180, 255), // Stone
    };

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
        gameManager = GameManager.Instance;

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

            Debug.Log(GetChildWithName(resourceBoxes[i], "ResourceIcon").GetComponent<Image>());
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

    private void UpdateUIInfo(GameResources resource, int amount)
    {
        for (int i = 0; i < resourceBarSize; i++)
        {
            switch (resourceNamesText[resourceNames[i]])
            {
                case "Water":
                    resourceBoxText[i].text = gameManager.waterAmount + "/" + GameResourcesToGather[1];
                    GetChildWithName(resourceBoxes[i], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[i]];
                    break;

                case "Wood":
                    resourceBoxText[i].text = gameManager.woodAmount + "/" + GameResourcesToGather[2];
                    GetChildWithName(resourceBoxes[i], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[i]];
                    break;

                case "Stone":
                    resourceBoxText[i].text = gameManager.stoneAmount + "/" + GameResourcesToGather[3];
                    GetChildWithName(resourceBoxes[i], "ResourceForeground").GetComponent<Image>().color = resourceForegroundColors[resourceNames[i]];
                    break;

            }
        }
    }

    private void OnEnable()
    {
        GameEvents.Resource_Gathered += UpdateUIInfo;
    }

    private void OnDisable()
    {
        GameEvents.Resource_Gathered -= UpdateUIInfo;
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


