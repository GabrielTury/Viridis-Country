using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using Newtonsoft.Json;
using UnityEditor;
[DefaultExecutionOrder(-1)]
public class SaveManager : MonoBehaviour
{
    private string savePath;
    private string saveData;

    void Awake()
    {
        saveData = Path.Combine(Application.persistentDataPath, "SaveData");
        savePath = Path.Combine(saveData, "save.json");

        // Verificar se o diretório já existe, se não, criá-lo
        if (!Directory.Exists(saveData))
        {
            Directory.CreateDirectory(saveData);
        }
    }

    public void SaveGame(PlayerData data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        File.WriteAllText(savePath, jsonData);

        
    }

    public PlayerData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            return JsonConvert.DeserializeObject<PlayerData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }
}
