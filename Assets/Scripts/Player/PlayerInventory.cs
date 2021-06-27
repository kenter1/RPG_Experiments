using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerInventory : MonoBehaviour
{
    private AnimatorManager animatorManager;
    public WeaponSlotManager weaponSlotManager;
   
    private ApplyEffect applyEffect;
    private UIManager uIManager;
    private GameObject backslotHolder;
    private GameObject sheatheHolder;

    public GameObject backWeapon;
    public GameObject sheatheWeapon;

    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[9];
    public WeaponItem[] weaponInLeftHandSlots = new WeaponItem[1];

    public int currentRightWeaponIndex = -1;
    public int currentLeftWeaponIndex = -1;

    public List<Item> itemInventory;
    private int itemCount = 0;
    public int[] itemInventoryMask = new int[36];
    public Item[] quickSlotItems;

    QuickSlotsUI quickSlotsUI;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        backslotHolder = GameObject.Find("Item_SwordHolder");
        sheatheHolder = GameObject.Find("Item_SwordSheath");
        uIManager = FindObjectOfType<UIManager>();
        applyEffect = GetComponent<ApplyEffect>();
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

    private void Update()
    {
        if(itemCount != itemInventory.Count)
        {
            uIManager.UpdateUI();
            itemCount = itemInventory.Count;
        }
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

    public bool IsSlotAvailable(int slot)
    {
        if(Array.FindIndex(itemInventoryMask, row => row == slot) == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StoreBackWeapon()
    {   
        if(backWeapon != null)
        {
            DestroyBackWeapon();
        }

        if(quickSlotItems[0] != null)
        {
            backWeapon = Instantiate(((WeaponItem)quickSlotItems[0]).modelPrefab);
            backWeapon.transform.parent = backslotHolder.transform;
            backWeapon.transform.localPosition = Vector3.zero;
            backWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }

    public void StoreSideWeapon()
    {
        if (sheatheWeapon != null)
        {
            DestroySideWeapon();
        }

        if (quickSlotItems[1] != null)
        {
            sheatheWeapon = Instantiate(((WeaponItem)quickSlotItems[1]).modelPrefab);
            sheatheWeapon.transform.parent = sheatheHolder.transform;
            sheatheWeapon.transform.localPosition = Vector3.zero;
            sheatheWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }

    public void DestroyBackWeapon()
    {
        Destroy(backWeapon);
    }

    public void DestroySideWeapon()
    {
        Destroy(sheatheWeapon);
    }

    public void WeildUnarmed()
    {
        currentRightWeaponIndex = -1;
        rightWeapon = unarmedWeapon;
        weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
    }


    
    public void ChangeWeapon(int slot)
    {
        //Can only weild weapon from first or second quickslot
        if (slot == 0 || slot == 1)
        {
            print("Quickslot: " + slot);
            if (slot == currentRightWeaponIndex)
            {
                //If unweild weapon create stored weapon
                if (slot == 0)
                {
                    StoreBackWeapon();
                }
                else if (slot == 1)
                {
                    StoreSideWeapon();
                }

                //Unwield weapon
                WeildUnarmed();
            }
            else if ((WeaponItem)quickSlotItems[slot] != null && (WeaponItem)quickSlotItems[slot] != unarmedWeapon)
            {
                //If wearing weapon destroy stored weapon
                if (backWeapon != null && slot == 0)
                {
                    DestroyBackWeapon();
                }
                else if (sheatheWeapon != null && slot == 1)
                {
                    DestroySideWeapon();
                }

                //If switching weapon
                if (slot == 0 && sheatheWeapon == null)
                {
                    StoreSideWeapon();
                }
                else if (slot == 1 && backWeapon == null)
                {
                    StoreBackWeapon();
                }

                if(slot == 0)
                {
                    animatorManager.PlayTargetAnimation("Draw_Back_Sword_01", true, true);
                }
                else if(slot == 1)
                {
                    animatorManager.PlayTargetAnimation("Draw_Side_Sword_01", true, true);
                }

                //Equip weapon
                currentRightWeaponIndex = slot;
                rightWeapon = (WeaponItem)quickSlotItems[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot((WeaponItem)quickSlotItems[currentRightWeaponIndex], false);

            }
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
