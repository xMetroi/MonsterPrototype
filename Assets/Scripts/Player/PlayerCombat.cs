using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour, IDamageable
{
    [Header("Combat Properties")]
    [SerializeField] private float monsterHp;
    [SerializeField] private Attack basickAttack1;
    [SerializeField] private Attack basickAttack2;
    [SerializeField] private Attack specialAttack;
    private bool canUseBasicAttack1 = true;
    private bool canUseBasicAttack2 = true;
    private bool canUseSpecialAttack = true;
    [Space]
    [SerializeField] private float hittedCooldown;
    private bool isHitted = false;

    [Header("Defense Properties")]
    [SerializeField] private float initialDefenseBubbleHP;
    [SerializeField] private float defenseBubbleHP;
    [SerializeField] private float defenseBubbleBrokenCooldown;
    public bool isDefensed;
    private bool defenseBroken;
    private Coroutine BubbleBreakRoutine;

    [SerializeField] private Transform arrowTransform;
    [SerializeField] private PlayerReferences references;

    private Animator animatorMonster;

    #region Events

    //Attack
    public event Action<Attack> AttackStarted;

    //Transformation
    public event Action<Attack> TransformationStart;
    public event Action<Attack> TransformationEnd;

    //Hit
    public event Action<float, Monster> StartHitted;
    public event Action StopHitted;

    //Defense
    public event Action<float> StartDefense;
    public event Action<float> StopDefense;
    public event Action<float> HitDefensed;
    public event Action DefenseBroke;
    public event Action DefenseRegenerated;

    #endregion

    #region Getter / Setters

    public float GetHP()
    {
        return monsterHp;
    }

    public void SetHP(float hp)
    {
        monsterHp = hp;
    }

    public bool GetIsHitted() { return isHitted; }

    #endregion

    #region Utilities / Checkers

    public bool CanAttack()
    {
        if (isDefensed)
            return false;

        if (isHitted)
            return false;

        if (!GameManager.instance.isInBattle)
            return false;

        if (GameManager.instance.gameFinished)
            return false;

        return true;
    }

    public bool CanDefend()
    {
        if (defenseBroken)
            return false;

        if (isHitted)
            return false;

        return true;
    }

    #endregion

    private void Awake()
    {
        defenseBubbleHP = initialDefenseBubbleHP;
    }

    private void Start()
    {
        animatorMonster = GetComponent<Animator>();

        Initialize();
    }

    public void Initialize()
    {
        if (references.currentMonster != null)
        {
            monsterHp = references.currentMonster.monsterHealth;
            basickAttack1 = references.currentMonster.basickAttack1;
            basickAttack2 = references.currentMonster.basickAttack2;
            specialAttack = references.currentMonster.specialAttack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Combat();
        Defense();
    }

    #region Combat

    private void Combat()
    {
        if (!CanAttack()) return;

        if (references.playerInputs.Combat.BasicAttack1.WasPerformedThisFrame() && canUseBasicAttack1)
        {
            Debug.Log($"{basickAttack1.attackName}, applying cooldown: {basickAttack1.attackCooldown}");
            StartCoroutine(StartBasicAttack1Cooldown(basickAttack1.attackCooldown));
            FindObjectOfType<CombatUI>().StartCooldownPlayerAttack1(basickAttack1.attackCooldown);
        }

        if (references.playerInputs.Combat.BasicAttack2.WasPerformedThisFrame() && canUseBasicAttack2)
        {
            Debug.Log($"{basickAttack2.attackName}, applying cooldown: {basickAttack2.attackCooldown}");
            StartCoroutine(StartBasicAttack2Cooldown(basickAttack2.attackCooldown));
            FindObjectOfType<CombatUI>().StartCooldownPlayerAttack2(basickAttack2.attackCooldown);

        }

        if (references.playerInputs.Combat.Special.WasPerformedThisFrame() && canUseSpecialAttack)
        {
            Debug.Log($"{specialAttack.attackName}, applying cooldown: {specialAttack.attackCooldown}");
            StartCoroutine(StartSpecialAttackCooldown(specialAttack.attackCooldown));
            FindObjectOfType<CombatUI>().StartCooldownPlayerAttack3(specialAttack.attackCooldown);

        }
    }

    private IEnumerator StartBasicAttack1Cooldown(float cooldown)
    {
        canUseBasicAttack1 = false;
        Attack attack = references.currentMonster.basickAttack1;

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
        Attack attack = references.currentMonster.basickAttack2;

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
        Attack attack = references.currentMonster.specialAttack;

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
            float angleInRadians = Mathf.Atan2(lastMovementInput.y, lastMovementInput.x); // �ngulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi�n a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        go.GetComponent<ProjectileManager>().Initialize(direction, rotation, attack, 5f, GetComponent<Collider2D>());

        AttackStarted?.Invoke(attack);
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
            float angleInRadians = Mathf.Atan2(lastMovementInput.y, lastMovementInput.x); // �ngulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi�n a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        GameObject go = Instantiate(attack.meleePrefab, direction, rotation);

        MeleeAttack(go.transform.position, lastMovementInput * attack.attackKnockback, attack);

        AttackStarted?.Invoke(attack);
    }

    public IEnumerator StartTransformAttack(Attack attack)
    {
        references.monsterSprite.sprite = attack.spriteTransformation;
        references.monsterAnimator.runtimeAnimatorController = attack.animatorTransformation;

        references.playerMovement.SetMovementSpeeds
        (
            references.currentMonster.monsterSpeed * attack.transformationSpeedMultiplier,
            references.currentMonster.monsterAimSpeed * attack.transformationSpeedMultiplier
        );

        TransformationStart?.Invoke(attack);

        yield return new WaitForSeconds(attack.transformationDuration);
        references.monsterSprite.sprite = references.currentMonster.monsterSprite;
        references.monsterAnimator.runtimeAnimatorController = references.currentMonster.monsterAnimator;
        
        references.playerMovement.SetMovementSpeeds
        (
            references.currentMonster.monsterSpeed,
            references.currentMonster.monsterAimSpeed
        );

        TransformationEnd?.Invoke(attack);
    }

    public void MeleeAttack(Vector2 position, Vector2 kb, Attack attack)
    {
        if (!CanAttack()) return;

        Collider2D[] objects = Physics2D.OverlapCircleAll(position, attack.meleeRange);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Monster") && collider != GetComponent<Collider2D>())
            {
                IDamageable damageable = collider.GetComponentInParent<IDamageable>();
                damageable.Damage(attack.attackDamage, kb);
            }
        }
    }

    #endregion

    #region Defense
    private void Defense()
    {
        if (references.playerInputs.Combat.Defense.WasPressedThisFrame())
        {
            if (CanDefend())
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
            if (BubbleBreakRoutine != null)
            {
                StopCoroutine(BubbleBreakCoroutine());
                return;
            }

            BubbleBreakRoutine = StartCoroutine(BubbleBreakCoroutine());
        }
    }

    private IEnumerator BubbleBreakCoroutine()
    {
        DefenseBroke?.Invoke();
        isDefensed = false;
        defenseBroken = true;
        yield return new WaitForSeconds(defenseBubbleBrokenCooldown);
        DefenseRegenerated?.Invoke();   
        defenseBubbleHP = initialDefenseBubbleHP;
        defenseBroken = false;

        BubbleBreakRoutine = null;
    }

    #endregion

    #region Damage

    public void Damage(float damage, Vector2 kb)
    {
        if (isDefensed)
        {
            defenseBubbleHP--;
            HitDefensed?.Invoke(defenseBubbleHP);
        }
        else
        {
            if (!isHitted)
            {
                StartHitted?.Invoke(damage, references.currentMonster);
                StartCoroutine(RoutineDamage(damage, kb));

                if (monsterHp <= 0 && GameManager.instance.isInBattle)
                {
                    FindObjectOfType<TrainerController>().ChangeMonster(this.gameObject);
                }
            }
        }               
    }

    IEnumerator RoutineDamage(float damage, Vector2 kb)
    {        
        isHitted = true;

        references.rb.velocity = Vector2.zero;
        references.playerMovement.ApplyForce(kb);

        monsterHp -= damage;

        yield return new WaitForSeconds(hittedCooldown);

        StopHitted?.Invoke();
        isHitted = false;
    }

    #endregion
}
