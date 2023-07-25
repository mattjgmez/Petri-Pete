using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAndDebuffOnTouch : DamageOnTouch
{
    public List<Debuff> Debuffs;

    protected override void OnCollideWithDamageable()
    {
        base.OnCollideWithDamageable();

        CharacterDebuffable debuffTarget = _colliderHealth.Character.GetAbility<CharacterDebuffable>();
        if (debuffTarget != null)
        {
            foreach (Debuff debuff in Debuffs)
            {
                Type debuffType = debuff.GetType();
                debuffTarget.AddActiveDebuff(debuff, debuffType);
            }
        }
    }
}
