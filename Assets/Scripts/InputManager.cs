using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private PlayerAttacker playerAttacker;
    private PlayerInventory playerInventory;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool b_Input;
    public bool jump_Input;
    public bool x_Input;

    public bool rb_Input;
    public bool rt_Input;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.B.performed += i => b_Input = true;
            playerControls.PlayerActions.B.canceled += i => b_Input = false;
            playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
            playerControls.PlayerActions.X.performed += i => x_Input = true;

            playerControls.PlayerActions.RB.performed += i => rb_Input = true;
            playerControls.PlayerActions.RT.performed += i => rt_Input = true;


        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleDodgeInput();
        //HandleActionInput()
        HandleAttackInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (b_Input)
        {
            if(movementInput.magnitude > 0)
            {
                playerLocomotion.isSprinting = true;
            }
            else
            {
                playerLocomotion.isSprinting = false;
            }
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }

    }

    private void HandleDodgeInput()
    {
        if (x_Input)
        {
            x_Input = false;
            if (Mathf.Abs(movementInput.y) > 0 || Mathf.Abs(movementInput.x) > 0)
            {
                playerLocomotion.HandleRolling();
            }
            else
            {
                playerLocomotion.HandleDodge();
            }
        }
    }

    private void HandleAttackInput()
    {


        if (rt_Input)
        {
            rt_Input = false;
            if (movementInput.magnitude >= 1)
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon, true);
            }
            else
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }
        else if (rb_Input)
        {
            print("Speed while running: " + movementInput.magnitude);
            rb_Input = false;
            if (movementInput.magnitude >= 1)
            {
                playerAttacker.HandleLightAttack(playerInventory.rightWeapon, true);
            }
            else
            {
                playerAttacker.HandleLightAttack(playerInventory.rightWeapon, false);
            }
            

            
        }

    }
}
