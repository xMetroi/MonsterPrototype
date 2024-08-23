using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject panelStartBattle;

    public GameObject panelGameOver;

    private bool isBattle = false;

    public bool isWinBattle = false; 


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

        if (isBattle)
        {
            panelStartBattle.SetActive(true);
        }
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
        Time.timeScale = 0f;
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
