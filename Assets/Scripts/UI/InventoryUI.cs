using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This code takes the player's input and displays the inventory window.

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    InventorySlot[] slots;
    public GameObject inventoryUI;  // Parent of slots.
    public GameObject inventorySlotPrefab;



    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallBack += UpdateUI;

        for(int i=0; i<inventory.space; i++)
        {
            Instantiate(inventorySlotPrefab, inventoryUI.transform);
        }

        slots = inventoryUI.transform.GetComponentsInChildren<InventorySlot>();

        // Inventory UI Initialization
        UpdateUI();

        // Off Inventory
        inventoryUI.SetActive(false);
    }



    void Update()
    {
        // Inventory UI Torggle
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }



    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
