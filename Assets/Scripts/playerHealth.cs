using UnityEngine;
using UnityEngine.SceneManagement; // to reload the scene when player dies

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f; // The maximum health of the player.
    private float currentHealth; // The current health of the player.
    
    void Start()
    {
        currentHealth = maxHealth; // Set current health to max at the start.
    }

    public void TakeDamage(float amount)
    {
        // Reduce the current health by the damage amount.
        currentHealth -= amount;
        
        // Check if the player has died.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Reload the current scene, effectively resetting the level.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
