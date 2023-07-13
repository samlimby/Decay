using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierEnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    private Health health;
    private Animator animator;

    private float lastAttackTime;

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
        // Check if enough time has passed since the last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Check if player is within the enemy's attack range
        Collider2D hitPlayer = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, playerLayer);

        if (hitPlayer != null)
        {
            // Get the Health component of the player and deal damage
            Health playerHealth = hitPlayer.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastAttackTime = Time.time;  // Update the time of the last attack
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
