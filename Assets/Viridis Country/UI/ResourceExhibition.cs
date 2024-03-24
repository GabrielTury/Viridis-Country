using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ResourceExhibition : MonoBehaviour
{
    // precisa de uma lista de recursos que o player tem que pegar, provavelmente vindo do game manager
    // mas por enquanto deixa isso de exemplo

    private int[] GameResourcesToGather = { 0, 1, 0, 0 }; // 0 = None, 1 = Water, 2 = Wood, 3 = Stone

    [SerializeField]
    private Image resourceBackground;

    [SerializeField]
    private GameObject gameManager;

    [SerializeField]
    private Sprite[] resourceIcons;

    [SerializeField]
    private List<GameObject> resourceBoxes;

    [SerializeField]
    private GameObject resourceBoxTemplate;

    [SerializeField]
    private List<int> resourceNames;

    private string[] resourceNamesText = { "None", "Water", "Wood", "Stone" };

    private List<Image> resourceBoxIcon;
    private List<TextMeshProUGUI> resourceBoxText;

    private int resourceBarSize = 0;

    private List<int[]> resourceBoxPositions = new List<int[]>
    {
        new int[] { 0, 0 },
        new int[] { -40, 0, 40, 0},
        new int[] { -80, 0, 0, 0, 80, 0},
        new int[] { -120, 0, -40, 0, 40, 0, 120, 0}
    };

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance.gameObject;

        for (int i = 0; i < GameResourcesToGather.Length; i++)
        {
            if (GameResourcesToGather[i] > 0) {
                resourceBarSize += 1;
                var clone = Instantiate(resourceBoxTemplate);
                clone.name = "ResourceBox" + i;
                resourceBoxes.Add(clone);
                resourceNames.Add(i);
            }
        }

        //Destroy(resourceBoxes[0]);

        var j = 0;
        for (int i = 0; i < resourceBarSize; i++)
        {
            resourceBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(resourceBoxPositions[resourceBarSize][i + j], resourceBoxPositions[resourceBarSize][i + 1 + j]);

            resourceBoxIcon.Add(GetChildWithName(resourceBoxes[i], "ResourceIcon").GetComponent<Image>());
            resourceBoxText.Add(GetChildWithName(resourceBoxes[i], "ResourceNumber").GetComponent<TextMeshProUGUI>());
            resourceBoxIcon[i].sprite = resourceIcons[resourceNames[i]];
            resourceBoxText[i].text = resourceNamesText[resourceNames[i]];
            j++;
        }

        resourceBackground.rectTransform.sizeDelta = new Vector2(80 * resourceBarSize, 40);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
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


