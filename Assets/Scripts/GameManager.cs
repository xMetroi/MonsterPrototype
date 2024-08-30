using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private GameObject trainerGO;
    [SerializeField] private Transform trainerTransform;

    [Header("Combat Site Properties")]
    public bool isInBattle = false;
    [SerializeField] private Transform playerMonsterSpawn;
    [SerializeField] private Transform enemyMonsterSpawn;
    [SerializeField] private GameObject playerMonsterPrefab;
    [SerializeField] private GameObject enemyMonsterPrefab;
    [SerializeField] private Transform battlePointTransform;
    public TrainerEnemyController trainerEnemyController;
    GameObject playerMonsterGO;
    GameObject enemyMonsterGO;

    [Header("Slow Motion")]
    public float slowMotionFactor = 0.05f;
    public float slowMotionDuration = 0.08f;
    private bool isSlowMotionActive = false;

    #region Events

    public event Action <TrainerEnemyController> BattleStarted;
    public event Action<TrainerEnemyController, bool> BattleEnded;

    #endregion

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
        BattleEnded += OnBattleEnded;
    }

    #region Getter / Setter

    public bool GetIsInBattle() { return isInBattle; }

    #endregion

    #region Event Manager

    public void TriggerBattleEnded(bool playerWon)
    {
        BattleEnded?.Invoke(trainerEnemyController, playerWon);
    }


    #endregion

    #region Scene Manager

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Intro")
        {
            LoadScene("WorldPROTOTYPEScene", 3);
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
        trainerGO = GameObject.Find("Trainer");
        trainerTransform = GameObject.Find("Trainer").transform;
        battlePointTransform = GameObject.Find("Fight Camera Holder").transform;

        playerMonsterSpawn = GameObject.Find("SpawnPointPlayerMonster").transform;
        enemyMonsterSpawn = GameObject.Find("SpawnPointEnemyMonster").transform;
    }

    /// <summary>
    /// We use this method to start a battle
    /// </summary>
    public void StartBattle(TrainerEnemyController trainerEnemyController)
    {
        FindObjectOfType<TrainerMovement>().SetCanMove(false);

        playerMonsterGO = Instantiate(playerMonsterPrefab, playerMonsterSpawn.position, Quaternion.identity);
        enemyMonsterGO = Instantiate(enemyMonsterPrefab, enemyMonsterSpawn.position, Quaternion.identity);

        isInBattle = true;
        FindObjectOfType<CombatUI>().AddDataUiFistTime(playerMonsterGO, enemyMonsterGO);
        this.trainerEnemyController = trainerEnemyController;
        BattleStarted?.Invoke(trainerEnemyController);

        trainerEnemyController.SetCurrentMonster(enemyMonsterGO);
    }

    //Events

    private void OnBattleEnded(TrainerEnemyController trainerEnemyController, bool playerWin)
    {
        //Si el jugador pierde
        if (!playerWin)
            GameObject.FindAnyObjectByType<GameOverCanvas>().ShowLooseHolder();

        else // Si el jugador gana
        {
            Destroy(trainerEnemyController.gameObject); // Destruimos al enemigo
        }

        isInBattle = false;
       
        Destroy(playerMonsterGO);
        Destroy(enemyMonsterGO);

        FindObjectOfType<CombatUI>().transform.Find("Panel").gameObject.SetActive(false);

        trainerGO.GetComponent<TrainerMovement>().SetCanMove(true);
    }

    #endregion

    #region Camera Manager

    private void CameraManager()
    {
        if (trainerTransform == null || trainerTransform.gameObject.scene.name != "WorldPROTOTYPEScene") return;

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

    //public void PlayerWins()
    //{
    //    panelLifeMonsters.SetActive(false);
    //    panelGameOver.SetActive(true);
    //    winText.SetActive(true);
    //    gameFinished = true;
    //}

    //public void PlayerLoose()
    //{
    //    panelLifeMonsters.SetActive(false);
    //    looseText.SetActive(true);
    //    gameFinished = true;
    //    StartCoroutine(StartSlow());
    //}

    //public void GameOver()
    //{
    //    panelLifeMonsters.SetActive(false);
    //    Time.timeScale = 0f;
    //    isInBattle = false;
    //    //areSpawned = false;
    //    StartCoroutine(StartSlow());
    //}
     
    IEnumerator StartSlow()
    {
        ActivateSlowMotion();
        yield return new WaitForSeconds(slowMotionDuration);
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

    public void PauseGame()
    {

    }
}
