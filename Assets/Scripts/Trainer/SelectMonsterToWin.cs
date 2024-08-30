using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectMonsterToWin : MonoBehaviour
{
    [SerializeField] private Transform buttonsHolder;
    [SerializeField] private GameObject monsterButtonPrefab;
    private bool canSelect = true;

    [SerializeField] private GameObject panelSelectMonster;
    [SerializeField] private GameObject panelSuccessful;


    private void Start()
    {
        panelSelectMonster.SetActive(false);
        panelSuccessful.SetActive(false);

        //Event subscription
        GameManager.instance.BattleEnded += OnBattleEnded;

        Invoke(nameof(FirstDataMonsterPlayer), 1f);
    }

    private void FirstDataMonsterPlayer()
    {
        if (FindObjectOfType<TrainerController>().GetAllMonsters().Count <= 0)
            SelectMonsterRandom(3);
    }

    private void OnDestroy()
    {
        GameManager.instance.BattleEnded -= OnBattleEnded;
    }

    /// <summary>
    /// Select Monsters from a specific list
    /// </summary>
    /// <param name="monsters"></param>
    public void SelectMonster(List<Monster> monsters)
    {
        panelSelectMonster.SetActive(true);
        FindObjectOfType<TrainerMovement>().SetCanMove(false);
        List<GameObject> monsterButtons = new List<GameObject>();

        if (monsters != null)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                Monster monster = monsters[i];
                GameObject monsterButton = Instantiate(monsterButtonPrefab, buttonsHolder);
                monsterButtons.Add(monsterButton);
                monsterButton.GetComponent<Image>().sprite = monsters[i].monsterSprite;
                monsterButton.GetComponent<Button>().onClick.AddListener(() => AddMonster(monster, monsterButtons));
            }
        }
    }

    /// <summary>
    /// Select random monsters
    /// </summary>
    /// <param name="monstersNumber"> numbers of monster to generate </param>
    public void SelectMonsterRandom(int monstersNumber)
    {
        panelSelectMonster.SetActive(true);
        FindObjectOfType<TrainerMovement>().SetCanMove(false);
        List<Monster> allMonstersList = DataManager.Instance.LoadAllMonstersFromAssets();
        List<GameObject> monsterButtons = new List<GameObject>();

        if (allMonstersList != null)
        {
            for(int i = 0; i < monstersNumber; i++)
            {
                monsterButtons.Add(Instantiate(monsterButtonPrefab, buttonsHolder));
            }
            
            foreach (GameObject monsterButton in monsterButtons)
            {
                int randomIndex = UnityEngine.Random.Range(0, allMonstersList.Count);
                Monster randomMonster = allMonstersList[randomIndex];

                monsterButton.GetComponent<Image>().sprite = randomMonster.monsterSprite;
                monsterButton.GetComponent<Button>().onClick.AddListener(() => AddMonster(randomMonster, monsterButtons));

                allMonstersList.RemoveAt(randomIndex);
            }
        }
    }

    private void AddMonster(Monster monster, List<GameObject> buttons)
    {
        panelSelectMonster.SetActive(false);
        GameObject.FindAnyObjectByType<TrainerController>().AddMonster(monster);

        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            Destroy(button);
        }

        StartCoroutine(ShowSuccessPanel());
    }

    private IEnumerator ShowSuccessPanel()
    {
        panelSelectMonster.SetActive(false);
        panelSuccessful.SetActive(true);
        FindObjectOfType<TrainerMovement>().SetCanMove(true);
        yield return new WaitForSeconds(4f);   
        panelSuccessful.SetActive(false);
    }

    private void OnBattleEnded(TrainerEnemyController trainerEnemyController, bool playerWin)
    {
        if (playerWin && canSelect)
        {
            List<Monster> enemyMonsters = trainerEnemyController.monsters;
            SelectMonster(enemyMonsters);
        }            
    }
}
