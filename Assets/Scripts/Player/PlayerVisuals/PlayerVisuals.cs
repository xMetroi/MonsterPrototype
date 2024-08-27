using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Defense Bubble Properties")]
    [SerializeField] private GameObject defenseBubbleGO;

    [Header("Combat Visuals")]
    [SerializeField] private Color damageColor;
    private Color originalColor;

    [SerializeField] private PlayerReferences references;

    private void Start()
    {
        SetMonsterData();
    }

    public void SetMonsterData()
    {
        //Initialize
        originalColor = Color.white;

        //Events subscriptions

        //Defense
        references.playerCombat.StartDefense += DefenseStarted;
        references.playerCombat.StopDefense += DefenseStopped;
        references.playerCombat.DefenseBroke += OnDefenseBroken;
        references.playerCombat.HitDefensed += OnHitDefensed;

        //Hit
        references.playerCombat.StartHitted += OnPlayerStartHitted;
        references.playerCombat.StopHitted += OnPlayerStopHitted;

        //Set the sprite of the actual monster
        references.monsterSprite.sprite = references.currentMonster.monsterSprite;
        //references.monsterSprite.color = originalColor;

        Debug.Log(references.currentMonster.monsterAnimator.name);
        references.monsterAnimator.runtimeAnimatorController = references.currentMonster.monsterAnimator;
    }

    private void OnDisable()
    {
        //Events desubscription

        //Defense
        references.playerCombat.StartDefense -= DefenseStarted;
        references.playerCombat.StopDefense -= DefenseStopped;
        references.playerCombat.DefenseBroke -= OnDefenseBroken;
        references.playerCombat.HitDefensed -= OnHitDefensed;

        //Hit
        references.playerCombat.StartHitted -= OnPlayerStartHitted;
        references.playerCombat.StopHitted -= OnPlayerStopHitted;
    }

    #region Combat

    #region Hitted

    /// <summary>
    /// Triggers when the player is hitted
    /// </summary>
    /// <param name="damage"></param>
    private void OnPlayerStartHitted(float damage)
    {
        references.monsterSprite.color = damageColor;
    }

    /// <summary>
    /// Triggers when the hit time ends
    /// </summary>
    private void OnPlayerStopHitted()
    {
        references.monsterSprite.color = originalColor;
    }

    #endregion

    #region Defense

    /// <summary>
    /// Triggers when defense starts
    /// </summary>
    /// <param name="bubbleHP"></param>
    private void DefenseStarted(float bubbleHP)
    {
        defenseBubbleGO.SetActive(true);
        defenseBubbleGO.GetComponent<Animator>().SetTrigger("StartDefense");
        defenseBubbleGO.GetComponent<Animator>().SetInteger("DefenseBubbleHP", (int)bubbleHP);
    }

    /// <summary>
    /// Triggers when defense stops
    /// </summary>
    /// <param name="bubbleHP"></param>
    private void DefenseStopped(float bubbleHP)
    {
        defenseBubbleGO.SetActive(false);
    }

    /// <summary>
    /// Triggers when defenseHP reach zero or less
    /// </summary>
    private void OnDefenseBroken()
    {
        defenseBubbleGO.SetActive(false);
    }

    /// <summary>
    /// Triggers when bubble defends a hit
    /// </summary>
    private void OnHitDefensed(float bubbleHP)
    {
        defenseBubbleGO.GetComponent<Animator>().SetInteger("DefenseBubbleHP", (int)bubbleHP);
    }

    #endregion

    #endregion
}
