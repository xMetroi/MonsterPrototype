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
        try
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
        catch (System.Exception ex)
        {
            Debug.LogError("Error en Awake: " + ex.Message);
        }
    }

    private void Start()
    {
        try
        {
            filePath = Application.persistentDataPath + "/playerData.json";
            LoadPlayerData();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en Start: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        try
        {
            TrainerController trainer = GameObject.FindAnyObjectByType<TrainerController>();
            SavePlayerData(trainer.GetAllMonstersById());
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al destruir el objeto: " + ex.Message);
        }
    }

    public void SavePlayerData(List<int> monstersIds)
    {
        try
        {
            PlayerData data = new PlayerData { monstersIds = monstersIds };
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al guardar los datos del jugador: " + ex.Message);
        }
    }

    private PlayerData LoadPlayerData()
    {
        try
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
        catch (System.Exception ex)
        {
            Debug.LogError("Error al cargar los datos del jugador: " + ex.Message);
            return null;
        }
    }

    public List<int> LoadPlayerMonster()
    {
        try
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
        catch (System.Exception ex)
        {
            Debug.LogError("Error al cargar los IDs de los monstruos del jugador: " + ex.Message);
            return null;
        }
    }

    public List<Monster> LoadAllMonstersFromAssets()
    {
        /*try
        {
            // Encuentra todos los assets de tipo Monster en la carpeta "Assets/Monsters"
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
        catch (System.Exception ex)
        {
            Debug.LogError("Error al cargar todos los monstruos desde los assets: " + ex.Message);
            return new List<Monster>();
        }*/
        return null;
    }
}