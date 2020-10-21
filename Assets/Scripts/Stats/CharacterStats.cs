using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Ragdoll ragdoll;

    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth { get; private set; }
    public event System.Action<float, float> OnHealthChanged;

    [Header("Stat")]
    public Stat damage;
    public Stat armor;

    [Header("Attack")]
    public float attackSpeed = 1f;
    public float attackDelay = 0.5f;
    private float attackCooldown = 0f;
    public event System.Action OnAttack;



    private void OnEnable()
    {
        // HP Initialization
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + "damage.");

        OnHealthChanged(maxHealth, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log(transform.name + " died.");

        // On Ragdoll
        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(true);
        }
    }

    public virtual void Revive()
    {
        Debug.Log(transform.name + " revived.");

        // On Ragdoll
        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(false);
        }
    }

    // ==================================================================================

    public void Attack(CharacterStats targetStats)
    {
        // Delay check and reset
        if (attackCooldown > 0f)
        {
            return;
        }
        attackCooldown = 1f / attackSpeed;

        StartCoroutine(DoDamage(targetStats, attackDelay));

        // Event
        OnAttack();

    }
    IEnumerator DoDamage(CharacterStats stats, float delay)
    {
        yield return new WaitForSeconds(delay);

        stats.TakeDamage(damage.GetValue());
    }
}
