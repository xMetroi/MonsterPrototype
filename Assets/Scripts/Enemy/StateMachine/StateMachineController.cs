using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineController : MonoBehaviour
{
    [Header("Combat")]

    [Tooltip("Basically the general cooldown between attacks")]
    [SerializeField] private float attackCooldown;
    private float attackTimer;

    [Tooltip("How long the aproach last, x is min, y is max")]
    [SerializeField] private Vector2 aproachMinMaxTime;
    private float aproachTimer;
    private bool attack = true; // can change to a attack state
    private bool aproach = false;

    [Tooltip("How long takes to the next aproach")]
    [SerializeField] private Vector2 nextAproachMinMaxTime;
    private float nextAproachTime;

    [Header("Skills")]
    private bool canUseAttack1 = true;
    private bool canUseAttack2 = true;
    private bool canUseSpecial = true;

    [Header("Defense")]
    [Tooltip("How long the ai gonna defend, x is min, y is max")]
    [SerializeField] private Vector2 defenseMinMaxTime;
    private float defenseTimer;
    private bool defense = true;

    [SerializeField] private AIReferences references;

    #region Events

    //Attack
    public event Action<Attack> AttackStarted;

    //Defense
    public event Action<float> DefenseStarted;
    public event Action<float> DefenseFinished;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        float nextAproachTime = UnityEngine.Random.Range(nextAproachMinMaxTime.x, nextAproachMinMaxTime.y);
        SetNextAproach(nextAproachTime);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachineManager();
        ResetStates();
    }

    private (bool hasMeleeAttackAvailable, Attack availableAttack) HasMeleeAttacksAvailable()
    {
        if (references.currentMonster.basickAttack1?.attackType == Attack.AttackType.Melee && canUseAttack1)
            return (true, references.currentMonster.basickAttack1);

        if (references.currentMonster.basickAttack2?.attackType == Attack.AttackType.Melee && canUseAttack2)
            return (true, references.currentMonster.basickAttack2);

        if (references.currentMonster.specialAttack?.attackType == Attack.AttackType.Melee && canUseSpecial)
            return (true, references.currentMonster.specialAttack);

        return (false, null); // No hay ataque cuerpo a cuerpo disponible
    }

    private (bool hasThrowableAttackAvailable, Attack availableAttack) HasThrowableAttacksAvailable()
    {
        if (references.currentMonster.basickAttack1?.attackType == Attack.AttackType.Throwable && canUseAttack1)
            return (true, references.currentMonster.basickAttack1);

        if (references.currentMonster.basickAttack2?.attackType == Attack.AttackType.Throwable && canUseAttack2)
            return (true, references.currentMonster.basickAttack2);

        if (references.currentMonster.specialAttack?.attackType == Attack.AttackType.Throwable && canUseSpecial)
            return (true, references.currentMonster.specialAttack);

        return (false, null); // No hay ataque cuerpo a cuerpo disponible
    }

    #region State Machine

    private void StateMachineManager()
    {
        //Wandering is the initial / default state
        if (references.state == States.Wandering)
        {
            if (aproach) // if aproach is started
            {
                if (references.state == States.Wandering)
                {
                    // Debug.Log("Aproach");
                    float aproachTime = UnityEngine.Random.Range(aproachMinMaxTime.x, aproachMinMaxTime.y);
                    // Debug.Log(aproachTime);
                    references.SetState(States.Approach);
                    StartAproach(aproachTime);
                }
            }
            else
            {
                if (references.brain.HasRangedAttack()) // if the monster has atleast 1 ranged attack:
                {
                    if (references.brain.IsInMeleeRange(references.playerReferences.playerTransform.position)) //if is the melee range
                    {

                        var (hasMeleeAttackAvailable, availableAttack) = HasMeleeAttacksAvailable();

                        if (attack && references.brain.CanAttack() && hasMeleeAttackAvailable) //if the monster can attack and have a melee attack available (not in cooldown)
                        {
                            if (references.brain.GetDistanceWithPlayer() <= availableAttack.meleeRange) //if the distance with the player is in attack reach
                            {
                                // Debug.Log("Melee Attack");
                                AttackStarted?.Invoke(availableAttack);
                                StartAttack(attackCooldown, Attack.AttackType.Melee);
                            }
                        }
                        else //defend
                        {
                            if (defense && references.brain.CanDefense())
                            {
                                float defenseDuration = UnityEngine.Random.Range(defenseMinMaxTime.x, defenseMinMaxTime.y);
                                StartDefense(defenseDuration);

                                Debug.Log("Defend Started");
                                Debug.Log("Defense Duration: " + defenseDuration);
                            }
                        }
                    }
                    else // if is not in the melee range
                    {
                        var (hasThrowableAttackAvailable, availableAttack) = HasThrowableAttacksAvailable();

                        if (attack && references.brain.CanAttack() && hasThrowableAttackAvailable)
                        {
                            Debug.Log("Ranged Attack");
                            AttackStarted?.Invoke(availableAttack);
                            StartAttack(attackCooldown, Attack.AttackType.Throwable);
                        }
                    }
                }
                else //if the monster dont have a ranged attack:
                {
                    if (references.brain.IsInMeleeRange(references.playerReferences.playerTransform.position)) //if is in the melee range
                    {
                        var (hasMeleeAttackAvailable, availableAttack) = HasMeleeAttacksAvailable();

                        if (attack && references.brain.CanAttack() && hasMeleeAttackAvailable) //if the monster can attack and have a melee attack available (not in cooldown)
                        {
                            if (references.brain.GetDistanceWithPlayer() <= availableAttack.meleeRange) //if the distance with the player is in attack reach
                            {
                                Debug.Log("Melee Attack");
                                AttackStarted?.Invoke(availableAttack);
                                StartAttack(attackCooldown, Attack.AttackType.Melee);
                            }
                        }
                        else //defend
                        {
                            if (defense && references.brain.CanDefense())
                            {
                                float defenseDuration = UnityEngine.Random.Range(defenseMinMaxTime.x, defenseMinMaxTime.y);
                                StartDefense(defenseDuration);

                                Debug.Log("Defend Started");
                                Debug.Log("Defense Duration: " + defenseDuration);
                            }
                        }
                    }
                }
            }          
        }

        if (references.state == States.Approach)
        {
            if (references.brain.IsInMeleeRange(references.playerReferences.playerTransform.position)) // if is in the melee range
            {
                var (hasMeleeAttackAvailable, availableAttack) = HasMeleeAttacksAvailable();

                if (attack && references.brain.CanAttack() && hasMeleeAttackAvailable) //if the monster can attack and have a melee attack available (not in cooldown)
                {
                    if (references.brain.GetDistanceWithPlayer() <= availableAttack.meleeRange) //if the distance with the player is in attack reach
                    {
                        Debug.Log("Melee Attack");
                        AttackStarted?.Invoke(availableAttack);
                        StartAttack(attackCooldown, Attack.AttackType.Melee);
                    }
                }
                else //defend
                {
                    if (defense && references.brain.CanDefense())
                    {
                        float defenseDuration = UnityEngine.Random.Range(defenseMinMaxTime.x, defenseMinMaxTime.y);
                        StartDefense(defenseDuration);

                        Debug.Log("Defend Started");
                        Debug.Log("Defense Duration: " + defenseDuration);
                    }
                }
            }
            else 
            {

            }
        }
    }

    private void ResetStates()
    {
        if (references.state == States.MeleeAttack || references.state == States.RangedAttack)
        {
            if (attackTimer <= 0) //if the ai is still in the melee attack or ranged attack state, checks if the attack timers reach zero to reset the state
            {
                if (aproachTimer <= 0) //if the aproach is over, return to wandering
                    references.SetState(States.Wandering);
                else //if the aproach is not finished, return to approach
                    references.SetState(States.Approach);
            }
        }
        else if (references.state == States.Approach)
        {
            if (aproachTimer <= 0) //if the aproach finished return to wandering
                references.SetState(States.Wandering);
        }
        else if (references.state == States.Defensing)
        {
            if (defenseTimer <= 0) //check if the defense finished
            {
                if (aproachTimer <= 0) //if the aproach is over, return to wandering
                    references.SetState(States.Wandering);
                else //if the aproach is not finished, return to approach
                    references.SetState(States.Approach);
            }
        }
    }

    private void StartAttack(float time, Attack.AttackType attackType)
    {
        if (attackType == Attack.AttackType.Melee)
            references.SetState(States.MeleeAttack);
        else if (attackType == Attack.AttackType.Throwable)
            references.SetState(States.RangedAttack);

        StartCoroutine(StartAttackCoroutine(time, attackType));
    }

    private IEnumerator StartAttackCoroutine(float time, Attack.AttackType attackType)
    {       
        Attack basicAttack1 = references.currentMonster.basickAttack1;
        Attack basicAttack2 = references.currentMonster.basickAttack2;
        Attack specialAttack = references.currentMonster.specialAttack;

        attack = false;

        if (basicAttack1.attackType == attackType && canUseAttack1)
        {
            Debug.Log("Basic Attack 1 applied");
            StartCoroutine(Attack1CooldownCoroutine(basicAttack1.attackCooldown));
            FindObjectOfType<CombatUI>().StartCooldownEnemyAttack1(basicAttack1.attackCooldown);

        }
        else if (basicAttack2.attackType == attackType && canUseAttack2)
        {
            Debug.Log("Basic Attack 2 applied");
            StartCoroutine(Attack2CooldownCoroutine(basicAttack2.attackCooldown));
            FindObjectOfType<CombatUI>().StartCooldownEnemyAttack2(basicAttack2.attackCooldown);

        }
        else if (specialAttack.attackType == attackType && canUseSpecial)
        {
            Debug.Log("Special Attack applied");
            StartCoroutine(SpecialCooldownCoroutine(specialAttack.attackCooldown));
            FindObjectOfType<CombatUI>().StartCooldownEnemyAttack3(specialAttack.attackCooldown);

        }

        attackTimer = time;  // Set the timer

        while (attackTimer > 0)
        {
            yield return null;  // Wait a frame
            attackTimer -= Time.deltaTime;  // Incrementa el tiempo transcurrido
        }

        attack = true;
    }

    private IEnumerator Attack1CooldownCoroutine(float time)
    {
        canUseAttack1 = false;
        yield return new WaitForSeconds(time);
        canUseAttack1 = true;
    }
    private IEnumerator Attack2CooldownCoroutine(float time)
    {
        canUseAttack2 = false;
        yield return new WaitForSeconds(time);
        canUseAttack2 = true;
    }
    private IEnumerator SpecialCooldownCoroutine(float time)
    {
        canUseSpecial = false;
        yield return new WaitForSeconds(time);
        canUseSpecial = true;
    }

    private void StartDefense(float time)
    {
        references.SetState(States.Defensing);
        StartCoroutine(DefenseDurationCoroutine(time));
    }

    private IEnumerator DefenseDurationCoroutine(float time)
    {
        defense = false;

        defenseTimer = time;  // Reiniciar el tiempo transcurrido

        DefenseStarted?.Invoke(references.brain.GetBubbleHP());

        // Contar el tiempo mientras se espera
        while (defenseTimer > 0)
        {
            yield return null;  // Espera un frame
            defenseTimer -= Time.deltaTime;  // Incrementa el tiempo transcurrido
        }

        defense = true;
        DefenseFinished?.Invoke(references.brain.GetBubbleHP());
    }

    private void StartAproach(float time)
    {
        StartCoroutine(StartAproachCoroutine(time));
    }

    private IEnumerator StartAproachCoroutine(float time)
    {
        aproach = false;

        aproachTimer = time;  // Reiniciar el tiempo transcurrido

        // Contar el tiempo mientras se espera
        while (aproachTimer > 0)
        {
            yield return null;  // Espera un frame
            aproachTimer -= Time.deltaTime;  // Incrementa el tiempo transcurrido
        }

        float nextAproachTime = UnityEngine.Random.Range(nextAproachMinMaxTime.x, nextAproachMinMaxTime.y);
        SetNextAproach(nextAproachTime);
    }

    private void SetNextAproach(float time)
    {
        StartCoroutine(NextAproachCoroutine(time));
    }

    private IEnumerator NextAproachCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        aproach = true;
    }

    #endregion
}
