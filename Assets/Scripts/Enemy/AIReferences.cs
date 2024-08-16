using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIReferences : MonoBehaviour
{
    [Header("Info")]
    public Monster currentMonster;
    public SpriteRenderer monsterSprite;
    public Animator monsterAnimator;
    [Space]
    public Transform playerTransform;
    public Transform predicitonPlayer;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public NavMeshAgent agent;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        monsterSprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();
    }
}
