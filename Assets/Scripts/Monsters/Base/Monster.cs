using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Monsters/New Monster")]
public class Monster : ScriptableObject
{
    [Header("Monster Info")]
    public int monsterID;
    public string monsterName;

    [Header("Monster Stats Properties")]
    public float monsterHealth;
    public float monsterSpeed;
    public float monsterAimSpeed;
    public float monsterDashForce;
    public float monsterDashCooldown;

    [Header("Monster Attacks")]
    public Attack basickAttack1;
    public Attack basickAttack2;
    public Attack specialAttack;

    [Header("Sprite")]
    public GameObject prefabMonster;
}
