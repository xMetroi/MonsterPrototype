using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerReferences;
using UnityEngine.InputSystem;
using System;

public class TrainerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    private PlayerInputs controls;

    private Vector2 movInput;

    private bool canMove = true;

    private SpriteRenderer spriteRenderer;

    public PlayerDevice device;

    public enum PlayerDevice
    {
        Controller,
        KeyboardMouse
    }

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

    public bool CanMove()
    {
        if (GameManager.instance.isInBattle)
        {
            return false;
        }
        if (FindObjectOfType<TrainerEnemyMovement>().canSeePlayer)
        {
            return false;
        }
        if (!FindObjectOfType<SelectMonsterToWin>().canSelect)
        {
            return false;
        }
        if (GameManager.instance.isUiOpened)
        {
            return false;
        }

        return true;
    }

    #region Events

    public event Action<PlayerDevice> DeviceChanged;

    #endregion

    /// <summary>
    /// Triggers when the device changed
    /// </summary>
    /// <param name="playerInput"></param>
    public void SetPlayerDevice(PlayerInput playerInput)
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard && Mouse":
                device = PlayerDevice.KeyboardMouse;
                DeviceChanged?.Invoke(device);
                Debug.Log("Kry");
                return;
            case "Controller":
                device = PlayerDevice.Controller;
                DeviceChanged?.Invoke(device);
                Debug.Log("Xbox");
                return;
        }
    }
}
