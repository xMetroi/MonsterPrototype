using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<int> monstersIds = new List<int>();
    public Vector3 playerPosition;
    public int enemiesDefeated;
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
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
            DontDestroyOnLoad(gameObject);
        }        

        filePath = Application.persistentDataPath + "/playerData.json";
    }

    private void OnDestroy()
    {
        SaveDataBeforeExit();
    }

    private void OnApplicationQuit()
    {
        SaveDataBeforeExit();        
    }

    private void SaveDataBeforeExit()
    {
        try
        {
            TrainerController trainer = GameObject.FindAnyObjectByType<TrainerController>();
            // -------------------------Modifica esta linea para pasar la posicion del jugador y los enemigos derrotados--------------------------------------------
            Vector3 playerPosition = trainer.transform.position;
            int enemiesDefeated = 0;
            // ----------------------------------------------------------------------------------------------------------

            List<int> monstersIds = trainer.GetAllMonstersById();

            SavePlayerData(monstersIds, playerPosition, enemiesDefeated);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al destruir el objeto: " + ex.Message);
        }
    }

    public void SavePlayerData(List<int> monstersIds, Vector3 playerPosition, int enemiesDefeated)
    {
        Debug.Log("Path: " + filePath);
        try
        {
            PlayerData data = new PlayerData { monstersIds = monstersIds, playerPosition = playerPosition, enemiesDefeated = enemiesDefeated };
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
        PlayerData data = LoadPlayerData();
        return data?.monstersIds ?? new List<int>();
    }

    public Vector3 LoadPlayerPosition()
    {
        PlayerData data = LoadPlayerData();
        return data?.playerPosition ?? Vector3.zero;
    }

    public int LoadEnemiesDefeated()
    {
        PlayerData data = LoadPlayerData();
        return data?.enemiesDefeated ?? 0;
    }

    public List<Monster> LoadAllMonstersFromAssets()
    {
        try
        {
            // Cargar todos los assets de tipo Monster desde la carpeta "Resources/Monsters"
            Monster[] monsters = Resources.LoadAll<Monster>("Monsters");
            return new List<Monster>(monsters);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al cargar todos los monstruos desde los assets: " + ex.Message);
            return new List<Monster>();
        }
    }

}