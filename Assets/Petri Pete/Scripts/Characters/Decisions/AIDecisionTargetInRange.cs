using UnityEngine;
using JadePhoenix.Tools;

namespace JadePhoenix.Tools
{
    public class AIDecisionTargetInRange : AIDecision
    {
        [Tooltip("The maximum range within which the target should be for this decision to return true.")]
        public float Range = 5.0f;

        public override bool Decide()
        {
            return IsTargetInRange();
        }

        protected virtual bool IsTargetInRange()
        {
            if (!_brain.Target)
                return false;

            float distanceToTarget = Vector3.Distance(this.transform.position, _brain.Target.position);

            return distanceToTarget <= Range;
        }

#if UNITY_EDITOR
        // Visualization of the decision range in the Unity editor.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Range);
        }
#endif
    }
}
