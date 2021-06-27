using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    private PlayerInventory playerInventory;

    [Header("UI Windows")]
    public GameObject hudWindow;
    public GameObject selectWindow;
    public GameObject inventoryWindow;

    public GameObject inventorySlotPrefab;
    public Transform inventorySlotsParent;
    private InventorySlot[] inventorySlots;


    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        inventorySlots = inventorySlotsParent.GetComponentsInChildren<InventorySlot>();
        inventoryWindow.SetActive(false);
    }




    public void SwapItemPosition(int itemPosition1, int itemPosition2)
    {
        int itemIndex1 = Array.FindIndex(playerInventory.itemInventoryMask, row => row == itemPosition1);
        int itemIndex2 = Array.FindIndex(playerInventory.itemInventoryMask, row => row == itemPosition2);
        
       

        //Case 1: Two Objects Swap
        if(itemIndex1 != -1 && itemIndex2 != -1)
        {
            int temp = playerInventory.itemInventoryMask[itemIndex2];
            playerInventory.itemInventoryMask[itemIndex2] = playerInventory.itemInventoryMask[itemIndex1];
            playerInventory.itemInventoryMask[itemIndex1] = temp;

        }
        else if(itemIndex1 != -1 && itemIndex2 == -1)
        {
            //playerInventory.itemInventoryMask[itemPosition2] = -1;
            playerInventory.itemInventoryMask[itemIndex1] = itemPosition2;
        }


        Debug.Log("Idx1: " + itemIndex1 + ", Idx2" + itemIndex2 + " Size: " + playerInventory.itemInventoryMask.Length);
        Debug.Log("Pos1: " + itemPosition1 + ", Pos2: " + itemPosition2 + " Size: " + playerInventory.itemInventoryMask.Length);

        UpdateUI();

        #region Weapon Sheathe
        //Swapped 
        if ( (itemPosition1 == 27 && itemPosition2 == 28) || (itemPosition1 == 28 && itemPosition2 == 27))
        {
            playerInventory.WeildUnarmed();

            playerInventory.DestroyBackWeapon();
            playerInventory.DestroySideWeapon();

            if(playerInventory.backWeapon != playerInventory.rightWeapon)
            {
                playerInventory.StoreBackWeapon();
            }

            if (playerInventory.sheatheWeapon != playerInventory.rightWeapon)
            {
                playerInventory.StoreSideWeapon();
            }
        }
        else
        {
            //item is removed.
            if (itemPosition1 == 27)
            {
                playerInventory.DestroyBackWeapon();
            }
            else if (itemPosition1 == 28)
            {
                playerInventory.DestroySideWeapon();
            }

            //Item is swapped to nothing
            if (itemPosition2 == 27)
            {
                playerInventory.StoreBackWeapon();
            }
            else if (itemPosition2 == 28)
            {
                playerInventory.StoreSideWeapon();
            }

            //Item
        }
        #endregion


    }

    public void UpdateUI()
    {
        Item[] quickslotItems = new Item[9];

        #region Debug Messages
        /*
        string str = "";

        foreach (var item in playerInventory.itemInventoryMask)
        {
            str += item + ",";
        }
        Debug.Log(str);

        str = "QuickSlots: ";
        for(int i = 0; i < playerInventory.itemInventoryMask.Length; i++)
        {
            if(playerInventory.itemInventoryMask[i] > 26)
            {
                str += playerInventory.itemInventory[i] + ":" + playerInventory.itemInventoryMask[i] % 9 + " ";
                quickslotItems[playerInventory.itemInventoryMask[i] % 9] = playerInventory.itemInventory[i];
            }
        }

        Debug.Log(str);
        */
        #endregion

        for (int i = 0; i < playerInventory.itemInventoryMask.Length; i++)
        {

            if (playerInventory.IsSlotAvailable(i))
            {
                inventorySlots[i].Empty();
            }
            
        }


        for (int i = 0; i < playerInventory.itemInventoryMask.Length; i++)
        {
            if (playerInventory.itemInventoryMask[i] != -1)
            {
                inventorySlots[playerInventory.itemInventoryMask[i]].AddItem(playerInventory.itemInventory[i]);
            } 
        }

        for (int i = 0; i < playerInventory.itemInventoryMask.Length; i++)
        {
            if (playerInventory.itemInventoryMask[i] > 26)
            {
                quickslotItems[playerInventory.itemInventoryMask[i] % 9] = playerInventory.itemInventory[i];
            }
        }
        playerInventory.UpdateQuickSlots(quickslotItems);


        /*
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(i < playerInventory.itemInventory.Count)
            {
                if(inventorySlots.Length < playerInventory.itemInventory.Count)
                {
                    Instantiate(inventorySlotPrefab, inventorySlotsParent);
                    inventorySlots = inventorySlotsParent.GetComponentsInChildren<InventorySlot>();
                }
                inventorySlots[i].AddItem(playerInventory.itemInventory[i]);
            }
            else
            {
                inventorySlots[i].ClearInventorySlot();
            }
        }
        */
    }

    private void validateUI()
    {
        for(int i = 0; i < playerInventory.itemInventory.Count; i++)
        {
            //Something Wrong happened Temporary Fix find an empty spot
            if(playerInventory.itemInventory[i] != null && playerInventory.itemInventoryMask[i] == -1)
            {
                playerInventory.itemInventoryMask[i] = playerInventory.MinAvailableIndex();
            }
        }
    }

    public void OpenSelectWindow()
    {
        selectWindow.SetActive(true);
    }
    public void CloseSelectWindow()
    {
        selectWindow.SetActive(false);
    }

    public void OpenInventoryWindow()
    {
        inventoryWindow.SetActive(true);
        validateUI();
    }

    public void CloseInventoryWindow()
    {
        inventoryWindow.SetActive(false);
    }
}
