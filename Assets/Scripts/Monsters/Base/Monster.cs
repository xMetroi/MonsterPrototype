using UnityEngine;

[CreateAssetMenu(menuName = "Monsters/New Monster")]
public class Monster : ScriptableObject
{
    [Header("Monster Info")]
    public int monsterID;
    public string monsterName;
    public Sprite monsterSprite;

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
}
