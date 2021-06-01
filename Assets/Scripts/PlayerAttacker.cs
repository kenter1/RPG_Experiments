using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorManager animatorHandler;
    PlayerManager playerManager;
    PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    public void HandleLightAttack(WeaponItem weapon, bool running = false)
    {
        if (!playerManager.isInteracting && playerLocomotion.isGrounded)
        {


            if (running)
            {
                print("Playing Run Light Attack Animation: " + weapon.OH_Run_Attack_01);
                animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_01, true);

            }
            else
            {
                print("Playing Light Attack Animation: " + weapon.OH_Light_Attack_1);
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
            }
        }

    }

    public void HandleHeavyAttack(WeaponItem weapon, bool running = false)
    {
        if (!playerManager.isInteracting && playerLocomotion.isGrounded)
        { 
            if (running)
            {
                print("Playing Run Light Attack Animation: " + weapon.OH_Run_Attack_02);
                animatorHandler.PlayTargetAnimation(weapon.OH_Run_Attack_02, true);

            }
            else
            {
                print("Playing Heavy Attack Animation: " + weapon.OH_Heavy_Attack_1);
                animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            }

        }

    }
}
