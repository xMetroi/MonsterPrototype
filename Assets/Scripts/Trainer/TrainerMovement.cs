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
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

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

            transform.rotation = Quaternion.Euler(0, movInput.x > 0 ? 0 : movInput.x < 0 ? 180f : transform.rotation.eulerAngles.y, 0);
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
