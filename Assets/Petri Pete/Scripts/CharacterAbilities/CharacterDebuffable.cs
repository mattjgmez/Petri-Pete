using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDebuffable : CharacterAbility
{
    public List<Debuff> ActiveDebuffs = new List<Debuff>();

    protected override void Initialization()
    {
        base.Initialization();
    }

    public override void ProcessAbility()
    {
        List<Debuff> debuffsToRemove = new List<Debuff>();

        foreach (Debuff debuff in ActiveDebuffs)
        {
            debuff.UpdateDebuff();

            // If the debuff needs to be removed, add it to the removal list.
            if (debuff.DebuffFinished)
            {
                debuffsToRemove.Add(debuff);
            }
        }

        // Remove the debuffs that were marked for removal.
        foreach (Debuff debuffToRemove in debuffsToRemove)
        {
            RemoveDebuff(debuffToRemove);
        }
    }

    public virtual void AddDebuffs(List<Debuff> debuffsToAdd)
    {
        foreach(Debuff debuff in debuffsToAdd)
        {
            if (!debuff.Stackable && ActiveDebuffs.Any(existingDebuff => debuff.GetType().IsAssignableFrom(existingDebuff.GetType())))
            {
                Debuff foundDebuff = ActiveDebuffs.FirstOrDefault(existingDebuff => debuff.GetType().IsAssignableFrom(existingDebuff.GetType()));
                foundDebuff.RefreshDebuff();
                continue;
            }

            Debuff newDebuff = debuff.Clone();
            ActiveDebuffs.Add(newDebuff);
            newDebuff.SetTargetCharacter(this);
            newDebuff.Initialize();

            if (_character.PlayerID == "Player" && UIManager.Instance != null)
            {
                UIManager.Instance.AddJournalEntryWithID(newDebuff.Label);
            }            
        }
    }

    public virtual void RemoveDebuff(Debuff debuffToRemove) 
    {
        ActiveDebuffs.Remove(debuffToRemove);
    }
}
