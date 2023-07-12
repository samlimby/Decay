using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Animator animator;

    //starting health when the game begins
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            //death occurs
            Die();
            //play death animation
            animator.SetBool("Dead", true);
            //show gameoverscreen
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        //Die animation
        //Disable the enemy
    }

}
