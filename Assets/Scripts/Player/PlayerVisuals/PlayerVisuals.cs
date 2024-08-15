using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Defense Bubble Properties")]
    [SerializeField] private GameObject defenseBubbleGO;

    [SerializeField] private PlayerReferences references;

    private void Start()
    {
        //Events subscriptions
        references.playerCombat.StartDefense += DefenseStarted;
        references.playerCombat.StopDefense += DefenseStopped;

        //Set the sprite of the actual monster
        references.monsterSprite.sprite = references.currentMonster.prefabMonster.GetComponent<SpriteRenderer>().sprite;
    }

    private void OnDisable()
    {
        //Events desubscription
        references.playerCombat.StartDefense -= DefenseStarted;
        references.playerCombat.StopDefense -= DefenseStopped;
    }

    void Update()
    {
        
    }

    #region Combat

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

    #endregion

    #endregion
}
