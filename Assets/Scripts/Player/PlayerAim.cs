using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("Aim Properties")]
    [SerializeField] private GameObject pointerGO;
    [HideInInspector] public bool isAiming;
    private Vector2 rightMouseInput;
    private Vector2 lastrightMouseInput;

    [SerializeField] private PlayerReferences references;
    // Update is called once per frame
    void Update()
    {
        GetInputs();
        HandlePointerAiming();
    }

    /// <summary>
    /// Here get the inputs
    /// </summary>
    private void GetInputs()
    {
        isAiming = references.playerInputs.Interactions.Aim.IsPressed();
        rightMouseInput = references.playerInputs.Interactions.AimLook.ReadValue<Vector2>();
    }

    /// <summary>
    /// Method in charge of handle the aiming arrow
    /// 
    /// pointerGO is the arrow gameobject
    /// </summary>
    private void HandlePointerAiming()
    {
        //if the actual device is the keyboard&&mouse
        if (references.device == PlayerReferences.PlayerDevice.KeyboardMouse)
        {
            if (isAiming)
            {
                pointerGO.SetActive(true);
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector3 aimDirection = (mousePos - transform.position).normalized;
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                pointerGO.transform.eulerAngles = new Vector3(0, 0, angle);
            }
            else
                pointerGO.SetActive(false);
        }
        //if the actual device is the controller
        else if (references.device == PlayerReferences.PlayerDevice.Controller)
        {
            if (isAiming)
            {
                pointerGO.SetActive(true);
                if (rightMouseInput != Vector2.zero)
                {                    
                    lastrightMouseInput = rightMouseInput;
                    float angle = Mathf.Atan2(rightMouseInput.y, rightMouseInput.x) * Mathf.Rad2Deg;
                    pointerGO.transform.eulerAngles = new Vector3(0, 0, angle);
                }
                else
                {
                    float angle = Mathf.Atan2(lastrightMouseInput.y, lastrightMouseInput.x) * Mathf.Rad2Deg;
                    pointerGO.transform.eulerAngles = new Vector3(0, 0, angle);
                }

            }
            else
                pointerGO.SetActive(false);


        }
    }
}
