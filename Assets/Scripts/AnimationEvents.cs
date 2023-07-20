using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    // You'll need a reference to the PlayerCombat to change the isAttacking variable.
    public PlayerCombat playerCombat;
    public EnemyAI enemyAI;

    public void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        enemyAI = GetComponent<EnemyAI>();
    }
    
    public void HandleDeath()
    {
        Destroy(gameObject);
    }
    
    public void StartAttack()
    {
        playerCombat.isAttacking = true;
        enemyAI.isAttacking = true;
    }
    
    public void EndAttack()
    {
        playerCombat.isAttacking = false;
        enemyAI.isAttacking = false;
    }
}
