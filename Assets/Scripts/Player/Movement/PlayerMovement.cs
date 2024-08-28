using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float aimingSpeed;
    [Space]
    [SerializeField] private Vector2 forceToApply;
    [SerializeField] private float forceDamping;

    public Transform predictionPivot;
    
    [Space]
    private Vector2 movementInput;
    private Vector2 movementDirection;
    private Vector2 lastMovementInput = new Vector2(1, 0);
    private Animator animator;

    [Header("Combat Properties")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;

    [SerializeField] private PlayerReferences references;

    #region Getter / Setter

    public void SetMovementSpeeds(float movementSpeed, float aimingSpeed) { this.movementSpeed = movementSpeed; this.aimingSpeed = aimingSpeed; }

    public Vector2 GetMovementInput() { return movementInput; }
    public Vector2 GetLastMovementInput() { return lastMovementInput; }

    public bool IsMoving() { return references.rb.velocity.magnitude > 0.1f; }

    #endregion

    #region Checkers / Utilities

    /// <summary>
    /// Determines if the player can move
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        if (references.playerCombat.isDefensed)
            return false;

        if (references.playerCombat.GetIsHitted())
            return false;

        if (!GameManager.instance.isInBattle)
            return false;

        return true;
    }

    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (references.currentMonster != null)
        {
            movementSpeed = references.currentMonster.monsterSpeed;
            aimingSpeed = references.currentMonster.monsterAimSpeed;
            dashForce = references.currentMonster.monsterDashForce;
            dashCooldown = references.currentMonster.monsterDashCooldown;

        }

        references.playerCombat.StartDefense += DefenseStarted;
        references.playerCombat.StopDefense += DefenseStopped;

    }

    private void OnDisable()
    {
        references.playerCombat.StartDefense -= DefenseStarted;
        references.playerCombat.StopDefense -= DefenseStopped;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        MovePivot();
        animator.SetFloat("speed", movementInput.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void GetInputs()
    {
        if (CanMove())
            movementInput = references.playerInputs.Movement.Movement.ReadValue<Vector2>();
        else
            movementInput = Vector2.zero;

        movementDirection = movementInput.normalized * currentSpeed;

        if (movementInput != Vector2.zero)
            lastMovementInput = movementInput;

        if (references.playerInputs.Movement.Dash.WasPerformedThisFrame() && canDash)
        {
            if (CanMove())
                StartCoroutine(Dash());
        }
    }

    private void MovePivot()
    {
        if (movementInput.sqrMagnitude != 0)
        {
            float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            predictionPivot.transform.eulerAngles = new Vector3(0, 0, angle);
        } 
    }

    private void Movement()
    {                
        if (references.playerAim.isAiming)
            currentSpeed = aimingSpeed;
        else
            currentSpeed = movementSpeed;
        
        transform.rotation = Quaternion.Euler(0, movementInput.x > 0 ? 0 : movementInput.x < 0 ? 180f : transform.rotation.eulerAngles.y, 0);

        movementDirection += forceToApply;
        forceToApply /= forceDamping;

        if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
        {
            forceToApply = Vector2.zero;
        }
       
        references.rb.velocity = movementDirection;
    }

    public void ApplyForce(Vector2 force)
    {
        forceToApply = force;
    }

    /// <summary>
    /// triggers when the player start the defense 
    /// </summary>
    /// <param name="bubbleHP"> hp of the bubble </param>
    private void DefenseStarted(float bubbleHP)
    {
        references.rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// triggers when the player stops the defense
    /// </summary>
    /// <param name="bubbleHP"> hp of the bubble</param>
    private void DefenseStopped(float bubbleHP)
    {

    }

    private IEnumerator Dash()
    {
        canDash = false;
        forceToApply = lastMovementInput * dashForce;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
}
