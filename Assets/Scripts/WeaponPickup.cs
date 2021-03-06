using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponPickup : Interactable
{
    public WeaponItem weapon;

    private void Awake()
    {
        
        //
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        Physics.IgnoreCollision(GetComponent<SphereCollider>(), playerManager.playerCollider);
        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {


        PlayerInventory playerInventory;
        PlayerLocomotion playerLocomotion;
        PlayerAnimatorManager animatorManager;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
        animatorManager = playerManager.GetComponentInChildren<PlayerAnimatorManager>();




        playerLocomotion.playerRigidBody.velocity = Vector3.zero; //Stops the player from moving while picking up an item
        animatorManager.PlayTargetAnimation("Pick Up Item", true, true); // Plays the animation of looting the item
        playerInventory.itemInventory.Add(weapon);
        int i;
        for (i = 0; i < playerInventory.itemInventoryMask.Length; i++)
        {
            if(playerInventory.itemInventoryMask[i] == -1)
            {
                if (playerInventory.IsSlotAvailable(27))
                {
                    playerInventory.itemInventoryMask[i] = 27;    
                }
                else if (playerInventory.IsSlotAvailable(28))
                {
                    playerInventory.itemInventoryMask[i] = 28;
                }
                else 
                {
                    playerInventory.itemInventoryMask[i] = playerInventory.MinAvailableIndex();
                }

                break;
            }
        }


        playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = weapon.itemName;
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
        playerManager.itemInteractableGameObject.SetActive(true);

        Destroy(gameObject);
    }

}
