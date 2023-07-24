using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager : Singleton<WorldEventManager>
{
    public List<WorldEvent> AvailableWorldEvents;

    [Header("Timer")]
    public Timer EventTriggerTimer;
    /// Duration of the timer in seconds
    public float EventTriggerTimerDuration = 120;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        AvailableWorldEvents = new List<WorldEvent>(GetComponents<WorldEvent>());
        EventTriggerTimer = new Timer(EventTriggerTimerDuration, null, OnTimerCompleted);
        EventTriggerTimer.StartTimer();
    }

    protected virtual void OnTimerCompleted()
    {
        TriggerRandomEvent();
        EventTriggerTimer.ResetTimer();
        EventTriggerTimer.StartTimer();
    }

    public virtual void TriggerRandomEvent()
    {
        int randomIndex = Random.Range(0, AvailableWorldEvents.Count);

        AvailableWorldEvents[randomIndex].PerformWorldEvent();
        AvailableWorldEvents.RemoveAt(randomIndex);
    }

    protected virtual void Update()
    {
        if (EventTriggerTimer == null || AvailableWorldEvents.Count <= 0) { return; }

        EventTriggerTimer.UpdateTimer();
    }
}
