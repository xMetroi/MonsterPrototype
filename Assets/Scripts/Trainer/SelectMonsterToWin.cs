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

        //if (FindObjectOfType<TrainerController>().GetAllMonsters().Count <= 0)
            //SelectMonsterRandom(3);
    }

    private void OnDestroy()
    {
        GameManager.instance.BattleEnded -= OnBattleEnded;
    }

    public void SelectMonster(List<Monster> monsters)
    {
        panelSelectMonster.SetActive(true);
        List<GameObject> monsterButtons = new List<GameObject>();

        if (monsters != null)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                GameObject monsterButton = Instantiate(monsterButtonPrefab, buttonsHolder);
                monsterButtons.Add(monsterButton);
                monsterButton.GetComponent<Image>().sprite = monsters[i].monsterSprite;
                monsterButton.GetComponent<Button>().onClick.AddListener(() => AddMonster(monsters[i], monsterButtons));
            }
        }
    }

    [ContextMenu("test")]
    public void SelectMonsterRandom()
    {
        panelSelectMonster.SetActive(true);
        List<Monster> allMonstersList = DataManager.Instance.LoadAllMonstersFromAssets();
        List<GameObject> monsterButtons = new List<GameObject>();

        if (allMonstersList != null)
        {
            for(int i = 0; i < 3; i++)
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

        foreach(GameObject monsterButton in buttons)
        {
            Destroy(monsterButton);
        }

        StartCoroutine(ShowSuccessPanel());
    }

    private IEnumerator ShowSuccessPanel()
    {
        panelSelectMonster.SetActive(false);
        panelSuccessful.SetActive(true);
        yield return new WaitForSeconds(4f);   
        panelSuccessful.SetActive(false);
    }

    private void OnBattleEnded(bool playerWin)
    {
        if (playerWin && canSelect)
        {

        }            
    }
}
