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

    [Header("Player Properties")]
    public PlayerReferences playerReferences;

    [Header("AI Properties")]
    public Transform MonsterTransform;
    public Collider2D monsterCollider;

    [Header("State Machine")]
    public States state;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator sm;
    [HideInInspector] public EnemyBrain brain;
    [HideInInspector] public StateMachineController stateMachineController;

    private void Awake()
    {
        //playerReferences = GameObject.Find("Player").GetComponent<PlayerReferences>();
        playerReferences = FindObjectOfType<PlayerReferences>();


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
