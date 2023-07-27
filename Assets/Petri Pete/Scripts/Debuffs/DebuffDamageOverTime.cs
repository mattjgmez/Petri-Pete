using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DebuffDamageOverTime", menuName = "ScriptableObjects/Debuff/DamageOverTime", order = 2)]
public class DebuffDamageOverTime : Debuff, ICloneable<DebuffDamageOverTime>
{
    public int DamagePerTick = 3;

    protected Health _targetHealth;

    public new DebuffDamageOverTime Clone()
    {
        return Instantiate(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        _targetHealth = TargetCharacter.Health;
    }

    protected override void ProcessDebuff()
    {
        _targetHealth.Damage(DamagePerTick, null, 0);
    }
}
