using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseThreshold;
    private Animator animator;
    private Vector2 direction;
    private enum State { Idle, Patrol, Chase }
    private State state;

    private AstarPath path;
    private Seeker seeker;
    private Path myPath;
    private int currentWaypoint = 0;

    void Start() 
    {
        animator = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        state = State.Idle;
        // Start the enemy as idle
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        switch (state) 
        {
            case State.Idle:
                if (distanceToPlayer < chaseThreshold)
                {
                    state = State.Chase;
                    seeker.StartPath(transform.position, player.position, OnPathComplete);
                }
                break;
            case State.Patrol:
                // Implement patrol logic here
                break;
            case State.Chase:
                if (myPath == null) return;

                if (currentWaypoint >= myPath.vectorPath.Count)
                {
                    state = State.Idle; // Go back to Idle when we reach the player
                }
                else
                {
                    direction = ((Vector2)myPath.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
                    transform.Translate(direction * Time.deltaTime);
                    // Check if we reached the current waypoint
                    if (Vector2.Distance(transform.position, myPath.vectorPath[currentWaypoint]) < 0.1f)
                    {
                        currentWaypoint++;
                    }
                }
                break;
        }
        animator.SetFloat("DirX", direction.x);
        animator.SetFloat("DirY", direction.y);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            myPath = p;
            currentWaypoint = 0;
        }
    }
}
