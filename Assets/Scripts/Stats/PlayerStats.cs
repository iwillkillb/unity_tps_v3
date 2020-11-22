using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    // Start is called before the first frame update
    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }

    // Update is called once per frame
    void OnEquipmentChanged(int slotIndex, Equipment item, bool isGetting)
    {
        if (isGetting)
        {
            // Add Item
            defense.AddModifier(item.defenseModifier);
            attack.AddModifier(item.attackModifier);
            Debug.Log("Plus  " + item.name + " -> defense : " + item.defenseModifier + " / attack : " + item.attackModifier);
        }
        else
        {
            // Remove Item
            defense.RemoveModifier(item.defenseModifier);
            attack.RemoveModifier(item.attackModifier);
            Debug.Log("Minus " + item.name + " -> defense : " + item.defenseModifier + " / attack : " + item.attackModifier);
        }
    }
    
    // Player kill itself
    // PlayerStats.Die() -> ragdoll.ToggleRagdoll(true)
    // -> PlayerManager.Die() -> player.SetActive(false)

    // Player is revived by Game Manager
    // PlayerManager.Revive() -> player.SetActive(true)
    // -> Player's CharacterStats.Revive() -> ragdoll.ToggleRagdoll(false)

    public override void Die()
    {
        base.Die();

        PlayerManager.instance.Die();

        //PlayerManager.instance.KillPlayer();
    }
}
