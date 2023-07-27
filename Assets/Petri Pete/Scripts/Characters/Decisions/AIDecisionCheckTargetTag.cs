using JadePhoenix.Tools;
using UnityEngine;

public class AIDecisionCheckTargetTag : AIDecision
{
    [Tooltip("The specific tag to check against the target's tag.")]
    public string TargetTag;

    /// <summary>
    /// On Decide we check if our target's tag matches the specified tag
    /// </summary>
    /// <returns></returns>
    public override bool Decide()
    {
        return CheckTargetTag();
    }

    /// <summary>
    /// Returns true if the target's tag matches the specified tag
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckTargetTag()
    {
        if (_brain.Target != null)
        {
            return _brain.Target.CompareTag(TargetTag);
        }
        return false;
    }
}
