using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters = new List<Monster>();
    private float[] monstersHP = new float[4];
    private int coins = 0;

    PlayerReferences playerReferences;
    PlayerVisuals playerVisuals;
    PlayerCombat playerCombat;

    private PlayerInputs controls;

    private int currentMonster;

    private bool canChange = true;

    private void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        if (SaveLoadManager.Instance.LoadPlayerMonster() != null)
        {
            SetMonsters(SaveLoadManager.Instance.LoadPlayerMonster());
        }

        SetAllMonstersHP();
    }

    public void SetAllMonstersHP()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monstersHP[i] = monsters[i].monsterHealth;
        }
    }

    public void SetMonsters(List<int> ids)
    {
        List<Monster> allMonsters = SaveLoadManager.Instance.LoadAllMonstersFromAssets();

        List<Monster> filteredMonstersById = allMonsters
            .Where(monster => ids.Contains(monster.monsterID) && monster.monsterID != 0)
            .ToList(); // Convertir el resultado a una lista

        monsters = filteredMonstersById; // Asignar la lista filtrada a la variable monsters
    }

    public void AddMonster(Monster monster)
    {
        monsters.Add(monster);
        int index = monsters.IndexOf(monster);
        monstersHP[index] = monster.monsterHealth;
        Debug.Log("Monstruo añadido");
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

    public int GetCoins()
    {
        return this.coins;
    }

    private void Update()
    {

        if (GameManager.Instance.isBattle)
        {
            if (playerReferences == null)
            {
                playerReferences = FindObjectOfType<PlayerReferences>();
            }
            if (playerVisuals == null)
            {
                playerVisuals = FindObjectOfType<PlayerVisuals>();
            }
            if (playerCombat == null)
            {
                playerCombat = FindObjectOfType<PlayerCombat>();
            }
        }

        if (playerReferences != null && canChange)
        {
            if (controls.Combat.Change1.ReadValue<float>() > 0)
            {
                SetCurrentMonster(0);
                StartCoroutine(CanChange());
            }
            if (controls.Combat.Change2.ReadValue<float>() > 0)
            {
                SetCurrentMonster(1);
                StartCoroutine(CanChange());
            }
        }
    }

    IEnumerator CanChange()
    {
        canChange = false;
        yield return new WaitForSeconds(8f);
        canChange = true;

    }

    private void SetCurrentMonster(int idx)
    {
        monstersHP[currentMonster] = playerCombat.GetHP();

        playerReferences.currentMonster = monsters[idx];
        playerVisuals.SetMonsterData();
        playerCombat.Initialize();
        playerCombat.SetHP(monstersHP[idx]);
        
        currentMonster = idx;
    }
}
