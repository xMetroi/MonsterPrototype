using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private PlayerReferences references;

    void Start()
    {
        //Events subscription

        //Attack
        references.playerCombat.AttackStarted += OnAttackStarted;

        //Transformation
        references.playerCombat.TransformationStart += OnTransformationStarted;
        references.playerCombat.TransformationEnd += OnTransformationEnd;

        //Hit
        references.playerCombat.StartHitted += OnPlayerStartHitted;
        references.playerCombat.StopHitted += OnPlayerStopHitted;

        //Defense
        references.playerCombat.StartDefense += OnDefenseStarted;
        references.playerCombat.StopDefense += OnDefenseStopped;

        references.playerCombat.HitDefensed += OnHitDefensed;
        references.playerCombat.DefenseBroke += OnDefenseBroken;
        references.playerCombat.DefenseRegenerated += OnDefenseRegenerated;
    }

    private void OnDestroy()
    {
        //Events desubscription

        //Attack
        references.playerCombat.AttackStarted -= OnAttackStarted;

        //Transformation
        references.playerCombat.TransformationStart -= OnTransformationStarted;
        references.playerCombat.TransformationEnd -= OnTransformationEnd;

        //Hit
        references.playerCombat.StartHitted -= OnPlayerStartHitted;
        references.playerCombat.StopHitted -= OnPlayerStopHitted;

        //Defense
        references.playerCombat.StartDefense -= OnDefenseStarted;
        references.playerCombat.StopDefense -= OnDefenseStopped;

        references.playerCombat.HitDefensed -= OnHitDefensed;
        references.playerCombat.DefenseBroke -= OnDefenseBroken;
        references.playerCombat.DefenseRegenerated -= OnDefenseRegenerated;
    }

    /// <summary>
    /// Triggers when player starts a attack
    /// </summary>
    /// <param name="attack"></param>
    private void OnAttackStarted(Attack attack)
    {

    }

    /// <summary>
    /// Triggers when a player starts a transformation
    /// </summary>
    /// <param name="attack"></param>
    private void OnTransformationStarted(Attack attack)
    {

    }

    /// <summary>
    /// Triggers when a transformation ends
    /// </summary>
    /// <param name="attack"></param>
    private void OnTransformationEnd(Attack attack)
    {

    }

    /// <summary>
    /// Triggers when the player gets hitted
    /// </summary>
    /// <param name="damage"></param>
    private void OnPlayerStartHitted(float damage)
    {

    }

    /// <summary>
    /// Triggers when the hit cooldown ends
    /// </summary>
    private void OnPlayerStopHitted()
    {

    }

    /// <summary>
    /// Triggers when player start defense
    /// </summary>
    private void OnDefenseStarted(float bubbleHP)
    {

    }

    /// <summary>
    /// Triggers when player stop defense
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
