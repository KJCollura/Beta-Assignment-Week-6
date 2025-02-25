using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMove : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        RaycastHit m_HitInfo = new RaycastHit();
        Animator m_Animator;

        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();

            // Ensure Root Motion is OFF (Agent controls movement)
            m_Animator.applyRootMotion = false;

            // Optimize movement speed and responsiveness
            m_Agent.speed = 6.0f;
            m_Agent.acceleration = 30.0f; // Higher acceleration for instant movement
            m_Agent.angularSpeed = 120.0f;
            m_Agent.stoppingDistance = 0.1f;

            // Prevent rotation while idle
            m_Agent.updateRotation = false;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out m_HitInfo))
                {
                    m_Agent.SetDestination(m_HitInfo.point);
                    m_Agent.isStopped = false;  

                    // 🚀 Force instant animation update when clicking
                    m_Animator.SetBool("Running", true);
                }
            }

            // ✅ Use pathStatus to ensure immediate movement detection
            bool isMoving = m_Agent.hasPath && 
                            m_Agent.pathStatus == NavMeshPathStatus.PathComplete &&
                            m_Agent.remainingDistance > m_Agent.stoppingDistance;

            // 🚀 Immediate animation transition
            m_Animator.SetBool("Running", isMoving);

            // 🏃 Rotate character only when moving
            if (isMoving)
            {
                Quaternion targetRotation = Quaternion.LookRotation(m_Agent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
