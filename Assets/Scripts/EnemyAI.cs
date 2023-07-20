using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[System.Serializable]
public class EnemyAttack
{
    public string animationTrigger;
    public string hitAnimationTrigger;
    public float cooldown;
    public int attackDamage;
    [HideInInspector]
    public float nextAttackTime = 0f;
    public bool isAttacking = false;
}

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;


    [Header("Attack")]
    public LayerMask playerAttackLayer; // tag this to the collider associated to the attack animation cycle from the player
    public float attackRange = 0.5f;
    private EnemyAttack currentAttack;
    private Collider2D currentPlayer;
    private PlayerCombat player;
    public bool isAttacking;
    public List<EnemyAttack> attacks;

    private Path path;
    private int currentWaypoint = 0;
    RaycastHit2D isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    Health health;
    private bool reachedEndOfPath = false;
    private Collider2D[] currentHitPlayer;
    public LayerMask playerLayer;
    public Animator animator;
    public Transform attackPoint;
    private Health enemyHealth;

    public void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<Health>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerCombat>();
        }
    }

    public void Start()
    {
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        // Check if the player is in reach through a Collider2D while generating a circle radius
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        
        // If player is in range, find an attack to execute
        if (hitPlayer != null)
        {
            // Check if the player is in attack animation
            PlayerCombat playerCombat = hitPlayer.GetComponent<PlayerCombat>();
            if (playerCombat != null && playerCombat.isAttacking)
            {
                foreach (var attack in attacks)
                {
                    // Only trigger hit animation if the enemy is hit by the player's attack
                    if (Time.time >= attack.nextAttackTime)
                    {
                        animator.SetTrigger(attack.hitAnimationTrigger);
                        break;
                    }
                }
            }
            else
            {
                if(!isAttacking) // Start an attack if not currently attacking
                {
                    // If player is not in attack animation, enemy executes its own attack
                    foreach (var attack in attacks)
                    {
                        if (Time.time >= attack.nextAttackTime)
                        {
                            ExecuteAttack(attack, hitPlayer);
                            break;
                        }
                    }

                }
            }
        }
        else
        {
            if(isAttacking) // End an attack if no player is in range
            {
                EndAttack();
            }
        }    
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }else
        {
            reachedEndOfPath = false;
        }

        // See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        // Debug draw the ground check raycast
        Debug.DrawLine(startOffset, startOffset - Vector3.up * 0.05f, Color.red);
        
        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        // Apply constant force for smooth movement
        float forceMagnitude = force.magnitude;
        Vector2 normalizedForce = force.normalized;
        rb.velocity = normalizedForce * speed;

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

         // Debug message
         Debug.Log("Enemy is moving!");
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void StartAttack()
    {
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
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

        foreach(Collider2D player in currentHitPlayer)
        {
            currentPlayer.GetComponent<Health>().TakeDamage(currentAttack.attackDamage);
        }
    }

    IEnumerator DestroyAfterAnimation(GameObject objectToDestroy, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(objectToDestroy);
    }

    public void HandleDeath()
    {
        animator.SetBool("Dead", true);
        StartCoroutine(DestroyAfterAnimation(gameObject, animator));
    }


    void OnDrawGizmosSelected() 
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}