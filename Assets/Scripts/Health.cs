using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        animator.SetTrigger("Hit");

        if(currentHealth <= 0)
        {
            Die();
            animator.SetBool("Dead", true);
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        animator.SetBool("Dead", true);
    }
}
