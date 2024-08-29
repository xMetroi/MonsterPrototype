using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float actualSpeed;
    [Tooltip("The distance limit to still following the navmesh agent")]
    [SerializeField] private float stopDistance;
    [SerializeField] private Transform centerPoint;
    private bool goToNextPoint = true;
    [SerializeField] private Vector2 forceToApply;
    [SerializeField] private float forceDamping = 1.2f;
    private Vector2 velocity;

    [Header("Combat")]
    [SerializeField] private Monster actualMonster;
    [SerializeField] private float meleeRange;
    [SerializeField] private float hittedCooldown;
    private bool isHitted = false;

    [Header("Combat Stats")]
    [SerializeField] private float monsterHp;

    [Header("Combat Visuals")]
    [SerializeField] private Color damageColor;
    private Color originalColor;

    [Header("Defense")]
    [SerializeField] private float initialDefenseBubbleHP;
    [SerializeField] private float defenseBubbleHP;
    [SerializeField] private float defenseBubbleBrokenCooldown;
    [SerializeField] private bool isDefended;
    private bool defenseBroken;
    private Coroutine BubbleBreakRoutine;

    [Header("Defense Visuals")]
    [SerializeField] private GameObject defenseBubbleGO;

    [SerializeField] private AIReferences references;

    #region Events

    //Hit
    public event Action<float> StartHitted;
    public event Action StopHitted;

    //Defense
    public event Action<float> HitDefensed;
    public event Action DefenseBroke;
    public event Action DefenseRegenerated;

    #endregion

    #region Getter / Setter

    /// <summary>
    /// Return isDefended value
    /// </summary>
    /// <returns></returns>
    public bool GetIsDefended() { return isDefended; }

    public float GetBubbleHP() { return defenseBubbleHP; }

    #endregion

    #region Utility / Checkers

    /// <summary>
    /// Check if the monster has a ranged attack
    /// </summary>
    /// <returns></returns>
    public bool HasRangedAttack()
    {
        if (references.currentMonster.basickAttack1.attackType == Attack.AttackType.Throwable)
            return true;

        if (references.currentMonster.basickAttack2.attackType == Attack.AttackType.Throwable)
            return true;

        if (references.currentMonster.specialAttack.attackType == Attack.AttackType.Throwable)
            return true;

        return false;
    }

    /// <summary>
    /// Check if distance between player and the AI is in the melee range
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    public bool IsInMeleeRange(Vector3 playerPosition)
    {
        return Vector3.Distance(playerPosition, references.MonsterTransform.position) <= meleeRange;
    }

    /// <summary>
    /// Get distance from the player
    /// </summary>
    /// <returns></returns>
    public float GetDistanceWithPlayer()
    {
        return Vector3.Distance(references.MonsterTransform.position, references.playerReferences.playerTransform.position);
    }

    public bool CanMove()
    {
        if (isHitted)
            return false;

        if (isDefended)
            return false;

        if (!GameManager.instance.isInBattle)
            return false;

        return true;
    }

    /// <summary>
    /// Check if the AI can defense
    /// </summary>
    /// <returns></returns>
    public bool CanDefense()
    {
        if (defenseBroken)
            return false;

        if (isHitted)
            return false;

        return true;
    }

    /// <summary>
    /// Check if the AI can attack
    /// </summary>
    /// <returns></returns>
    public bool CanAttack()
    {
        if (references.state == States.Defensing)
            return false;

        if (isHitted)
            return false;

        if (!GameManager.instance.isInBattle)
            return false;

        return true;
    }

    #endregion

    private void Start()
    {
        originalColor = references.monsterSprite.color;
        centerPoint = GameObject.Find("CenterPoint").transform;

        references.stateMachineController.AttackStarted += OnAttackStarted;
        Initialize();
    }

    public void Initialize()
    {
        if (references.currentMonster != null)
        {
            monsterHp = references.currentMonster.monsterHealth;
        }
    }
    private void OnDestroy()
    {
        references.stateMachineController.AttackStarted -= OnAttackStarted;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove())
        {
            AgentMovement();
        }

        Defense();

        if (references.rb.velocity != Vector2.zero)
        {
            references.monsterAnimator.SetFloat("speed", 1);
        }
        else
        {
            references.monsterAnimator.SetFloat("speed", 0);
        }
    }

    public float GetHP()
    {
        return monsterHp;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    #region Movement

    private void AgentMovement()
    {
        if (references.state == States.Wandering)
        {
            if (goToNextPoint)
            {
                goToNextPoint = false;
                RandomPoint(centerPoint.position, 5, out Vector2 result);
                references.agent.SetDestination(result);
            }

            if (references.agent.remainingDistance <= references.agent.stoppingDistance && !references.agent.pathPending)
            {
                goToNextPoint = true;
            }
        }
        else
            references.agent.ResetPath();
    }

    private void Movement()
    {
        Vector2 direction = Vector2.zero;
        
        if (references.state == States.Wandering)
        {
            if (Vector3.Distance(references.MonsterTransform.position, references.agent.transform.position) >= 1)
                //Follow the NavMesh Agent
                direction = new Vector2(references.agent.transform.position.x, references.agent.transform.position.y) - references.rb.position;
        }
        else if (references.state == States.Approach)
        {
            //Go directly to the player
            direction = (references.playerReferences.playerTransform.position - references.MonsterTransform.position).normalized;
        }
        else if (references.state == States.RangedAttack)
        {
            //Follow the NavMesh Agent
            direction = new Vector2(references.agent.transform.position.x, references.agent.transform.position.y) - references.rb.position;
        }
        else
        {
            direction = Vector2.zero;
        }

        direction.Normalize();

        velocity = direction * actualSpeed;

        velocity += forceToApply;
        forceToApply /= forceDamping;

        if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
        {
            forceToApply = Vector2.zero;
        }

        references.rb.velocity = velocity;

        if (direction != Vector2.zero)
        {
            float rotationY = GameObject.FindObjectOfType<PlayerMovement>().transform.position.x > references.MonsterTransform.position.x ? 0 : 180f;
            references.MonsterTransform.rotation = Quaternion.Euler(0, rotationY, 0);
        }

    }

    public void ApplyForce(Vector2 force)
    {
        forceToApply = force;
    }

    /// <summary>
    /// Give us a random point in the nav mesh area to do random patrolling
    /// </summary>
    /// <param name="center"></param>
    /// <param name="range"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private bool RandomPoint(Vector3 center, float range, out Vector2 result)
    {
        Vector2 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector2.zero;
        return false;
    }

    #endregion

    #region Combat

    /// <summary>
    /// Triggers when an attack started
    /// </summary>
    /// <param name="attack"> The attack used </param>
    private void OnAttackStarted(Attack attack)
    {
        if (attack.attackType == Attack.AttackType.Melee)
        {
            SpawnMelee(attack);
        }
        else if (attack.attackType == Attack.AttackType.Throwable)
        {
            //Debug.Log(attack.attackName);
            SpawnThrowable(attack, true);
        }
        else if (attack.attackType == Attack.AttackType.Transformation)
        {
            //We are gonna use this? if yes tell me to add a transformation state to the state machine!
        }
    }

    /// <summary>
    /// We use this to spawn a melee attack prefab
    /// </summary>
    /// <param name="attack"></param>
    private void SpawnMelee(Attack attack)
    {
        Vector2 direction = references.playerReferences.playerTransform.position - references.MonsterTransform.position;
        direction.Normalize();
        float angleInRadians = Mathf.Atan2(direction.y, direction.x); // 聲gulo en radianes
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi鏮 a grados
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));

        GameObject go = Instantiate(attack.meleePrefab, references.playerReferences.playerTransform.position, rotation);
        IDamageable damageable = references.playerReferences.GetComponent<IDamageable>();
        damageable.Damage(attack.attackDamage, direction * attack.attackKnockback);
    }

    /// <summary>
    /// We use this to spawn a throwable attack prefab
    /// </summary>
    /// <param name="attack"></param>
    private void SpawnThrowable(Attack attack, bool enablePredict)
    {
        GameObject go = Instantiate(attack.throwablePrefab, references.MonsterTransform.position, Quaternion.identity);
        Vector2 direction;
        Quaternion rotation;

        if (enablePredict)
        {
            if (references.playerReferences.playerTransform.gameObject.GetComponent<PlayerMovement>().IsMoving())
            {
                direction = (references.playerReferences.playerPredictThrowableTransform.position - transform.position);
                direction.Normalize();
                float angleInRadians = Mathf.Atan2(direction.y, direction.x); // 聲gulo en radianes
                float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi鏮 a grados
                rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
            }
            else
            {
                direction = references.playerReferences.playerTransform.position - references.MonsterTransform.position;
                direction.Normalize();
                float angleInRadians = Mathf.Atan2(direction.y, direction.x); // 聲gulo en radianes
                float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi鏮 a grados
                rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
            }
        }
        else
        {
            direction = references.playerReferences.playerTransform.position - references.MonsterTransform.position;
            direction.Normalize();
            float angleInRadians = Mathf.Atan2(direction.y, direction.x); // 聲gulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi鏮 a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        go.GetComponent<ProjectileManager>().Initialize(direction, rotation, attack, 5f, references.monsterCollider);
    }

    #endregion

    #region Damage

    public void Damage(float damage, Vector2 kb)
    {
        if (isDefended)
        {
            defenseBubbleHP--;
        }
        else
        {
            if (!isHitted)
            {
                StartCoroutine(RoutineDamage(damage, kb));

                if (monsterHp <= 0 && GameManager.instance.isInBattle)
                {
                    GameManager.instance.TriggerBattleEnded(true);
                }
            }
        }
    }

    private IEnumerator RoutineDamage(float damage, Vector2 kb)
    {
        isHitted = true;

        references.monsterSprite.color = damageColor;
        monsterHp -= damage;
        ApplyForce(kb);

        yield return new WaitForSeconds(hittedCooldown);

        references.monsterSprite.color = originalColor;
        isHitted = false;
    }

    #endregion

    #region Defense
    private void Defense()
    {
        defenseBroken = defenseBubbleHP <= 0;

        isDefended = references.state == States.Defensing;

        DefenseVisuals();

        if (defenseBroken)
        {
            if (BubbleBreakRoutine != null)
            {
                StopCoroutine(BubbleBreakRoutine);
                return;
            }

            BubbleBreakRoutine = StartCoroutine(BubbleBreakCoroutine());
        }
    }

    private void DefenseVisuals()
    {
        defenseBubbleGO.SetActive(isDefended);
    }

    private IEnumerator BubbleBreakCoroutine()
    {
        DefenseBroke?.Invoke();
        yield return new WaitForSeconds(defenseBubbleBrokenCooldown);
        DefenseRegenerated?.Invoke();
        defenseBubbleHP = initialDefenseBubbleHP;
        BubbleBreakRoutine = null;
    }

    #endregion
}
