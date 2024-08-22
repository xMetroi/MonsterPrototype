using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<int> monstersIds = new List<int>();
}

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    private string filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persistir entre escenas
        }
    }

    private void Start()
    {
        filePath = Application.persistentDataPath + "/playerData.json";
        LoadPlayerData();
    }

    private void OnDestroy()
    {
       TrainerController trainer = GameObject.FindAnyObjectByType<TrainerController>();
        SavePlayerData(trainer.GetAllMonstersById(), trainer.GetCoins());
    }

    public void SavePlayerData(List<int> monstersIds, int coins)
    {
        PlayerData data = new PlayerData { monstersIds = monstersIds};
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    private PlayerData LoadPlayerData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogError("No se encontró el archivo de datos: " + filePath);
            return null;
        }
    }

    public List<int> LoadPlayerMonster()
    {
        PlayerData data = LoadPlayerData();
        if (data != null)
        {
            return data.monstersIds;
        }
        else
        {
            return null;
        }
    }

    public List<Monster> LoadAllMonstersFromAssets()
    {
        // Encuentra todos los assets de tipo Monster en la carpeta "Assets/>Monsters"
        string[] guids = AssetDatabase.FindAssets("t:Monster", new[] { "Assets/Monsters" });
        List<Monster> monsters = new List<Monster>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Monster monster = AssetDatabase.LoadAssetAtPath<Monster>(path);
            if (monster != null)
            {
                monsters.Add(monster);
                Debug.Log("Aa: " + monster.name);
            }
        }

        return monsters;
    }
}