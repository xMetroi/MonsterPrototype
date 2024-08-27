using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject panelStartBattle;
    public GameObject panelLifeMonsters;

    public Transform trainerTransform;
    public Transform battlePointTransform;

    [Header("Game Over")]
    public GameObject panelGameOver;
    [SerializeField] GameObject winText;
    [SerializeField] GameObject looseText;
    public bool gameFinished;

    public bool isInBattle = false;
    public bool battleStarted = false;

    public bool isWinBattle = false;

    [Header("Combat Site Properties")]
    [SerializeField] private Transform playerMonsterSpawn;
    [SerializeField] private Transform enemyMonsterSpawn;
    [SerializeField] private GameObject playerMonsterPrefab;
    [SerializeField] private GameObject enemyMonsterPrefab;

    [Header("Slow Motion")]
    public float slowMotionFactor = 0.05f;
    public float slowMotionDuration = 0.08f;
    private bool isSlowMotionActive = false;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //Events Subscriptions

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #region Getter / Setter

    public bool GetIsInBattle() { return isInBattle; }

    #endregion

    #region Scene Manager

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Intro")
        {
            LoadScene("WorldPROTOTYPEScene", 33);
        }

        if (scene.name == "WorldPROTOTYPEScene")
        {
            InitializeWorldVariables();
        }
    }

    #region Utilities

    /// <summary>
    /// Load a new scene
    /// </summary>
    /// <param name="sceneName"> name of the scene to load </param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a new scene with a delay
    /// </summary>
    /// <param name="sceneName"> name of the scene to load </param>
    /// <param name="seconds"> delay to load the scene </param>
    public void LoadScene(string sceneName, float seconds)
    {
        StartCoroutine(LoadSceneDelay(sceneName, seconds));
    }

    IEnumerator LoadSceneDelay(string sceneName, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #endregion

    #region Battle Manager

    /// <summary>
    /// We use this to initialize the variables we are gonna need in the world scene
    /// </summary>
    private void InitializeWorldVariables()
    {
        trainerTransform = GameObject.Find("Trainer").transform;
        battlePointTransform = GameObject.Find("Fight Camera Holder").transform;

        playerMonsterSpawn = GameObject.Find("SpawnPointPlayerMonster").transform;
        enemyMonsterSpawn = GameObject.Find("SpawnPointEnemyMonster").transform;
    }

    /// <summary>
    /// We use this method to start a battle
    /// </summary>
    public void StartBattle()
    {
        //panelStartBattle.SetActive(true);
        //panelLifeMonsters.SetActive(true);
        FindObjectOfType<TrainerMovement>().SetCanMove(false);

        Instantiate(playerMonsterPrefab, playerMonsterSpawn.position, Quaternion.identity);
        Instantiate(enemyMonsterPrefab, enemyMonsterSpawn.position, Quaternion.identity);

        isInBattle = true;
    }

    #endregion

    #region Camera Manager

    private void CameraManager()
    {
        if (isInBattle)
            Camera.main.transform.position = new Vector3(battlePointTransform.position.x, battlePointTransform.position.y, -10);
        else
            Camera.main.transform.position = new Vector3(trainerTransform.position.x, trainerTransform.position.y, -10);
    }

    #endregion

    private void Update()
    {
        /*
        if (isBattle && !areSpawned)
        {

            areSpawned = true;           
        }*/

        CameraManager();
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
        isInBattle = false;
        //areSpawned = false;
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

    public void ExitGame()
    {
        Application.Quit();
    }
}
