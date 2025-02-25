using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIResponsiveTarget : MonoBehaviour
{
    public enum AIState { Idle, Patrol, Chase, Attack, Flee }
    public AIState currentState = AIState.Idle;

    public Transform Target;
    public float AttackDistance = 2.0f; // Distance at which AI attacks
    public float ChaseDistance = 10.0f; // Distance at which AI starts chasing
    public float FleeDistance = 5.0f; // Distance at which AI flees
    public Transform[] patrolPoints; // Patrol points for AI
    private int currentPatrolIndex = 0;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private bool isAttacking = false;
    private bool isFleeing = false;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        if (patrolPoints.Length > 0)
        {
            currentState = AIState.Patrol;
            MoveToNextPatrolPoint();
        }
    }

    void Update()
{
    if (Target == null) return;

    float distanceToTarget = Vector3.Distance(Target.position, transform.position);

    switch (currentState)
    {
        case AIState.Patrol:
            PatrolBehavior();
            if (distanceToTarget <= ChaseDistance)
            {
                currentState = AIState.Chase;
            }
            break;

        case AIState.Chase:
            ChaseBehavior(distanceToTarget);
            break;

        case AIState.Attack:
            AttackBehavior(distanceToTarget);
            break;

        case AIState.Flee:
            FleeBehavior(distanceToTarget);
            break;
    }

    // ✅ Update the Speed parameter for animation
    float agentSpeed = m_Agent.velocity.magnitude; // Get NavMeshAgent movement speed
    m_Animator.SetFloat("Speed", agentSpeed); // Set Speed in Animator
}

    void PatrolBehavior()
    {
        if (m_Agent.remainingDistance < 1f && patrolPoints.Length > 0)
        {
            MoveToNextPatrolPoint();
        }
    }

    void ChaseBehavior(float distanceToTarget)
    {
        if (distanceToTarget > ChaseDistance)
        {
            Debug.Log("🔄 AI Lost Target, Resuming Patrol.");
            currentState = AIState.Patrol;
            MoveToNextPatrolPoint();
            return;
        }

        if (distanceToTarget <= AttackDistance)
        {
            currentState = AIState.Attack;
            return;
        }

        m_Agent.isStopped = false;
        m_Agent.destination = Target.position;
        m_Animator.SetBool("Attack", false);
    }

    void AttackBehavior(float distanceToTarget)
    {
        if (distanceToTarget > AttackDistance)
        {
            Debug.Log("🏃 AI Target Moved, Switching Back to Chase Mode.");
            currentState = AIState.Chase;
            isAttacking = false;
            return;
        }

        if (!isAttacking)
        {
            Debug.Log("⚔ AI Attacking!");
            m_Agent.isStopped = true;
            m_Agent.velocity = Vector3.zero;

            if (HasAnimatorParameter(m_Animator, "Attack"))
            {
                m_Animator.SetBool("Attack", true);
                isAttacking = true;
            }
        }
    }

    void FleeBehavior(float distanceToTarget)
    {
        if (distanceToTarget > FleeDistance)
        {
            Debug.Log("🔄 AI Stopped Fleeing.");
            currentState = AIState.Patrol;
            MoveToNextPatrolPoint();
            isFleeing = false;
            return;
        }

        Vector3 fleeDirection = transform.position - Target.position;
        Vector3 fleePoint = transform.position + fleeDirection.normalized * 5f;

        m_Agent.isStopped = false;
        m_Agent.destination = fleePoint;
        m_Animator.SetBool("Attack", false);
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        m_Agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DangerZone")) // AI flees if it enters a Danger Zone
        {
            Debug.Log("⚠ AI Entered Danger Zone!");
            currentState = AIState.Flee;
            isFleeing = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DangerZone"))
        {
            Debug.Log("✅ AI Exited Danger Zone.");
            currentState = AIState.Patrol;
            MoveToNextPatrolPoint();
        }
    }

    private bool HasAnimatorParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}

