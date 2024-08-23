using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject panelStartBattle;

    public GameObject panelGameOver;

    public bool isBattle = false;

    public bool isWinBattle = false; 

    public Transform playerLocation;
    public Transform enemyLocation;
    public GameObject playerMonster;
    public GameObject enemyMonster;

    bool areSpawned = false;

    [SerializeField] GameObject cameraFight;
    [SerializeField] GameObject cameraWorld;


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
    }

    private void Update()
    {
        if (panelGameOver == null)
        {
            panelGameOver = FindInactiveGameObjectByName("PanelGameOver");
        }

        if (isBattle && !areSpawned)
        {
            panelStartBattle.SetActive(true);

            FindObjectOfType<TrainerMovement>().SetCanMove(false);
            Instantiate(playerMonster, playerLocation.position, Quaternion.identity);
            Instantiate(enemyMonster, enemyLocation.position, Quaternion.identity);
            areSpawned = true;

            cameraWorld.SetActive(false);
            cameraFight.SetActive(true);

           
        }
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
        Time.timeScale = 0f;
        isBattle = false;
        areSpawned = false;

        cameraWorld.SetActive(true);
        cameraFight.SetActive(false);
    }

    GameObject FindInactiveGameObjectByName(string name)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    public void SetIsBattle(bool isBattle)
    {
        this.isBattle = isBattle;
    }

    public bool IsBattle()
    {
        return isBattle;
    }
}
