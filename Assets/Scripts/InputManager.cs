using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private StatusUI statusUI;
    private BuildInput_Custom buildInput_Custom;
    private UserControl playerControls;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private PlayerAttacker playerAttacker;
    private PlayerInventory playerInventory;
    private PlayerManager playerManager;
    private WeaponSlotManager weaponSlotManager;
    private UIManager uiManager;
    private CameraManager cameraManager;
    private ApplyEffect applyEffect;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool a_Input;
    public bool b_Input;
    public bool jump_Input;
    public bool x_Input;
    public bool y_Input;

    public bool rb_Input;
    public bool rt_Input;

    public bool quickslot1_Input;
    public bool quickslot2_Input;
    public bool quickslot3_Input;
    public bool quickslot4_Input;

    public bool buildMode_Input;

    public bool inventory_Input;
    public bool options_Input;

    public bool lockOnInput;
    public bool lockOnLeftInput;
    public bool lockOnRightInput;

    public bool effectInput;
    public bool initEffectFlag = true;

    public bool inventoryFlag;
    public bool optionFlag;
    public bool comboFlag;
    public bool lockOnFlag;
    public bool twoHandFlag;
    public bool buildModeFlag;

    private void Awake()
    {
        statusUI = FindObjectOfType<StatusUI>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        uiManager = FindObjectOfType<UIManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        applyEffect = GetComponent<ApplyEffect>();
        buildInput_Custom = FindObjectOfType<BuildInput_Custom>();
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new UserControl();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.B.performed += i => b_Input = true;
            playerControls.PlayerActions.B.canceled += i => b_Input = false;
            playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
            playerControls.PlayerActions.X.performed += i => x_Input = true;

            playerControls.PlayerActions.Interact.performed += i => a_Input = true;

            playerControls.PlayerActions.RB.performed += i => rb_Input = true;
            playerControls.PlayerActions.RT.performed += i => rt_Input = true;

            playerControls.PlayerActions.Quickslot1.performed += i => quickslot1_Input = true;
            playerControls.PlayerActions.Quickslot2.performed += i => quickslot2_Input = true;
            playerControls.PlayerActions.Quickslot3.performed += i => quickslot3_Input = true;
            playerControls.PlayerActions.Quickslot4.performed += i => quickslot4_Input = true;

            playerControls.PlayerActions.Inventory.performed += i => inventory_Input = true;
            playerControls.PlayerActions.Options.performed += i => options_Input = true;

            playerControls.PlayerActions.AutoLock.performed += i => lockOnInput = true;
            playerControls.PlayerActions.LockOnLeftTarget.performed += i => lockOnLeftInput = true;
            playerControls.PlayerActions.LockOnRightTarget.performed += i => lockOnRightInput = true;

            playerControls.PlayerActions.Effect.performed += i => effectInput = true;
            playerControls.PlayerActions.Y.performed += i => y_Input = true;
            playerControls.PlayerActions.BuildMode.performed += i => buildMode_Input = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        if (!uiManager.inventoryWindow.activeSelf)
        {
            HandleMovementInput();
            HandleSprintingInput();
            HandleJumpingInput();
            HandleDodgeInput();
            HandleQuickSlotsInput();
            HandleInteractingButtonInput();
            HandleLockOnInput();
            HandleEffectInput();
            HandleTwoHandInput();
            HandleBuildModeInput();
        }
        HandleAttackInput();

        HandleInventoryInput();
        HandleOptionInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        if (lockOnFlag && playerLocomotion.isSprinting == false)
        {
            animatorManager.UpdateAnimatorValues(horizontalInput, verticalInput, playerLocomotion.isSprinting);
        }
        else
        {
            animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
        }
        
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
            //playerAttacker.weaponSlotManager.CloseLeftDamageCollider();
            //playerAttacker.weaponSlotManager.CloseRightDamageCollider();
            Debug.Log("WTH is going on");
            if (Mathf.Abs(movementInput.y) > 0 || Mathf.Abs(movementInput.x) > 0)
            {
                playerLocomotion.HandleRolling();
            }
            else
            {
                playerLocomotion.HandleDodge();
            }

            x_Input = false;

        }
    }

    private void HandleAttackInput()
    {


        if (rt_Input)
        {
            rt_Input = false;
            if (uiManager.inventoryWindow.activeSelf)
            {
                return;
            }

            if (playerLocomotion.isSprinting)
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon, true);
            }
            else if (playerLocomotion.isGrounded)
            {
                playerLocomotion.playerRigidBody.velocity = Vector3.zero; //Stops the player from moving while picking up an item
                if (playerManager.canDoCombo)
                {
                    //Do Combo
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon, true);
                    comboFlag = false;
                }
                else 
                {
                    //First Attack
                    playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
                }
            }
        }
        else if (rb_Input)
        {
            //print("Speed while running: " + movementInput.magnitude);
            rb_Input = false;
            if (uiManager.inventoryWindow.activeSelf)
            {
                return;
            }

            if (playerLocomotion.isSprinting)
            {
                playerAttacker.HandleLightAttack(playerInventory.rightWeapon, true);
            }
            else if(playerLocomotion.isGrounded)
            {
                playerLocomotion.playerRigidBody.velocity = Vector3.zero; //Stops the player from moving while picking up an item
                if (playerManager.canDoCombo)
                {
                    //Do Combo
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {
                    //First Attack
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon, false);
                }
            }



        }

    }

    private void HandleQuickSlotsInput()
    {
        /*
         Notes: Video EP. 14 Weapon Quick Slots: 
            Link https://www.youtube.com/watch?v=qC9TPzXdxyk
         */
        if (!playerManager.isInteracting)
        {
            if (quickslot1_Input)
            {
                playerInventory.ChangeWeapon(0);
            }
            else if (quickslot2_Input)
            {
                playerInventory.ChangeWeapon(1);
            }
            else if (quickslot3_Input)
            {
                playerInventory.ChangeWeapon(2);
            }
            else if (quickslot4_Input)
            {
                playerInventory.ChangeWeapon(3);
            }
        }


    }

    private void HandleInteractingButtonInput()
    {
        if (a_Input)
        {
            //a_Input = false;
            //Debug.Log("Press A Input");
            
        }
    }

    private void HandleOptionInput()
    {
        if (options_Input)
        {
            

            if (uiManager.inventoryWindow.activeSelf)
            {
                inventoryFlag = false;
                uiManager.CloseInventoryWindow();
                uiManager.hudWindow.SetActive(true);
                optionFlag = false;
            }
            else
            {
                optionFlag = !optionFlag;
            }

            

            if (optionFlag)
            {
                uiManager.OpenSelectWindow();
            }
            else
            {
                uiManager.CloseSelectWindow();
            }

 
        }

    }
    private void HandleInventoryInput()
    {
        if (inventory_Input)
        {
            inventoryFlag = !inventoryFlag;

            if (inventoryFlag)
            {
                uiManager.OpenInventoryWindow();
                uiManager.UpdateUI();
                uiManager.hudWindow.SetActive(false);
            }
            else
            {
                uiManager.CloseInventoryWindow();
                uiManager.hudWindow.SetActive(true);
            }
        }
    }

    private void HandleLockOnInput()
    {
        if(lockOnInput && lockOnFlag == false)
        {
            lockOnInput = false;
            cameraManager.HandleLockOn();
            cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
            if(cameraManager.nearestLockOnTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
                lockOnFlag = true;
            }            
        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraManager.ClearLockOnTargets();

        }

        if(lockOnFlag && lockOnLeftInput)
        {
            lockOnLeftInput = false;
            cameraManager.HandleLockOn();
            if(cameraManager.leftLockTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.leftLockTarget;
            }
        }else if(lockOnFlag && lockOnRightInput)
        {
            lockOnRightInput = false;
            cameraManager.HandleLockOn();
            if (cameraManager.rightLockTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.rightLockTarget;
            }
        }

        cameraManager.SetCameraHeight();

    }

    private void HandleEffectInput()
    {

        if (effectInput)
        {
            effectInput = false;
            if (initEffectFlag || applyEffect.psUpdater == null)
            {
                applyEffect.ApplyOnWeapon(weaponSlotManager.rightHandSlot.currentWeaponModel.transform.GetChild(0).GetChild(0).gameObject);
                initEffectFlag = false;
            }
            else
            {
                applyEffect.ToggleEffect();
            }
            //Debug.Log("Found: " + weaponSlotManager.rightHandSlot.currentWeaponModel.transform.GetChild(0).GetChild(0).name);
            
            
     
        }
    }

    private void HandleTwoHandInput()
    {
        if (y_Input  && weaponSlotManager.rightHandSlot.currentWeaponModel != null)
        {
            twoHandFlag = !twoHandFlag;
            y_Input = false;

            if (twoHandFlag)
            {
                //Enable Two Handing
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            }
            else
            {
                //Disable Two Handing
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);

            }

        }
    }

    private void HandleBuildModeInput()
    {

        if (buildMode_Input)
        {
            buildMode_Input = false;

            buildModeFlag = !buildModeFlag;

            if (buildModeFlag)
            {
                buildInput_Custom.enabled = true;
            }
            else
            {
                buildInput_Custom.enabled = false;
            }


        }

    }
}
