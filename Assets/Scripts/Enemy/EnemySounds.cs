using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [SerializeField] AIReferences references;

    void Start()
    {
        //Events suscriptions

        //Attack
        references.stateMachineController.AttackStarted += OnAttackStarted;

        //Hit
        references.brain.StartHitted += OnEnemyStartHitted;
        references.brain.StopHitted += OnEnemyStopHitted;

        //Defense
        references.stateMachineController.DefenseStarted += OnDefenseStarted;
        references.stateMachineController.DefenseFinished += OnDefenseStopped;
        references.brain.HitDefensed += OnHitDefensed;
        references.brain.DefenseBroke += OnDefenseBroken;
        references.brain.DefenseRegenerated += OnDefenseRegenerated;
    }

    private void OnDestroy()
    {
        //Events desuscriptions

        //Attack
        references.stateMachineController.AttackStarted -= OnAttackStarted;

        //Hit
        references.brain.StartHitted -= OnEnemyStartHitted;
        references.brain.StopHitted -= OnEnemyStopHitted;

        //Defense
        references.stateMachineController.DefenseStarted -= OnDefenseStarted;
        references.stateMachineController.DefenseFinished -= OnDefenseStopped;
        references.brain.HitDefensed -= OnHitDefensed;
        references.brain.DefenseBroke -= OnDefenseBroken;
        references.brain.DefenseRegenerated -= OnDefenseRegenerated;
    }

    /// <summary>
    /// Triggers when enemy starts a attack
    /// </summary>
    /// <param name="attack"></param>
    private void OnAttackStarted(Attack attack)
    {

    }

    /// <summary>
    /// Triggers when the enemy gets hitted
    /// </summary>
    /// <param name="damage"></param>
    private void OnEnemyStartHitted(float damage)
    {

    }

    /// <summary>
    /// Triggers when the hit cooldown ends
    /// </summary>
    private void OnEnemyStopHitted()
    {

    }

    /// <summary>
    /// Triggers when enemy start defense
    /// </summary>
    private void OnDefenseStarted(float bubbleHP)
    {

    }

    /// <summary>
    /// Triggers when enemy stop defense
    /// </summary>
    /// <param name="bubbleHP"></param>
    private void OnDefenseStopped(float bubbleHP)
    {

    }

    /// <summary>
    /// Triggers when a hit is defensed with the bubble
    /// </summary>
    /// <param name="bubbleHP"></param>
    private void OnHitDefensed(float bubbleHP)
    {

    }

    /// <summary>
    /// Triggers when the defense reachs zero
    /// </summary>
    private void OnDefenseBroken()
    {

    }

    /// <summary>
    /// Triggers when the defense cooldowns end after being broke
    /// </summary>
    private void OnDefenseRegenerated()
    {

    }
}
