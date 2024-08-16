using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReferences : MonoBehaviour
{

    /// <summary>
    /// In this scrips we are gonna add all the references needed
    /// we do this to avoid create multiple references in multiples scripts
    /// </summary>

    public enum PlayerDevice
    {
        Controller,
        KeyboardMouse
    }

    public PlayerDevice device;

    public Monster currentMonster;

    [Header("Player Sprites")]
    public SpriteRenderer monsterSprite;
    public Animator monsterAnimator;

    #region Scripts
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerAim playerAim;
    [HideInInspector] public PlayerCombat playerCombat;
    #endregion

    #region Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PlayerInputs playerInputs;
    [HideInInspector] public PlayerInput playerInput;
    #endregion

    private void Awake()
    {
        monsterAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAim = GetComponent<PlayerAim>();
        playerCombat = GetComponent<PlayerCombat>();

        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerInputs = new PlayerInputs();
        playerInputs.Movement.Enable();
        playerInputs.Interactions.Enable();
        playerInputs.Combat.Enable();
    }

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
                return;
            case "Controller":
                device = PlayerDevice.Controller;
                return;
        }
    }
}
