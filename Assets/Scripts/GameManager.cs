using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject panelStartBattle;
    public GameObject panelLifeMonsters;

    public Transform trainerTransform;
    public Transform battlePointTransform;

    [Header("Game Over")]
    public GameObject panelGameOver;
    [SerializeField] GameObject winText;
    [SerializeField] GameObject looseText;
    public bool gameFinished;

    public bool isBattle = false;
    public bool battleStarted = false;

    public bool isWinBattle = false; 

    public Transform playerLocation;
    public Transform enemyLocation;
    public GameObject playerMonster;
    public GameObject enemyMonster;

    bool areSpawned = false;

    [Header("Slow Motion")]
    public float slowMotionFactor = 0.05f;
    public float slowMotionDuration = 0.08f;
    private bool isSlowMotionActive = false;

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
        if (isBattle && !areSpawned)
        {
            panelStartBattle.SetActive(true);
            panelLifeMonsters.SetActive(true);
            FindObjectOfType<TrainerMovement>().SetCanMove(false);
            Instantiate(playerMonster, playerLocation.position, Quaternion.identity);
            Instantiate(enemyMonster, enemyLocation.position, Quaternion.identity);
            areSpawned = true;           
        }

        if (isBattle)
            Camera.main.transform.position = new Vector3(battlePointTransform.position.x, battlePointTransform.position.y, -10);
        else
            Camera.main.transform.position = new Vector3(trainerTransform.position.x, trainerTransform.position.y, -10);
    }

    void ActivateSlowMotion()
    {
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = Time.fixedDeltaTime * slowMotionFactor;
        isSlowMotionActive = true;
    }

    void RestoreNormalSpeed()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.fixedDeltaTime / slowMotionFactor;
        isSlowMotionActive = false;
        slowMotionDuration = 0.08f;
    }

    public void PlayerWins()
    {
        panelLifeMonsters.SetActive(false);
        panelGameOver.SetActive(true);
        winText.SetActive(true);
        gameFinished = true;
    }

    public void PlayerLoose()
    {
        panelLifeMonsters.SetActive(false);
        looseText.SetActive(true);
        gameFinished = true;
        StartCoroutine(StartSlow());
    }

    public void GameOver()
    {
        panelLifeMonsters.SetActive(false);
        Time.timeScale = 0f;
        isBattle = false;
        areSpawned = false;
        StartCoroutine(StartSlow());
    }

    IEnumerator StartSlow()
    {
        ActivateSlowMotion();
        yield return new WaitForSeconds(slowMotionDuration);
        panelGameOver.SetActive(true);
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

    public void ExitGame()
    {
        Application.Quit();
    }
}
