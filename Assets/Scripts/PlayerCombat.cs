using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string animationTrigger;
    public float cooldown;
    public int attackDamage;
    [HideInInspector]
    public float nextAttackTime = 0f;
}

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public bool isAttacking;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers; 

    public List<Attack> attacks;

    private Attack currentAttack; // keep a reference to the current attack being executed
    private Collider2D[] currentHitEnemies; // keep a reference to the enemies being hit

    void Update()
    {
        if (Time.time >= attacks[0].nextAttackTime && Input.GetMouseButtonDown(0))
        {
            ExecuteAttack(attacks[0]);
        }

        if (attacks.Count > 1 && Time.time >= attacks[1].nextAttackTime && Input.GetMouseButtonDown(1))
        {
            ExecuteAttack(attacks[1]);
        }
        // Add more conditions for more attacks
    }

    void ExecuteAttack(Attack attack)
    {
        animator.SetTrigger(attack.animationTrigger);
        isAttacking = true;

        //Detect enemies in range of attack
        currentHitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Set the current attack being executed
        currentAttack = attack;

        attack.nextAttackTime = Time.time + attack.cooldown;
    }

    // Call this function from the animation event
    public void DealDamage()
    {
        //Damage them
        foreach(Collider2D enemy in currentHitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(currentAttack.attackDamage);
        }
    }

    void OnDrawGizmosSelected() 
    {
        if(attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void StartAttack()
    {
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false; // Player finishes attacking
    }
}
