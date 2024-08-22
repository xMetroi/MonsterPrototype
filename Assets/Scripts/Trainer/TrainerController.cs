using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters = new List<Monster>();
    private int coins = 0;


    private void Start()
    {
        if (SaveLoadManager.Instance.LoadPlayerMonster() != null)
        {
            SetMonsters(SaveLoadManager.Instance.LoadPlayerMonster());
        }
        
        //SetCoins(SaveLoadManager.Instance.LoadPlayerCoins());
    }

    public void SetMonsters(List<int> ids)
    {
        Debug.Log("ASDASA: " + ids.Count);

        List<Monster> allMonsters = SaveLoadManager.Instance.LoadAllMonstersFromAssets();

        List<Monster> filteredMonstersById = allMonsters
            .Where(monster => ids.Contains(monster.monsterID) && monster.monsterID != 0)
            .ToList(); // Convertir el resultado a una lista

        monsters = filteredMonstersById; // Asignar la lista filtrada a la variable monsters
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
