using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorManager animatorHandler;
    PlayerManager playerManager;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();

    }

    public void HandleLightAttack(WeaponItem weapon, bool running = false)
    {
        if (!playerManager.isInteracting)
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

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (!playerManager.isInteracting)
        {
            print("Playing Heavy Attack Animation: " + weapon.OH_Heavy_Attack_1);
            animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
        }

    }
}
