using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICloneable<T>
{
    T Clone();
}

//[CreateAssetMenu(fileName = "Debuff", menuName = "ScriptableObjects/Debuff", order = 0)]
public class Debuff : ScriptableObject, ICloneable<Debuff>
{
    public float Duration = 6f;
    public float Interval = 2f;
    public bool Stackable = false;
    /// Signals the relevant CharacterDebuffable to remove this debuff from the list.
    public bool DebuffFinished { get; set; }
    public Character TargetCharacter { get; set; }
    public CharacterDebuffable TargetDebuffable { get; set; }

    protected Timer _debuffTimer;
    protected Timer _tickTimer;

    /// <summary>
    /// This needs to be overwritten using the "new" keyword in every deriving class, replacing "Debuff" with the appropriate type.
    /// </summary>
    public virtual Debuff Clone() 
    {
        return Instantiate(this);
    }

    /// <summary>
    /// Initializes the debuff.
    /// </summary>
    public virtual void Initialize()
    {
        _debuffTimer = new Timer("DebuffTimer", Duration, OnActivated, RemoveDebuff);
        _tickTimer = new Timer("TickTimer", Interval, null, DebuffTick);
        _debuffTimer.StartTimer();
        _tickTimer.StartTimer();
    }

    /// <summary>
    /// Method triggered by the TickTimer. Used to reset the timer and call ProcessDebuff.
    /// </summary>
    protected virtual void DebuffTick()
    {
        ProcessDebuff();
        _tickTimer.ResetTimer();
        _tickTimer.StartTimer();
    }

    /// <summary>
    /// Effects triggered at the start of a Debuff's duration.
    /// </summary>
    protected virtual void OnActivated() { }

    /// <summary>
    /// Effects triggered at the end of a Debuff's duration.
    /// </summary>
    protected virtual void OnDeactivated() { }

    /// <summary>
    /// Effects triggered after each interval.
    /// </summary>
    protected virtual void ProcessDebuff() { }

    /// <summary>
    /// Removes the debuff, triggering its OnDeactivated effect.
    /// </summary>
    public virtual void RemoveDebuff()
    {
        _debuffTimer.StopTimer();
        _tickTimer.StopTimer();
        OnDeactivated();
        DebuffFinished = true;
    }

    /// <summary>
    /// Refreshes the debuff timer, triggering its OnActivated effect.
    /// </summary>
    public virtual void RefreshDebuff()
    {
        Debug.Log($"{this.GetType()}.RefreshDebuff: Refreshing debuff with base class on {TargetCharacter.name}.", TargetCharacter);
        _debuffTimer.ResetTimer();
        _debuffTimer.StartTimer();
        OnActivated();
    }

    /// <summary>
    /// Called in the Update method of CharacterDebuffable.
    /// </summary>
    public virtual void UpdateDebuff()
    {
        _debuffTimer.UpdateTimer();
        _tickTimer.UpdateTimer();
    }

    public virtual void SetTargetCharacter(CharacterDebuffable target)
    {
        TargetCharacter = target.Character;
        TargetDebuffable = target;
    }
}
