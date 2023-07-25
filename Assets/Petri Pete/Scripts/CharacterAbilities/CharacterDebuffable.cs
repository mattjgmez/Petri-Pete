using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDebuffable : CharacterAbility
{
    public List<Debuff> ActiveDebuffs = new List<Debuff>();
    public float DebuffTickInterval = 0.5f;

    protected Timer _tickTimer;

    protected override void Initialization()
    {
        base.Initialization();
        _tickTimer = new Timer(DebuffTickInterval, null, TriggerDebuffs);
    }

    public virtual void TriggerDebuffs()
    {
        foreach (Debuff debuff in ActiveDebuffs)
        {
            debuff.ProcessDebuff();
        }
        _tickTimer.ResetTimer();
        _tickTimer.StartTimer();
    }

    public virtual void AddActiveDebuff(Debuff debuffToAdd, Type debuffType)
    {
        // Check if the debuffToAdd is a subclass of Debuff
        if (!debuffToAdd.GetType().IsSubclassOf(typeof(Debuff)))
        {
            Debug.LogError("Invalid debuff type provided.");
            return;
        }

        // Check if the debuff is stackable
        bool isStackable = debuffToAdd.Stackable;

        // Find an existing debuff of the same type (if not stackable)
        Debuff existingDebuff = null;
        if (!isStackable)
        {
            existingDebuff = ActiveDebuffs.FirstOrDefault(debuff => debuff.GetType() == debuffType);

            // If the debuff is not stackable and an instance already exists, refresh it
            if (existingDebuff != null)
            {
                existingDebuff.RefreshDebuff();
                return;
            }
        }

        // Create a new debuff instance based on the provided debuff type
        Debuff newDebuff = (Debuff)Activator.CreateInstance(debuffType, new object[] { debuffToAdd, _character });

        ActiveDebuffs.Add(newDebuff);
    }
}
