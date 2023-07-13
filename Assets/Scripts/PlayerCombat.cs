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

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers; 

    public List<Attack> attacks;

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
        //Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        //Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(attack.attackDamage);
        }
        attack.nextAttackTime = Time.time + attack.cooldown;
    }

    void OnDrawGizmosSelected() 
    {
        if(attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
