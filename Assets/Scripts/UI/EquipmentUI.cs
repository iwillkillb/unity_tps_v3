using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    EquipmentManager equipmentManager;
    InventorySlot[] slots;
    public GameObject equipmentUI;  // Parent of slots.

    // Start is called before the first frame update
    void Start()
    {
        equipmentManager = EquipmentManager.instance;
        equipmentManager.onEquipmentChanged += UpdateUI;

        // Size of slots == Count of EquipmentPart.
        slots = equipmentUI.transform.GetComponentsInChildren<InventorySlot>();

        // Off Equipment UI
        equipmentUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Equipment UI Torggle
        if (Input.GetButtonDown("Equipment"))
        {
            // UI is Initialized before activating.
            if (equipmentUI.activeSelf == false)
            {
                UpdateUI(0, null, true);
            }

            // UI Activate
            equipmentUI.SetActive(!equipmentUI.activeSelf);
        }
    }

    void UpdateUI(int slotIndex, Item item, bool isGetting)
    {
        if (isGetting)
        {
            // Add item
            // If item is null, then update ui only.
            if (item != null)
                slots[slotIndex].AddItem(item);
        }
        else
        {
            // Remove item
            slots[slotIndex].ClearSlot();
        }

        // Refresh UI
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetItem() == null)
                slots[i].ClearSlot();
        }
    }

    public void Remove()
    {
        //equipmentManager.Unequip(0);
        // Button click -> Unequip.
    }
}
