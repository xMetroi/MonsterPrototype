using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float actualSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float aimingSpeed;
    private Vector2 lastDir;
    bool goToNextPoint = true;

    [Header("Combat Properties")]
    [SerializeField] private float monsterHp;
    [SerializeField] private float meleeRange;
    [SerializeField] private Attack basickAttack1;
    [SerializeField] private Attack basickAttack2;
    [SerializeField] private Attack specialAttack;
    private bool canUseBasicAttack1 = true;
    private bool canUseBasicAttack2 = true;
    private bool canUseSpecialAttack = true;

    [Header("Aim Properties")]
    [SerializeField] private GameObject pointerGO;
    [SerializeField] private bool isAiming;

    [SerializeField] private AIReferences references;

    bool aswd = true;
    private void Start()
    {
        if (references.currentMonster != null)
        {
            monsterHp = references.currentMonster.monsterHealth;
            basickAttack1 = references.currentMonster.basickAttack1;
            basickAttack2 = references.currentMonster.basickAttack2;
            specialAttack = references.currentMonster.specialAttack;
        }
    }

    public void SetMovementSpeeds(float movementSpeed, float aimingSpeed) { this.movementSpeed = movementSpeed; this.aimingSpeed = aimingSpeed; }

    private void Update()
    {
     
        if (Vector2.Distance(references.playerTransform.position, transform.position) <= meleeRange)
        {
            if (canUseBasicAttack1)
            {
                references.agent.SetDestination(references.playerTransform.position);

                StartCoroutine(StartBasicAttack1Cooldown(basickAttack1.attackCooldown));
            }
        }
        else
        {
            if (goToNextPoint)
            {
                RandomPoint(new Vector3(0, 0, 0), 5, out Vector2 result);
                references.agent.SetDestination(result);
                StartCoroutine(GoNextPointDelay(1));
            }

            if (canUseBasicAttack2)
            {
                StartCoroutine(StartBasicAttack2Cooldown(basickAttack2.attackCooldown));
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();    
    }

    #region Movement

    private void Movement()
    {
        //Follow the NavMesh Agent
        Vector2 velocity = (new Vector2(references.agent.nextPosition.x, references.agent.nextPosition.y) - references.rb.position);
        references.rb.velocity = velocity * actualSpeed;
        lastDir = velocity.normalized;
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

    /// <summary>
    /// Delay to activate go to next point variable
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator GoNextPointDelay(float seconds)
    {
        goToNextPoint = false;

        yield return new WaitForSeconds(seconds);

        goToNextPoint = true;
    }

    #endregion

    #region Combat

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
        Quaternion rotation;

        if (isAiming)
            direction = pointerGO.transform.right;
        else
            direction = lastDir;

        if (isAiming)
            rotation = pointerGO.transform.rotation;
        else
        {
            float angleInRadians = Mathf.Atan2(lastDir.y, lastDir.x); // Ángulo en radianes
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg; // Conversión a grados
            rotation = Quaternion.Euler(new Vector3(0, 0, angleInDegrees));
        }

        go.GetComponent<ProjectileManager>().Initialize(direction, rotation, attack.throwableSpeed, 5f, GetComponent<Collider2D>());
    }

    public void SpawnMelee(Attack attack)
    {
        Vector2 direction;
        Quaternion rotation;

        if (isAiming)
            direction = transform.position + pointerGO.transform.right;
        else
            direction = new Vector2(transform.position.x + lastDir.x, transform.position.y + lastDir.y);

        if (isAiming)
            rotation = pointerGO.transform.rotation;
        else
        {
            float angleInRadians = Mathf.Atan2(lastDir.y, lastDir.x); // Ángulo en radianes
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

        SetMovementSpeeds
        (
            references.currentMonster.monsterSpeed * attack.transformationSpeedMultiplier,
            references.currentMonster.monsterAimSpeed * attack.transformationSpeedMultiplier
        );

        yield return new WaitForSeconds(attack.transformationDuration);
        FindAnyObjectByType<PlayerController>().AssignPlayerPropertiesCollider(references.currentMonster.prefabMonster.GetComponent<BoxCollider2D>());
        references.monsterSprite.sprite = references.currentMonster.prefabMonster.GetComponent<SpriteRenderer>().sprite;
        references.monsterAnimator.runtimeAnimatorController = references.currentMonster.prefabMonster.GetComponent<Animator>().runtimeAnimatorController;

        SetMovementSpeeds
        (
            references.currentMonster.monsterSpeed,
            references.currentMonster.monsterAimSpeed
        );
    }


    public bool CanAttack()
    {
        /*
        if (isDefensed)
            return false;*/

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
                //Damage
            }
        }
    }

    #endregion
}
