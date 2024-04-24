using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

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
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, jsonData);
    }

    public PlayerData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }
}
