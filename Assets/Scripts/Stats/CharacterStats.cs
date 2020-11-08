using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Ragdoll ragdoll;

    [Header("HP/MP")]
    public int maxHP = 100;
    public int maxMP = 100;
    public int currentHP { get; private set; }
    public int currentMP { get; private set; }
    public event System.Action<int, int> OnHPChanged;

    [Header("Stat")]
    public Stat attack;
    public Stat defense;

    [Header("Attack")]
    public float attackSpeed = 1f;
    public float attackDelay = 0.5f;
    private float attackCooldown = 0f;
    public event System.Action OnAttack;



    private void OnEnable()
    {
        // Initialization
        currentHP = maxHP;
        currentMP = maxMP;
    }

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        damage -= defense.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHP -= damage;
        Debug.Log(transform.name + " takes " + damage + "damage.");

        OnHPChanged(maxHP, currentHP);

        if (currentHP <= 0)
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

        stats.TakeDamage(attack.GetValue());
    }
}
