using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float actualSpeed;
    [Tooltip("The distance limit to still following the navmesh agent")]
    [SerializeField] private float stopDistance;
    private bool goToNextPoint = true;

    [Header("Combat")]
    [SerializeField] private Monster actualMonster;
    [SerializeField] private float meleeRange;

    [Header("Combat Stats")]
    [SerializeField] private float monsterHp;

    [Header("Defense")]
    [SerializeField] private float initialDefenseBubbleHP;
    [SerializeField] private float defenseBubbleHP;
    [SerializeField] private float defenseBubbleBrokenCooldown;
    [SerializeField] private bool isDefended;

    [SerializeField] private AIReferences references;

    private void Start()
    {
        references.stateMachineController.AttackStarted += OnAttackStarted;
    }

    private void OnDestroy()
    {
        references.stateMachineController.AttackStarted -= OnAttackStarted;
    }

    #region Getter / Setter

    /// <summary>
    /// Return isDefended value
    /// </summary>
    /// <returns></returns>
    public bool GetIsDefended() { return isDefended; }

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

    /// <summary>
    /// Check if the AI can defense
    /// </summary>
    /// <returns></returns>
    public bool CanDefense()
    {
        if (defenseBubbleHP <= 0)
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

        return true;
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        AgentMovement();
        Defense();
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
                RandomPoint(new Vector3(0, 0, 0), 5, out Vector2 result);
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
        if (references.state == States.Wandering)
        {
            //Follow the NavMesh Agent
            Vector2 velocity = new Vector2(references.agent.transform.position.x, references.agent.transform.position.y) - references.rb.position;
            references.rb.velocity = velocity.normalized * actualSpeed;
        }
        else if (references.state == States.Approach)
        {
            //Go directly to the player
            Vector2 velocity = (references.playerReferences.playerTransform.position - references.MonsterTransform.position).normalized;
            references.rb.velocity = velocity * actualSpeed;
        }
        else if (references.state == States.RangedAttack)
        {
            //Follow the NavMesh Agent
            Vector2 velocity = new Vector2(references.agent.transform.position.x, references.agent.transform.position.y) - references.rb.position;
            references.rb.velocity = velocity.normalized * actualSpeed;
        }
        else
        {
            references.rb.velocity = Vector2.zero;
        }
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
        Vector2 randomPoint = center + Random.insideUnitSphere * range;
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
        Vector2 direction = references.playerReferences.playerPredictThrowableTransform.position - references.MonsterTransform.position;
        direction.Normalize();
        float angleInRadians = Mathf.Atan2(direction.y, direction.x); // 聲gulo en radianes
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversi鏮 a grados
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));

        GameObject go = Instantiate(attack.meleePrefab, references.playerReferences.playerTransform.position, rotation);
        IDamageable damageable = references.playerReferences.GetComponent<IDamageable>();
        damageable.Damage(attack.attackDamage);
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

        go.GetComponent<ProjectileManager>().Initialize(direction, rotation, attack.attackDamage, attack.throwableSpeed, 5f, references.monsterCollider);
    }

    #endregion

    #region Defense
    private void Defense()
    {
        isDefended = references.state == States.Defensing;
    }
    #endregion
}
