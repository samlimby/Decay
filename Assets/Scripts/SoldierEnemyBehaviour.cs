using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttack
{
    public string animationTrigger;
    public float cooldown;
    public int attackDamage;
    [HideInInspector]
    public float nextAttackTime = 0f;
}

public class SoldierEnemyBehaviour : MonoBehaviour
{
    public Animator animator;

    public BoxCollider2D boxCollider;
    public LayerMask playerLayer;

    private Health health;
    
    public List<EnemyAttack> attacks;

    private void Awake()
    {
        // Get the Health and Animator components
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

        // Register to the OnDie event of the Health script
        health.OnDie.AddListener(HandleDeath);
    }

    private void Update()
    {
        foreach (var attack in attacks)
        {
            if (Time.time >= attack.nextAttackTime)
            {
                ExecuteAttack(attack);
                break;
            }
        }
    }

    public void ExecuteAttack(EnemyAttack attack)
    {
        // Check if player is within the enemy's attack range
        Collider2D hitPlayer = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, playerLayer);

        if (hitPlayer != null)
        {
            // Set the attack animation
            animator.SetTrigger(attack.animationTrigger);

            // Get the Health component of the player and deal damage
            Health playerHealth = hitPlayer.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attack.attackDamage);
                attack.nextAttackTime = Time.time + attack.cooldown;
            }
        }
    }

    private void HandleDeath()
    {
        // Set the Dead animation parameter to true
        animator.SetBool("Dead", true);

        // You can add more logic that should happen on death here
        // For example, you could stop all movement and attacking, etc.
    }
}
