using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour
{
    private Image icon;
    public Item item;
    public Sprite defaultSprite;

    private void Awake()
    {
        icon = GetComponentInChildren<Image>();
    }

    public void AddItem(Item newItem)
    {

        item = newItem;
        icon.sprite = item.itemIcon;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void Empty()
    {
        item = null;
        icon.sprite = defaultSprite;
        icon.enabled = true;
        gameObject.SetActive(true);

    }

    public void ClearInventorySlot()
    {

        item = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }
}
