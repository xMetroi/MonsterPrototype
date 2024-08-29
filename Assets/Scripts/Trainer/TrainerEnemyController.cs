using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainerEnemyController : MonoBehaviour
{
    public List<Monster> monsters = new List<Monster>();
    private int coins = 0;


    private void Start()
    {
        //SetMonsters(SaveLoadManager.Instance.LoadPlayerMonster());
        //SetCoins(SaveLoadManager.Instance.LoadPlayerCoins());
    }

    public void SetMonsters(List<int> ids)
    {
        List<Monster> allMonsters = DataManager.Instance.LoadAllMonstersFromAssets();

        Monster[] filteredMonstersById = allMonsters
            .Where(monster => ids.Contains(monster.monsterID))
            .ToArray();

        monsters = allMonsters;
    }

    public void AddMonster(Monster monster)
    {
        this.monsters.Add(monster);
    }

    public List<Monster> GetAllMonsters()
    {
        return this.monsters;
    }

    public List<int> GetAllMonstersById()
    {
        List<int> ids = new List<int>();

        foreach (Monster monster in this.monsters)
        {
            ids.Add(monster.monsterID);
        }

        return ids;
    }

    public Monster GetMonsterById(int id)
    {
        return this.monsters[id];
    }

    public void SetCoins(int coins)
    {
        this.coins = coins; 
    }

    public void AddCoins(int coin)
    {
        this.coins += coin;
    }

    public int GetCoins()
    {
        return this.coins;
    }
}
