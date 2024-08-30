using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    private PlayerInputs controls;

    private Vector2 movInput;

    private bool canMove = true;

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    private void Start()
    {
        if(DataManager.Instance.LoadPlayerPosition() != Vector3.zero)
        {
            this.transform.position = DataManager.Instance.LoadPlayerPosition();
            FindObjectOfType<TrainerController>().pointPosition = DataManager.Instance.LoadPlayerPosition();
        } 

        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer =  GetComponent<SpriteRenderer>();

        FindObjectOfType<VSManager>().SetImagePlayer(gameObject.GetComponent<SpriteRenderer>());
    }

    void Update()
    {
        if (CanMove())
        {
            movInput = controls.Movement.Movement.ReadValue<Vector2>().normalized;
        }

        else 
        {
            movInput = Vector2.zero;
        }

        playerAnimator.SetFloat("Speed", movInput.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        if (movInput != Vector2.zero && CanMove())
        {
            playerRb.MovePosition(playerRb.position + movInput * speed * Time.fixedDeltaTime);
            spriteRenderer.flipX = movInput.x < 0;
        }
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public bool CanMove()
    {
        return canMove;
    }
}
