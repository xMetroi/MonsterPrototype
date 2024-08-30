using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainerEnemyController : MonoBehaviour
{
    
    public List<Monster> monsters = new List<Monster>();
    [SerializeField] private float[] monstersHP = new float[4];

    [SerializeField] int count;

    AIReferences aiReferences;
    EnemyBrain enemyBrain;
    //PlayerCombat playerCombat;

    [SerializeField] private int currentMonster;

    private void Start()
    {
        //currentMonster = 0;
        //count = 0;
        SetAllMonstersHP();
    }

    public void SetAllMonstersHP()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monstersHP[i] = monsters[i].monsterHealth;
        }
    }


    public void SetCurrentMonster(GameObject enemy)
    {
        Debug.Log(monsters.Count);
        if (count >= monsters.Count)
        {
            Debug.Log("Gano el juego");
            GameManager.instance.TriggerBattleEnded(true);
            return;
        }
        else
        {
            Debug.Log("Seteo los valores");
            aiReferences = enemy.GetComponent<AIReferences>();
            enemyBrain = enemy.GetComponent<EnemyBrain>();

            monstersHP[currentMonster] = enemyBrain.GetHP();

            aiReferences.currentMonster = monsters[currentMonster];
            enemyBrain.Initialize();

            currentMonster++;
            count++;
        }   
    }

    public void ResetData()
    {
        currentMonster = 0;
        count = 0;
        SetAllMonstersHP();
    }
}
