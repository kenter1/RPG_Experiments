using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    PlayerAnimatorManager animatorHandler;
    PlayerManager playerManager;
    PlayerLocomotion playerLocomotion;
    InputManager inputHandler;
    public WeaponSlotManager weaponSlotManager;
    public string lastAttack;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<PlayerAnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputHandler = GetComponent<InputManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleWeaponCombo(WeaponItem weapon, bool heavyAttack = false)
    {
        if (inputHandler.comboFlag)
        {
            #region One Handed Sword Combos
            if (lastAttack == weapon.OH_Light_Attack_1)
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_2, true, true);
                lastAttack = weapon.OH_Light_Attack_2;
                animatorHandler.animator.SetBool("canDoCombo", true);
            }
            else if (lastAttack == weapon.OH_Light_Attack_2)
            {
                if (heavyAttack == false)
                {
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    lastAttack = weapon.OH_Run_Attack_01;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_01, true, true);
                }
                else
                {
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    lastAttack = weapon.OH_Run_Attack_02;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_02, true, true);
                }
            }
            else if (lastAttack == weapon.OH_Heavy_Attack_1)
            {
                if (heavyAttack == false)
                {
                    lastAttack = weapon.OH_Run_Attack_01;
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_01, true, true);
                }
                else
                {
                    lastAttack = weapon.OH_Heavy_Attack_2;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_2, true, true);
                    animatorHandler.animator.SetBool("canDoCombo", false);
                }

            }
            #endregion

            #region Two Handed Sword Combos
            if (lastAttack == weapon.TH_Light_Attack_1)
            {
                animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_2, true, true);
                lastAttack = weapon.TH_Light_Attack_2;
                animatorHandler.animator.SetBool("canDoCombo", true);
            }
            else if (lastAttack == weapon.TH_Light_Attack_2)
            {
                if (heavyAttack == false)
                {
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    lastAttack = weapon.TH_Run_Attack_01;
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_3, true, true);
                }
                else
                {
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    lastAttack = weapon.TH_Run_Attack_02;
                    animatorHandler.PlayTargetAnimation(weapon.TH_Run_Attack_02, true, true);
                }
            }
            else if (lastAttack == weapon.TH_Heavy_Attack_1)
            {
                if (heavyAttack == false)
                {
                    lastAttack = weapon.TH_Run_Attack_01;
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Run_Attack_01, true, true);
                }
                else
                {
                    lastAttack = weapon.TH_Run_Attack_02;
                    animatorHandler.animator.SetBool("canDoCombo", true);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Run_Attack_02, true, true);
                }

            } else if (lastAttack == weapon.TH_Run_Attack_02)
            {
                if(heavyAttack == false)
                {
                    lastAttack = weapon.TH_Run_Attack_01;
                    animatorHandler.animator.SetBool("canDoCombo", false);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Run_Attack_01, true, true);
                }
            }

            #endregion

        }

    }

    public void HandleLightAttack(WeaponItem weapon, bool running = false)
    {

        if (!playerManager.isInteracting && playerLocomotion.isGrounded)
        {
            weaponSlotManager.attackingWeapon = weapon;
            if (running)
            {
                if (inputHandler.twoHandFlag)
                {
                    print("Playing Run Two Handed Light Attack Animation: " + weapon.TH_Run_Attack_01);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Run_Attack_01, true, true);
                    lastAttack = weapon.TH_Run_Attack_01;
                }
                else
                {
                    print("Playing Run Light Attack Animation: " + weapon.OH_Run_Attack_01);
                    animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_01, true, true);
                    lastAttack = weapon.OH_Run_Attack_01;
                }
            }
            else
            {
                if (inputHandler.twoHandFlag)
                {
                    print("Playing Two Handed Light Attack Animation: " + weapon.TH_Light_Attack_1);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Attack_1, true, true);
                    lastAttack = weapon.TH_Light_Attack_1;
                }
                else
                {
                    print("Playing Light Attack Animation: " + weapon.OH_Light_Attack_1);
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true, true);
                    lastAttack = weapon.OH_Light_Attack_1;
                }


            }
        }

    }

    public void HandleHeavyAttack(WeaponItem weapon, bool running = false)
    {
        if (!playerManager.isInteracting && playerLocomotion.isGrounded)
        {
            weaponSlotManager.attackingWeapon = weapon;
            if (running)
            {
                if (inputHandler.twoHandFlag)
                {
                    print("Playing Run Two Handed Light Attack Animation: " + weapon.TH_Run_Attack_02);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Run_Attack_02, true, true);
                    lastAttack = weapon.TH_Run_Attack_02;
                }
                else
                {
                    print("Playing Run Light Attack Animation: " + weapon.OH_Run_Attack_02);
                    animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_02, true, true);
                    lastAttack = weapon.OH_Run_Attack_02;
                }


            }
            else
            {
                if (inputHandler.twoHandFlag)
                {
                    print("Playing Two Handed Heavy Attack Animation: " + weapon.TH_Heavy_Attack_1);
                    animatorHandler.PlayTargetAnimation(weapon.TH_Heavy_Attack_1, true, true);
                    lastAttack = weapon.TH_Heavy_Attack_1;
                }
                else
                {
                    print("Playing Heavy Attack Animation: " + weapon.OH_Heavy_Attack_1);
                    animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true, true);
                    lastAttack = weapon.OH_Heavy_Attack_1;
                }

            }

        }

    }
}
