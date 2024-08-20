using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIReferences : MonoBehaviour
{
    [Header("Info")]
    public Monster currentMonster;
    public SpriteRenderer monsterSprite;
    public Animator monsterAnimator;
    [Space]
    [Header("Player Properties")]
    public Transform playerTransform;
    public Transform predicitonPlayer;
    public Transform predicitonPlayerMelee;

    [Header("AI Properties")]
    public Transform MonsterTransform;

    [Header("State Machine")]
    public States state;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator sm;
    [HideInInspector] public EnemyBrain brain;
    [HideInInspector] public StateMachineController stateMachineController;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        monsterSprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();
        sm = GetComponent<Animator>();
        brain = GetComponent<EnemyBrain>();
        stateMachineController = GetComponent<StateMachineController>();
    }

    public void SetState(States newState)
    {
        sm.SetTrigger(newState.ToString());
        this.state = newState;
    }

    public States GetState() { return this.state; }
}
