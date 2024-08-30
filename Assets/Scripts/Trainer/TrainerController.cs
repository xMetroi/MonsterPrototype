using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters = new List<Monster>();
    [SerializeField] private float[] monstersHP = new float[4];

    PlayerReferences playerReferences;
    PlayerVisuals playerVisuals;
    PlayerCombat playerCombat;

    private PlayerInputs controls;

    private int currentMonster;

    private bool canChange = true;

    private int count;

    public Vector3 pointPosition;
    public List<string> defeatedEnemies = new List<string>();

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
        currentMonster = 0;
        count = 0;
        if (DataManager.Instance.LoadPlayerMonster() != null)
        {
            SetMonsters(DataManager.Instance.LoadPlayerMonster());
            SetAllMonstersHP();
        }
        FirstDataPlayer();
        //Invoke(nameof(FirstDataPlayer), 0);
    }

    private void FirstDataPlayer()
    {
        if (DataManager.Instance.LoadEnemiesDefeated().Count > 0)
        {
            defeatedEnemies = DataManager.Instance.LoadEnemiesDefeated();

            for (int i = 0; i < DataManager.Instance.LoadEnemiesDefeated().Count; i++)
            {
                var enemy = GameObject.Find(DataManager.Instance.LoadEnemiesDefeated()[i]).gameObject;
                Destroy(enemy);
            }
        }
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
        List<Monster> allMonsters = DataManager.Instance.LoadAllMonstersFromAssets();

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
        currentMonster++;
        count++;
        return this.monsters[id];
        
    }

    private void Update()
    {

        if (GameManager.instance.isInBattle)
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
                if (monsters[0] != null && monstersHP[0] > 0 && currentMonster != 1)
                {
                    SetCurrentMonster(0);
                    StartCoroutine(CanChange());
                } 
            }
            if (controls.Combat.Change2.ReadValue<float>() > 0)
            {
                if (monsters[1] != null && monstersHP[1] > 0 && currentMonster != 2)
                {
                    SetCurrentMonster(1);
                    StartCoroutine(CanChange());
                }
            }
            if (controls.Combat.Change3.ReadValue<float>() > 0)
            {
                if (monsters[2] != null && monstersHP[2] > 0 && currentMonster != 3)
                {
                    SetCurrentMonster(2);
                    StartCoroutine(CanChange());
                }
            }
            if (controls.Combat.Change4.ReadValue<float>() > 0 && currentMonster != 4)
            {
                if (monsters[3] != null && monstersHP[3] > 0)
                {
                    SetCurrentMonster(3);
                    StartCoroutine(CanChange());
                }
            }
        }
    }

    IEnumerator CanChange()
    {
        canChange = false;
        yield return new WaitForSeconds(1f);
        Debug.Log("Puede cambiar");
        canChange = true;

    }

    private void SetCurrentMonster(int idx)
    {
        monstersHP[currentMonster - 1] = playerCombat.GetHP();

        playerReferences.currentMonster = monsters[idx];
        playerVisuals.SetMonsterData();
        playerCombat.Initialize();
        playerCombat.SetHP(monstersHP[idx]);
        
        currentMonster = idx + 1;
    }

    public void ChangeMonster(GameObject enemy)
    {
        if (count >= monsters.Count)
        {
            GameManager.instance.TriggerBattleEnded(false);
            return;
        }

        // Actualiza el HP del monstruo actual
        if (currentMonster - 1 < monsters.Count)
        {
            monstersHP[currentMonster - 1] = playerCombat.GetHP();
            playerCombat.SetHP(monstersHP[currentMonster - 1]);
        }

        // Verifica si todos los monstruos tienen HP menor o igual a 0
        if (monstersHP.All(n => n <= 0))
        {
            GameManager.instance.TriggerBattleEnded(false);
            return;
        }
        // Verifica si currentMonster ha alcanzado el límite de monstruos
        else if (currentMonster >= monsters.Count)
        {
            // Encuentra la primera posición donde monstersHP sea mayor a 0 y no sea la posición actual
            int newIndex = -1;
            for (int i = 0; i < monstersHP.Length; i++)
            {
                if (i != currentMonster && monstersHP[i] > 0)
                {
                    newIndex = i;
                    break;
                }
            }

            // Si no encuentra un monstruo válido, termina el combate
            if (newIndex == -1)
            {
                GameManager.instance.TriggerBattleEnded(false);
                return;
            }

            // Actualiza currentMonster a la nueva posición válida
            currentMonster = newIndex;
        }

        

        // Configura las referencias del jugador y visuales
        playerReferences.currentMonster = monsters[currentMonster];
        playerVisuals.SetMonsterData();
        playerCombat.Initialize();
        playerCombat.SetHP(monstersHP[currentMonster]); // Descomentar si se necesita establecer HP específico

        // Avanza al siguiente monstruo
        currentMonster++;
        count++;
    }

    public void ResetData()
    {
        currentMonster = 0;
        count = 0;
        SetAllMonstersHP();
    }

}
