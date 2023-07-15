using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttack
{
    public string animationTrigger;
    public string hitAnimationTrigger;
    public float cooldown;
    public int attackDamage;
    [HideInInspector]
    public float nextAttackTime = 0f;
}

public class SoldierEnemyBehaviour : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public BoxCollider2D boxCollider;
    public LayerMask playerLayer;
    public float attackRange = 0.5f;

    private Health health;
    public List<EnemyAttack> attacks;

    private EnemyAttack currentAttack; // keep a reference to the current attack being executed
    private Collider2D currentPlayer; // keep a reference to the player being hit

    private PlayerCombat player; // Add this line

    private void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        health.OnDie.AddListener(HandleDeath);
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerCombat>();
        }
    }

    private void Update()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

         // If player is in range, find an attack to execute
        if (hitPlayer != null)
        {
            foreach (var attack in attacks)
            {
                if (Time.time >= attack.nextAttackTime)
                {
                    ExecuteAttack(attack, hitPlayer);
                    break;
                }
            }
        }

        // If player is attacking and enemy is in player's attack range, play hit animation
        if(hitPlayer != null && (player != null && player.isAttacking))
        {
            foreach (var attack in attacks)
            {
                animator.SetTrigger(attack.hitAnimationTrigger);
            }
        }
    }

    public void ExecuteAttack(EnemyAttack attack, Collider2D hitPlayer)
    {
        animator.SetTrigger(attack.animationTrigger);
        currentAttack = attack;
        currentPlayer = hitPlayer;
        attack.nextAttackTime = Time.time + attack.cooldown;
    }

    public void DealDamage()
    {
        Debug.Log("DealDamage is called");

        // Get the Health component of the player and deal damage
        Health playerHealth = currentPlayer.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(currentAttack.attackDamage);
        }
    }

    private void HandleDeath()
    {
        // Set the Dead animation parameter to true
        animator.SetBool("Dead", true);
    }

    void OnDrawGizmosSelected() 
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
