using UnityEngine;
using UnityEngine.AI;

namespace JadePhoenix.Tools
{
    public class AIActionMoveTowardsTarget : AIAction
    {
        [Tooltip("Toggle for whether the AI should try to avoid obstacles or not.")]
        public bool AvoidObstacles = true;

        [Tooltip("The distance to check for obstacles in front of the AI.")]
        public float ObstacleCheckDistance = 2.0f;

        [Tooltip("The layer mask to identify obstacles.")]
        public LayerMask ObstacleMask;

        [Tooltip("Number of rays casted in a fan shape in front of the AI for obstacle detection.")]
        public int NumberOfRays = 3;

        [Tooltip("Angle (in degrees) between each ray in the fan.")]
        public float AngleBetweenRays = 15.0f;

        private NavMeshAgent _agent;

        protected override void Initialization()
        {
            base.Initialization();
            _agent = this.gameObject.GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                _agent = this.gameObject.AddComponent<NavMeshAgent>();
            }
        }

        public override void PerformAction()
        {
            if (_brain.Target != null)
            {
                if (AvoidObstacles && MultipleRaycastObstacleCheck())
                {
                    AdjustPath();
                }
                else
                {
                    _agent.SetDestination(_brain.Target.position);
                }
            }
        }

        private bool MultipleRaycastObstacleCheck()
        {
            Vector3 directionToTarget = (_brain.Target.position - transform.position).normalized;
            bool obstacleDetected = false;

            for (int i = 0; i < NumberOfRays; i++)
            {
                float rayAngle = (-AngleBetweenRays * (NumberOfRays - 1) / 2.0f) + (AngleBetweenRays * i);
                Vector3 rayDirection = Quaternion.Euler(0, rayAngle, 0) * directionToTarget;

                if (Physics.Raycast(transform.position, rayDirection, ObstacleCheckDistance, ObstacleMask))
                {
                    obstacleDetected = true;
                }
            }

            return obstacleDetected;
        }

        private void AdjustPath()
        {
            Vector3 perpendicularDirection = Vector3.Cross((_brain.Target.position - transform.position).normalized, Vector3.up);
            _agent.SetDestination(transform.position + perpendicularDirection * ObstacleCheckDistance);
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            if (_agent)
            {
                _agent.isStopped = false;
            }
        }

        public override void OnExitState()
        {
            base.OnExitState();
            if (_agent)
            {
                _agent.isStopped = true;
            }
        }

#if UNITY_EDITOR
        // Draw gizmos in the editor to visualize raycasts and the path
        private void OnDrawGizmos()
        {
            if (_brain && _brain.Target)
            {
                Vector3 directionToTarget = (_brain.Target.position - transform.position).normalized;

                Gizmos.color = Color.red;

                // Draw the multiple raycasts
                for (int i = 0; i < NumberOfRays; i++)
                {
                    float rayAngle = (-AngleBetweenRays * (NumberOfRays - 1) / 2.0f) + (AngleBetweenRays * i);
                    Vector3 rayDirection = Quaternion.Euler(0, rayAngle, 0) * directionToTarget;
                    Gizmos.DrawRay(transform.position, rayDirection * ObstacleCheckDistance);
                }

                // Draw the path of the NavMeshAgent
                if (_agent && _agent.path != null)
                {
                    Gizmos.color = Color.blue;
                    var path = _agent.path;
                    if (path.corners.Length > 1)
                    {
                        for (int i = 1; i < path.corners.Length; i++)
                        {
                            Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);
                        }
                    }
                }
            }
        }
#endif
    }
}
