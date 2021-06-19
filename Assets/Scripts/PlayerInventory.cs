using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerInventory : MonoBehaviour
{
    private WeaponSlotManager weaponSlotManager;
    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[9];
    public WeaponItem[] weaponInLeftHandSlots = new WeaponItem[1];

    public int currentRightWeaponIndex = -1;
    public int currentLeftWeaponIndex = -1;

    public List<Item> itemInventory;
    public int[] itemInventoryMask = new int[36];
    public Item[] quickSlotItems;

    QuickSlotsUI quickSlotsUI;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
        //quickSlotsUI.LoadQuickSlotIcons(weaponsInRightHandSlots);
        for(int i = 0; i < itemInventoryMask.Length; i++)
        {
            itemInventoryMask[i] = -1;
        }
    }

    private void Start()
    {
        rightWeapon = unarmedWeapon;
        leftWeapon = unarmedWeapon;

    }


    public void UpdateQuickSlots(Item[] items)
    {
        quickSlotsUI.LoadQuickSlotIcons(items);
        quickSlotItems = items;
    }

    public int MinAvailableIndex()
    {
        for (int i = 0; i < itemInventoryMask.Length; i++)
        {
            if (Array.FindIndex(itemInventoryMask, row => row == i) == -1)
            {
                return i;
            }
        }

        return -1;
    }

    public void ChangeWeapon(int slot)
    {
        print("Quickslot: " + slot);
        if(slot == currentRightWeaponIndex)
        {
            //Unwield weapon
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);

            
        }else if((WeaponItem)quickSlotItems[slot] != null && (WeaponItem)quickSlotItems[slot] != unarmedWeapon)
        {
            //Equip weapon
            currentRightWeaponIndex = slot;
            rightWeapon = (WeaponItem)quickSlotItems[currentRightWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot((WeaponItem)quickSlotItems[currentRightWeaponIndex], false);
        }

    }

    /*
    public void ChangeRightWeapon()
    {
        currentRightWeaponIndex = currentRightWeaponIndex + 1;

        if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null)
        {
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
        }
        else if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null)
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;
        }

        if(currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
        {
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
        }
        else
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;
        }

        if(currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
        {
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        }
    }
    */
}
