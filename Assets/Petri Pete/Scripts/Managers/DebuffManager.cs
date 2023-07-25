using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The DebuffManager holds coroutines for triggering a variety of debuffs.
/// This allows for debuffing objects to be disabled, without interfering with their functionality.
/// </summary>
public class DebuffManager : Singleton<DebuffManager>
{
    public virtual void TriggerDebuff(Character attacker, DebuffTypes[] types, Character target)
    {
        CharacterDebuffable targetDebuffable = target.GetAbility<CharacterDebuffable>();
        if (targetDebuffable == null) { return; }

        foreach (DebuffTypes type in types)
        {
            switch (type)
            {
                default:
                    break;

                case DebuffTypes.DamageOverTime:
                    //target.Health.DamageOverTime();
                    break;
            }
        }
    }
}
