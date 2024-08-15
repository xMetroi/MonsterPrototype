using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Properties")]
    [SerializeField] private float monsterHp;
    [SerializeField] private Attack basickAttack1;
    [SerializeField] private Attack basickAttack2;
    [SerializeField] private Attack specialAttack;
    private bool canUseBasicAttack1 = true;
    private bool canUseBasicAttack2 = true;
    private bool canUseSpecialAttack = true;

    [Header("Defense Properties")]
    [SerializeField] private float initialDefenseBubbleHP;
    [SerializeField] private float defenseBubbleHP;
    [SerializeField] private float defenseBubbleBrokenCooldown;
    private bool canDefense = true;
    public bool isDefensed;
    private bool defenseBroken;

    [SerializeField] private Transform arrowTransform;
    [SerializeField] private PlayerReferences references;

    #region Events

    public event Action<float> StartDefense;
    public event Action<float> StopDefense;

    #endregion

    private void Awake()
    {
        defenseBubbleHP = initialDefenseBubbleHP;
    }

    private void Start()
    {
        if (references.actualMonster != null)
        {
            monsterHp = references.actualMonster.monsterHealth;
            basickAttack1 = references.actualMonster.basickAttack1;
            basickAttack2 = references.actualMonster.basickAttack2;
            specialAttack = references.actualMonster.specialAttack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Combat();
        DefenseBubble();
    }

    #region Combat

    private void Combat()
    {
        if (!CanAttack()) return;

        if (references.playerInputs.Combat.BasicAttack1.WasPerformedThisFrame() && canUseBasicAttack1)
        {
            Debug.Log($"{basickAttack1.attackName}, applying cooldown: {basickAttack1.attackCooldown}");
            StartCoroutine(StartBasicAttack1Cooldown(basickAttack1.attackCooldown));
        }

        if (references.playerInputs.Combat.BasicAttack2.WasPerformedThisFrame() && canUseBasicAttack2)
        {
            Debug.Log($"{basickAttack2.attackName}, applying cooldown: {basickAttack2.attackCooldown}");
            StartCoroutine(StartBasicAttack2Cooldown(basickAttack2.attackCooldown));
        }

        if (references.playerInputs.Combat.Special.WasPerformedThisFrame() && canUseSpecialAttack)
        {
            Debug.Log($"{specialAttack.attackName}, applying cooldown: {specialAttack.attackCooldown}");
            StartCoroutine(StartSpecialAttackCooldown(specialAttack.attackCooldown));
        }
    }

    private IEnumerator StartBasicAttack1Cooldown(float cooldown)
    {
        canUseBasicAttack1 = false;
        Attack attack = references.actualMonster.basickAttack1;

        if (basickAttack1.attackType == Attack.AttackType.Throwable)
        {
            SpawnThrowable(attack);
        }
        else if (basickAttack1.attackType == Attack.AttackType.Melee)
        {
            SpawnMelee(attack);
        }
        else if (basickAttack1.attackType == Attack.AttackType.Transformation)
        {
            StartCoroutine(StartTransformAttack(attack));
        }

        yield return new WaitForSeconds(cooldown);
        canUseBasicAttack1 = true;
    }

    private IEnumerator StartBasicAttack2Cooldown(float cooldown)
    {
        canUseBasicAttack2 = false;
        Attack attack = references.actualMonster.basickAttack2;

        if (basickAttack2.attackType == Attack.AttackType.Throwable)
        {       
            SpawnThrowable(attack);
        }
        else if (basickAttack2.attackType == Attack.AttackType.Melee)
        {
            SpawnMelee(attack);
        }
        else if (basickAttack2.attackType == Attack.AttackType.Transformation)
        {
            StartCoroutine(StartTransformAttack(attack));
        }

        yield return new WaitForSeconds(cooldown);
        canUseBasicAttack2 = true;
    }

    private IEnumerator StartSpecialAttackCooldown(float cooldown)
    {
        canUseSpecialAttack = false;
        Attack attack = references.actualMonster.specialAttack;

        if (specialAttack.attackType == Attack.AttackType.Throwable)
        {
            SpawnThrowable(attack);
        }
        else if (specialAttack.attackType == Attack.AttackType.Melee)
        {
            SpawnMelee(attack);
        }
        else if (specialAttack.attackType == Attack.AttackType.Transformation)
        {
            StartCoroutine(StartTransformAttack(attack));
        }

        yield return new WaitForSeconds(cooldown);
        canUseSpecialAttack = true;
    }

    public void SpawnThrowable(Attack attack)
    {
        GameObject go = Instantiate(attack.throwablePrefab, transform.position, Quaternion.identity);

        Vector2 direction;
        Vector2 lastMovementInput = references.playerMovement.GetLastMovementInput();
        Quaternion rotation;

        if (references.playerAim.isAiming)
            direction = arrowTransform.right;
        else
            direction = references.playerMovement.GetLastMovementInput();

        if (references.playerAim.isAiming)
            rotation = arrowTransform.rotation;
        else
        {
            float angleInRadians = Mathf.Atan2(lastMovementInput.y, lastMovementInput.x); // Ángulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversión a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        go.GetComponent<ProjectileManager>().Initialize(direction, rotation, attack.throwableSpeed, 5f);
    }

    public void SpawnMelee(Attack attack)
    {
        Vector2 direction;
        Vector2 lastMovementInput = references.playerMovement.GetLastMovementInput();
        Quaternion rotation;

        if (references.playerAim.isAiming)
            direction = transform.position + arrowTransform.right;
        else
            direction = new Vector2(transform.position.x + lastMovementInput.x, transform.position.y + lastMovementInput.y);

        if (references.playerAim.isAiming)
            rotation = arrowTransform.rotation;
        else
        {
            float angleInRadians = Mathf.Atan2(lastMovementInput.y, lastMovementInput.x); // Ángulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversión a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        GameObject go = Instantiate(attack.meleePrefab, direction, rotation);
    }

    public IEnumerator StartTransformAttack(Attack attack)
    {
        references.monsterSprite.sprite = attack.transformationSprite;
        references.playerMovement.SetMovementSpeeds
        (
            references.actualMonster.monsterSpeed * attack.transformationSpeedMultiplier,
            references.actualMonster.monsterAimSpeed * attack.transformationSpeedMultiplier
        );

        yield return new WaitForSeconds(attack.transformationDuration);
        references.monsterSprite.sprite = references.actualMonster.monsterSprite;
        references.playerMovement.SetMovementSpeeds
        (
            references.actualMonster.monsterSpeed,
            references.actualMonster.monsterAimSpeed
        );
    }

    public bool CanAttack()
    {
        if (isDefensed)
            return false;

        return true;
    }

    #endregion

    #region Defense
    private void DefenseBubble()
    {
        if (references.playerInputs.Combat.Defense.WasPressedThisFrame())
        {
            if (canDefense)
            {
                isDefensed = true;
                StartDefense?.Invoke(defenseBubbleHP);
            }
        }

        if (references.playerInputs.Combat.Defense.WasReleasedThisFrame())
        {
            isDefensed = false;
            StopDefense?.Invoke(defenseBubbleHP);
        }

        if (defenseBubbleHP <= 0)
        {
            StartCoroutine(BubbleCooldown());
        }

        if (defenseBroken)
            canDefense = false;
    }

    private IEnumerator BubbleCooldown()
    {
        canDefense = false;
        isDefensed = false;
        defenseBroken = true;
        yield return new WaitForSeconds(defenseBubbleBrokenCooldown);
        defenseBubbleHP = initialDefenseBubbleHP;
        defenseBroken = false;
        canDefense = true;
    }

    #endregion
}
