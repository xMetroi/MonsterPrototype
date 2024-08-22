using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Slider sliderPlayer;

    private Animator animatorMonster;

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
        animatorMonster = GetComponent<Animator>();

        if (references.currentMonster != null)
        {
            monsterHp = references.currentMonster.monsterHealth;
            sliderPlayer.maxValue = monsterHp;
            sliderPlayer.value = monsterHp;
            basickAttack1 = references.currentMonster.basickAttack1;
            basickAttack2 = references.currentMonster.basickAttack2;
            specialAttack = references.currentMonster.specialAttack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Combat();
        DefenseBubble();

        IsDead();
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
            float angleInRadians = Mathf.Atan2(lastMovementInput.y, lastMovementInput.x); // Ángulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversión a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        go.GetComponent<ProjectileManager>().Initialize(direction, rotation, attack.throwableSpeed, 5f, GetComponent<Collider2D>());
        StartCoroutine(TrackProjectile(go, attack));
    }


    private IEnumerator TrackProjectile(GameObject projectile, Attack attack)
    {
        while (projectile != null && projectile.transform != null)
        {
            DistanceAttack(projectile, attack);
            yield return new WaitForSeconds(0.001f);
        }
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

        MeleeAttack(go.transform.position, attack);
    }

    public IEnumerator StartTransformAttack(Attack attack)
    {
        FindAnyObjectByType<PlayerController>().AssignPlayerPropertiesCollider(attack.prefabTransformation.GetComponent<BoxCollider2D>());
        references.monsterSprite.sprite = attack.prefabTransformation.GetComponent<SpriteRenderer>().sprite;
        references.monsterAnimator.runtimeAnimatorController = attack.prefabTransformation.GetComponent<Animator>().runtimeAnimatorController;

        references.playerMovement.SetMovementSpeeds
        (
            references.currentMonster.monsterSpeed * attack.transformationSpeedMultiplier,
            references.currentMonster.monsterAimSpeed * attack.transformationSpeedMultiplier
        );

        yield return new WaitForSeconds(attack.transformationDuration);
        FindAnyObjectByType<PlayerController>().AssignPlayerPropertiesCollider(references.currentMonster.prefabMonster.GetComponent<BoxCollider2D>());
        references.monsterSprite.sprite = references.currentMonster.prefabMonster.GetComponent<SpriteRenderer>().sprite;
        references.monsterAnimator.runtimeAnimatorController = references.currentMonster.prefabMonster.GetComponent<Animator>().runtimeAnimatorController;
        
        references.playerMovement.SetMovementSpeeds
        (
            references.currentMonster.monsterSpeed,
            references.currentMonster.monsterAimSpeed
        );
    }

    public bool CanAttack()
    {
        if (isDefensed)
            return false;

        return true;
    }

    public void MeleeAttack(Vector2 position, Attack attack)
    {
        if (!CanAttack()) return;

        Collider2D[] objects = Physics2D.OverlapCircleAll(position, attack.meleeRange);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Monster") && collider != GetComponent<Collider2D>())
            {
                GameObject.FindAnyObjectByType<AIBrain>().receiveDamage(1);
            }
        }
    }

    public void DistanceAttack(GameObject projectile, Attack attack)
    {
        if (!CanAttack()) return;

        Collider2D[] objects = Physics2D.OverlapCircleAll(projectile.transform.position, attack.meleeRange);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Monster") && collider != GetComponent<Collider2D>())
            {
                Debug.Log("AtaqueADistancia");
                GameObject.FindAnyObjectByType<AIBrain>().receiveDamage(2);
                Destroy(projectile); // Destruir el proyectil
            }
        }
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

    #region Damage

    public void receiveDamage(int damage)
    {
        StartCoroutine(RoutineDamage(damage));
    }

    IEnumerator RoutineDamage(int damage)
    {
        animatorMonster.SetBool("isAttacked", true);
        monsterHp -= damage;
        sliderPlayer.value = monsterHp;
        yield return new WaitForSeconds(0.5f);
        animatorMonster.SetBool("isAttacked", false);

    }

    private void IsDead()
    {
        if (monsterHp <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    #endregion
}
