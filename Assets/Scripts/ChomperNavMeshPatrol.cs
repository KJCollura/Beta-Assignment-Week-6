using UnityEngine;
using UnityEngine.AI;

public class ChomperNavMeshPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float stoppingDistance = 0.2f;
    private int currentPointIndex = 0;

    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Ensure NavMeshAgent works properly for 2D movement
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0)
            return;

        // Check if the agent has reached the patrol point
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            GoToNextPoint();
        }

        // Ensure Chomper moves
        if (agent.velocity.magnitude > 0.1f)
        {
            animator.SetFloat("Speed", 1f); // Play walk animation
        }
        else
        {
            animator.SetFloat("Speed", 0f); // Stay in idle
        }

        // Flip sprite based on direction
        if (agent.velocity.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (agent.velocity.x < -0.1f)
            spriteRenderer.flipX = true;
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }
}
