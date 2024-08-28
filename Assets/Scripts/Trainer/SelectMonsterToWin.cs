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

        //Event subscription
        GameManager.instance.BattleEnded += OnBattleEnded;

        //FindMontersEnemy();
        AddEventsToButtons();    

        SelectMonster();
    }

    private void OnDestroy()
    {
        GameManager.instance.BattleEnded -= OnBattleEnded;
    }

    #region FirstEnterGame

    public void SelectMonster()
    {
        Debug.Log("asasasas: " + FindObjectOfType<TrainerController>().GetAllMonsters().Count);

        //Debug.Log("Cantidad" + FindObjectOfType<TrainerController>().GetAllMonsters().Count);
        if (FindObjectOfType<TrainerController>().GetAllMonsters().Count <= 0)
        {
            panelSelectMonster.SetActive(true);
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

        }
    }

    #endregion

    #region Initialization

    public void AddEventsToButtons()
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

    private void OnBattleEnded(bool playerWin)
    {
        if (playerWin && canSelect)
            panelSelectMonster.SetActive(true);
    }
}
