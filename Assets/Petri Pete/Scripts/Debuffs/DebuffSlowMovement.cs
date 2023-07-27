using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff", menuName = "ScriptableObjects/Debuff/SlowMovement", order = 2)]
public class DebuffSlowMovement : Debuff, ICloneable<DebuffSlowMovement>
{
    [Range(0f, 1f)]
    public float SlowPercentage = 0.5f;
    
    protected CharacterMovement _characterMovement;

    public override void Initialize()
    {
        _characterMovement = TargetCharacter.GetAbility<CharacterMovement>();
        base.Initialize();
    }

    public new DebuffSlowMovement Clone()
    {
        return Instantiate(this);
    }

    /// <summary>
    /// Refreshes the debuff timer, without its OnActivated effect.
    /// </summary>
    public override void RefreshDebuff()
    {
        _debuffTimer.ResetTimer();
        _debuffTimer.StartTimer(false);
    }

    protected override void OnActivated()
    {
        Debug.Log($"{this.GetType()}.OnActivated: Applying slow to {TargetCharacter.name}.", TargetCharacter);
        _characterMovement.MovementSpeedMultiplier *= SlowPercentage;
    }

    protected override void OnDeactivated()
    {
        _characterMovement.MovementSpeedMultiplier /= SlowPercentage;
    }
}
