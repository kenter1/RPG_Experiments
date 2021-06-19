using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotsUI : MonoBehaviour
{
    public Image[] quickslotIcons;

    public void LoadQuickSlotIcons(Item[] quickslotItems)
    {
        for(int i = 0; i < quickslotItems.Length; i++)
        {
            if(quickslotItems[i] != null)
            {
                if (quickslotItems[i].itemIcon != null)
                {
                    quickslotIcons[i].sprite = quickslotItems[i].itemIcon;
                    quickslotIcons[i].enabled = true;
                }
                else
                {
                    quickslotIcons[i].sprite = null;
                    quickslotIcons[i].enabled = false;
                }
            }
        }

        //Cleanup
        for (int i = 0; i < quickslotIcons.Length; i++)
        {
            if(quickslotIcons[i] != null && quickslotItems[i] == null)
            {
                quickslotIcons[i].sprite = null;
                quickslotIcons[i].enabled = false;
            }
        }

    }
    /*
    public void UpdateWeaponQuickSlotsUI(WeaponItem weapon, int quickslotIndex)
    {
        switch (quickslotIndex)
        {
            case 1:
                if(weapon.itemIcon != null)
                {
                    quickslotIcon1.sprite = weapon.itemIcon;
                    quickslotIcon1.enabled = true;
                }
                else
                {
                    quickslotIcon1.sprite = null;
                    quickslotIcon1.enabled = false;
                }

                break;
            case 2:
                if (weapon.itemIcon != null)
                {
                    quickslotIcon2.sprite = weapon.itemIcon;
                    quickslotIcon2.enabled = true;
                }
                else
                {
                    quickslotIcon2.sprite = null;
                    quickslotIcon2.enabled = false;
                }

                break;
            case 3:
                if (weapon.itemIcon != null)
                {
                    quickslotIcon3.sprite = weapon.itemIcon;
                    quickslotIcon3.enabled = true;
                }
                else
                {
                    quickslotIcon3.sprite = null;
                    quickslotIcon3.enabled = false;
                }

                break;
            case 4:
                if (weapon.itemIcon != null)
                {
                    quickslotIcon4.sprite = weapon.itemIcon;
                    quickslotIcon4.enabled = true;
                }
                else
                {
                    quickslotIcon4.sprite = null;
                    quickslotIcon4.enabled = false;
                }

                break;
            case 5:
                if (weapon.itemIcon != null)
                {
                    quickslotIcon5.sprite = weapon.itemIcon;
                    quickslotIcon5.enabled = true;
                }
                else
                {
                    quickslotIcon5.sprite = null;
                    quickslotIcon5.enabled = false;
                }

                break;
        }
    }
    */

}
