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
    public bool DebuffFinished = false;
    public Character TargetCharacter;
    public CharacterDebuffable TargetDebuffable;

    protected Timer _debuffTimer;
    protected Timer _tickTimer;

    public Debuff(float duration, bool stackable, Character targetCharacter, Timer debuffTimer)
    {
        Duration = duration;
        Stackable = stackable;
        TargetCharacter = targetCharacter;
        _debuffTimer = debuffTimer;
    }

    public Debuff(Debuff debuff)
    {
        Duration = debuff.Duration;
        Stackable = debuff.Stackable;
        TargetCharacter = debuff.TargetCharacter;
    }

    /// <summary>
    /// This needs to be overwritten in every deriving class, replacing "Debuff" with the appropriate type.
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
        _debuffTimer.ResetTimer();
        _debuffTimer.StartTimer();
        _tickTimer.ResetTimer();
        _tickTimer.StartTimer();
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
