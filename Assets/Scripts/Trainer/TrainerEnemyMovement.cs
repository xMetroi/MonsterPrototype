using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerEnemyMovement : MonoBehaviour
{
    Transform playerTransform;
    [SerializeField] private float speed;
    [SerializeField] private float stopDistance;
    private Vector3 initialPosition;

    private Animator animator;

    public bool canSeePlayer;
    public bool isStop;
    

    private void Start()
    {
        initialPosition = transform.position;
        animator = GetComponent<Animator>();
        canSeePlayer = false;
        playerTransform = GameObject.FindAnyObjectByType<TrainerMovement>().transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= 4f && distance > stopDistance)
        {
            canSeePlayer = true;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
            animator.SetFloat("Speed", 1);

            float rotationY = playerTransform.position.x > transform.position.x ? 0 : 180f;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        else 
        {
            animator.SetFloat("Speed", 0);
        }

        if (canSeePlayer && !isStop)
        {
            isStop = true;
            GameObject.Find("DialogueSystem").transform.localScale = new Vector3(1, 1, 0);
            FindObjectOfType<TrainerMovement>().SetCanMove(false);
            GetComponent<DialogueTrigger>().TriggerDialogue();
            FindObjectOfType<VSManager>().SetImageEnemy(gameObject.GetComponent<SpriteRenderer>());
            FindObjectOfType<VSManager>().SetEnemyName(gameObject.GetComponent<DialogueTrigger>().name);
        }
    }

    public void ResetData()
    {
        transform.position = initialPosition;
        isStop = false;
        canSeePlayer = false;
    }
}
