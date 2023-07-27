using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager : Singleton<WorldEventManager>
{
    public List<WorldEvent> AvailableWorldEvents;
    public List<WorldEvent> ActivatedWorldEvents;

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
        EventTriggerTimer = new Timer(EventTriggerTimerDuration, null, OnTimerCompleted);
        EventTriggerTimer.StartTimer();
    }

    protected virtual void OnTimerCompleted()
    {
        TriggerRandomEvent();
    }

    public virtual void TriggerRandomEvent()
    {
        int randomIndex = Random.Range(0, AvailableWorldEvents.Count);

        WorldEvent triggeredEvent = AvailableWorldEvents[randomIndex];

        ActivatedWorldEvents.Add(triggeredEvent);
        AvailableWorldEvents.Remove(triggeredEvent);

        //Debug.Log($"{this.GetType()}.TriggerRandomEvent: Triggering {triggeredEvent.name}. Remaining Events: {AvailableWorldEvents.Count}", gameObject);

        triggeredEvent.Initialization();

        if (AvailableWorldEvents.Count <= 0)
        {
            //Debug.Log($"{this.GetType()}.TriggerRandomEvent: Stopping EventTriggerTimer.", gameObject);
            EventTriggerTimer.StopTimer();
            return;
        }
        EventTriggerTimer.ResetTimer();
        EventTriggerTimer.StartTimer();
    }

    protected virtual void Update()
    {
        if (EventTriggerTimer == null) { return; }

        EventTriggerTimer.UpdateTimer();

        if (ActivatedWorldEvents.Count <= 0) { return; }

        foreach(WorldEvent worldEvent in ActivatedWorldEvents)
        {
            worldEvent.UpdateWorldEvent();
        }
    }

    protected virtual void OnDrawGizmos()
    {
        foreach (WorldEvent worldEvent in AvailableWorldEvents)
        {
            worldEvent.DrawGizmos();
        }
        foreach (WorldEvent worldEvent in ActivatedWorldEvents)
        {
            worldEvent.DrawGizmos();
        }
    }
}
