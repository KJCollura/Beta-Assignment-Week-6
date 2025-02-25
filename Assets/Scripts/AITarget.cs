using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance = 2.0f; // The range at which the AI will attack

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    private bool isAttacking = false; // Prevents attack spam

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        m_Agent.isStopped = false; // Ensure agent starts active
    }

    void Update()
    {
        if (Target == null)
        {
            Debug.LogError("❌ ERROR: Target is not assigned to AI.");
            return; // Prevent null reference errors
        }

        // Calculate distance to target
        m_Distance = Vector3.Distance(Target.position, transform.position);
        Debug.Log($"AI Distance to Target: {m_Distance} | AttackDistance: {AttackDistance}");

        if (m_Distance <= AttackDistance)
        {
            // If AI is not already attacking, start attack
            if (!isAttacking)
            {
                Debug.Log("🛑 AI has reached attack distance. Stopping and attacking.");
                m_Agent.isStopped = true; // Stop movement
                m_Agent.velocity = Vector3.zero; // Prevent movement drift

                // ✅ Check if the Animator has the "Attack" parameter before setting it
                if (HasAnimatorParameter(m_Animator, "Attack"))
                {
                    m_Animator.SetBool("Attack", true); // Start attack animation
                    isAttacking = true; // Mark AI as attacking
                }
                else
                {
                    Debug.LogError("❌ ERROR: Animator does not have an 'Attack' parameter! Check Animator Controller.");
                }
            }
        }
        else
        {
            Debug.Log("🏃 AI is moving towards target.");
            m_Agent.isStopped = false; // Resume movement
            m_Animator.SetBool("Attack", false); // Stop attacking when moving
            m_Agent.destination = Target.position;
            isAttacking = false; // Reset attack flag when moving
        }
    }

    void OnAnimatorMove()
    {
        // Only update movement speed when NOT attacking
        if (!m_Animator.GetBool("Attack"))
        {
            m_Agent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
        }
    }

    // ✅ FIX: Proper way to check if Animator has a parameter
    private bool HasAnimatorParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}
