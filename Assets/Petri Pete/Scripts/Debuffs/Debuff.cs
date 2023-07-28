using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a clonable interface with a method to clone objects of a generic type.
/// </summary>
/// <typeparam name="T">Type of object to be cloned.</typeparam>
public interface ICloneable<T>
{
    T Clone();
}

/// <summary>
/// Represents a debuff that can be applied to a character. This class should be extended to create
/// specific types of debuffs.
/// </summary>
public class Debuff : ScriptableObject, ICloneable<Debuff>
{
    public string Label = "";
    public float Duration = 6f;
    public float Interval = 2f;
    public bool Stackable = false;

    /// <summary>
    /// Indicates whether the debuff has finished its effect.
    /// </summary>
    public bool DebuffFinished { get; set; }

    /// <summary>
    /// The character being affected by this debuff.
    /// </summary>
    public Character TargetCharacter { get; set; }

    /// <summary>
    /// The CharacterDebuffable component associated with the target character.
    /// </summary>
    public CharacterDebuffable TargetDebuffable { get; set; }

    protected Timer _debuffTimer;
    protected Timer _tickTimer;

    /// <summary>
    /// Clones the current debuff instance.
    /// Note: This should be overridden in child classes.
    /// </summary>
    public virtual Debuff Clone()
    {
        return Instantiate(this);
    }

    /// <summary>
    /// Sets up and starts the debuff timers.
    /// </summary>
    public virtual void Initialize()
    {
        SetAndFormatLabel();

        _debuffTimer = new Timer("DebuffTimer", Duration, OnActivated, RemoveDebuff);
        _tickTimer = new Timer("TickTimer", Interval, null, DebuffTick);
        _debuffTimer.StartTimer();
        _tickTimer.StartTimer();
    }

    /// <summary>
    /// Called at regular intervals to apply debuff effects and restart the tick timer.
    /// </summary>
    protected virtual void DebuffTick()
    {
        ProcessDebuff();
        _tickTimer.ResetTimer();
        _tickTimer.StartTimer();
    }

    /// <summary>
    /// Effects applied when the debuff starts.
    /// </summary>
    protected virtual void OnActivated() { }

    /// <summary>
    /// Effects applied when the debuff ends.
    /// </summary>
    protected virtual void OnDeactivated() { }

    /// <summary>
    /// Contains the logic for the effects of the debuff. Called at regular intervals during the debuff's duration.
    /// </summary>
    protected virtual void ProcessDebuff() { }

    /// <summary>
    /// Removes the debuff and stops its timers.
    /// </summary>
    public virtual void RemoveDebuff()
    {
        _debuffTimer.StopTimer();
        _tickTimer.StopTimer();
        OnDeactivated();
        DebuffFinished = true;
    }

    /// <summary>
    /// Resets and restarts the debuff timer, re-applying the debuff.
    /// </summary>
    public virtual void RefreshDebuff()
    {
        Debug.Log($"{this.GetType()}.RefreshDebuff: Refreshing debuff with base class on {TargetCharacter.name}.", TargetCharacter);
        _debuffTimer.ResetTimer();
        _debuffTimer.StartTimer();
        OnActivated();
    }

    /// <summary>
    /// Updates the timers of the debuff. This should be called in the Update method of CharacterDebuffable.
    /// </summary>
    public virtual void UpdateDebuff()
    {
        _debuffTimer.UpdateTimer();
        _tickTimer.UpdateTimer();
    }

    /// <summary>
    /// Sets the target character and associated CharacterDebuffable component for this debuff.
    /// </summary>
    /// <param name="target">The CharacterDebuffable component of the target character.</param>
    public virtual void SetTargetCharacter(CharacterDebuffable target)
    {
        TargetCharacter = target.Character;
        TargetDebuffable = target;
    }

    /// <summary>
    /// Sets and formats the label to the name of the ScriptableObject, removing "Debuff" from the start if it exists.
    /// </summary>
    public void SetAndFormatLabel()
    {
        // Get the type's name.
        string name = GetType().Name;

        // If the name starts with "Debuff", remove it.
        if (name.StartsWith("Debuff"))
        {
            name = name.Substring(6);  // "Debuff" has 6 characters.
        }

        // Set the label.
        Label = name;
    }
}
