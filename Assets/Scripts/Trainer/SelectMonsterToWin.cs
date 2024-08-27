using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectMonsterToWin : MonoBehaviour
{
    [SerializeField] Button[] monsterButtons;
    [SerializeField]private List<Monster> monsters = new List<Monster>();
    private bool canSelect = true;

    [SerializeField] private GameObject panelSelectMonster;
    [SerializeField] private GameObject panelSuccessful;


    private void Start()
    {
        panelSelectMonster.SetActive(false);
        panelSuccessful.SetActive(false);

        //FindMontersEnemy();
        AddEventsToButtons();    

        SelectMonsterFistTime();
    }

    #region FirstEnterGame

    public void SelectMonsterFistTime()
    {
        Debug.Log("asasasas: " + FindObjectOfType<TrainerController>().GetAllMonsters().Count);

        //Debug.Log("Cantidad" + FindObjectOfType<TrainerController>().GetAllMonsters().Count);
        if (FindObjectOfType<TrainerController>().GetAllMonsters().Count <= 0)
        {
            var localMonsters = DataManager.Instance.LoadAllMonstersFromAssets();

            if (localMonsters != null)
            {
                monsters = localMonsters;

                for (int i = 0; i < monsterButtons.Length; i++)
                {
                    if (i < monsters.Count && monsters[i] != null)
                    {
                        monsterButtons[i].image.sprite = monsters[i].monsterSprite;
                    }
                    else
                    {
                        monsterButtons[i].gameObject.SetActive(false);
                    }
                }
            }

            panelSelectMonster.SetActive(true);
        }
    }

    #endregion

    #region Initialization

    private void AddEventsToButtons()
    {
        for (int i = 0; i < monsterButtons.Length; i++)
        {
            int index = i;

            if (monsterButtons[i] != null)
            {
                monsterButtons[i].onClick.AddListener(() => SelectMonster(index));
            }
        }
    } 

    private void FindMontersEnemy()
    {
        monsters = GameObject.FindAnyObjectByType<TrainerEnemyController>().GetAllMonsters();

        if (monsters != null)
        {
            for (int i = 0; i < monsterButtons.Length; i++)
            {
                if (i < monsters.Count && monsters[i] != null)
                {
                    monsterButtons[i].image.sprite = monsters[i].monsterSprite;
                }
                else
                {
                    monsterButtons[i].gameObject.SetActive(false);
                }
            }
        }
        
    }

    #endregion

    private void SelectMonster(int index) 
    {
        if (canSelect)
        {
            canSelect = false;
            GameObject.FindAnyObjectByType<TrainerController>().AddMonster(monsters[index]);
            StartCoroutine(ShowSuccessPanel());
        }
    }

    private IEnumerator ShowSuccessPanel()
    {
        panelSelectMonster.SetActive(false);
        panelSuccessful.SetActive(true);
        yield return new WaitForSeconds(4f);   
        panelSuccessful.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.isWinBattle && canSelect)
        {
            panelSelectMonster.SetActive(true);
        }
    }
}
