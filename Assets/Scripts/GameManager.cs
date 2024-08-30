using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public event Action<TrainerEnemyController> BattleStarted;
    public event Action<TrainerEnemyController, bool> BattleEnded;

    //Menu Pausa
    private GameObject pauseMenu;
    private GameObject pausePanel;

    private GameObject youWinPanel;

    public bool gameFinished;

    public bool isUiOpened;

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
            if (DataManager.Instance.LoadPlayerData() == null)
            {
                LoadScene("WorldPROTOTYPEScene", 33);
            }
            else
            {
                LoadScene("WorldPROTOTYPEScene");
            }
        }

        if (scene.name == "WorldPROTOTYPEScene")
        {
            InitializeWorldVariables();

            AudioManager.instance.PlayExplorationMusic();
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

        youWinPanel = GameObject.Find("YouWinPanel");
        youWinPanel.transform.Find("Quit").GetComponent<Button>().onClick.AddListener(() => ExitGame());
        youWinPanel.SetActive(false);
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.transform.Find("Pause Button").GetComponent<Button>().onClick.AddListener(() => PauseGame());
        

        pausePanel = pauseMenu.transform.Find("PanelPause").gameObject;

        Button[] buttons = pausePanel.GetComponentsInChildren<Button>();

        buttons[0].onClick.AddListener(() => PauseGame());
        buttons[1].onClick.AddListener(() => ExitGame());

        pausePanel.SetActive(false);
    }

    /// <summary>
    /// We use this method to start a battle
    /// </summary>
    public void StartBattle(TrainerEnemyController trainerEnemyController)
    {
        gameFinished = false;
        isUiOpened = false;
        FindAnyObjectByType<VSManager>().OnBattleStarted(trainerEnemyController);

        playerMonsterGO = Instantiate(playerMonsterPrefab, playerMonsterSpawn.position, Quaternion.identity);
        enemyMonsterGO = Instantiate(enemyMonsterPrefab, enemyMonsterSpawn.position, Quaternion.identity);

        isInBattle = true;
        FindObjectOfType<CombatUI>().AddDataUiFistTime(playerMonsterGO, enemyMonsterGO);
        this.trainerEnemyController = trainerEnemyController;
        BattleStarted?.Invoke(trainerEnemyController);

        trainerEnemyController.SetCurrentMonster(enemyMonsterGO);

        AudioManager.instance.StopMusicClip();
        AudioManager.instance.PlayFightMusic();
    }

    //Events

    private void OnBattleEnded(TrainerEnemyController trainerEnemyController, bool playerWin)
    {
        //Si el jugador pierde
        if (!playerWin)
        {
            gameFinished = true;
            GameObject.FindAnyObjectByType<GameOverCanvas>().ShowLooseHolder();
            var positionPlayer = FindObjectOfType<TrainerController>().pointPosition;
            FindObjectOfType<TrainerMovement>().transform.position = positionPlayer;
            FindObjectOfType<TrainerController>().ResetData();
            trainerEnemyController.ResetData();
            trainerEnemyController.gameObject.GetComponent<TrainerEnemyMovement>().ResetData();
        }

        else // Si el jugador gana
        {
            FindObjectOfType<TrainerController>().defeatedEnemies.Add(trainerEnemyController.gameObject.name);
            FindObjectOfType<TrainerController>().ResetData();
            trainerEnemyController.ResetData();
            Destroy(trainerEnemyController.gameObject); // Destruimos al enemigo

            if (FindObjectOfType<TrainerController>().defeatedEnemies.Count >= 4)
            {
                youWinPanel.SetActive(true);
            }
        }

        isInBattle = false;

        AudioManager.instance.StopMusicClip();
        AudioManager.instance.PlayExplorationMusic();

        Destroy(playerMonsterGO);
        Destroy(enemyMonsterGO);

        FindObjectOfType<CombatUI>().transform.Find("Panel").gameObject.SetActive(false);

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
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;

            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

    }
}
