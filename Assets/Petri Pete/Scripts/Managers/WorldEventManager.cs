using JadePhoenix.Tools;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager : Singleton<WorldEventManager>
{
    [Header("Events")]
    public List<WorldEvent> BasicEnemyEvents;
    public List<WorldEvent> ObstacleEvents;
    public List<WorldEvent> BossEvents;
    public List<WorldEvent> ActivatedWorldEvents;

    [Header("Timer")]
    public Timer EventTriggerTimer;
    [Tooltip("Duration of the timer in seconds before the next event gets triggered.")]
    public float EventTriggerTimerDuration = 120;
    [Tooltip("Duration of the timer in seconds before boss events can be spawned.")]
    public float BossSpawnThreshold = 600;

    protected List<WorldEvent> AvailableWorldEvents = new List<WorldEvent>();
    protected float timeSinceStart = 0f;

    /// <summary>
    /// Initialization method called at the start.
    /// </summary>
    protected virtual void Start()
    {
        Initialization();

        // Start the game with a random obstacle and two random basic enemies
        TriggerSpecificEvent(ObstacleEvents);
        TriggerSpecificEvent(BasicEnemyEvents);
        TriggerSpecificEvent(BasicEnemyEvents);
    }

    /// <summary>
    /// Sets up initial conditions for the world event manager.
    /// </summary>
    protected virtual void Initialization()
    {
        EventTriggerTimer = new Timer(EventTriggerTimerDuration, null, OnTimerCompleted);
        EventTriggerTimer.StartTimer();
        AvailableWorldEvents.AddRange(BasicEnemyEvents);
        AvailableWorldEvents.AddRange(ObstacleEvents);
    }

    /// <summary>
    /// Called when the event timer completes its duration.
    /// </summary>
    protected virtual void OnTimerCompleted()
    {
        if (timeSinceStart > BossSpawnThreshold && !AvailableWorldEvents.ContainsRange(BossEvents))
        {
            // The boss can be included in the random pool once the threshold is reached.
            AvailableWorldEvents.AddRange(BossEvents);
        }

        TriggerRandomEvent();
    }

    /// <summary>
    /// Triggers a specific event from a given event category.
    /// </summary>
    public virtual void TriggerSpecificEvent(List<WorldEvent> eventCategory)
    {
        int randomIndex = Random.Range(0, eventCategory.Count);
        WorldEvent triggeredEvent = eventCategory[randomIndex];

        ActivatedWorldEvents.Add(triggeredEvent);
        eventCategory.RemoveAt(randomIndex);

        triggeredEvent.Initialization();
    }

    /// <summary>
    /// Selects and triggers a random event from the available events.
    /// </summary>
    public virtual void TriggerRandomEvent()
    {
        int randomIndex = Random.Range(0, AvailableWorldEvents.Count);
        WorldEvent triggeredEvent = AvailableWorldEvents[randomIndex];
        ActivatedWorldEvents.Add(triggeredEvent);

        triggeredEvent.Initialization();

        EventTriggerTimer.ResetTimer();
        EventTriggerTimer.StartTimer();
    }

    /// <summary>
    /// Update method to manage world events and timers.
    /// </summary>
    protected virtual void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (EventTriggerTimer == null) return;
        EventTriggerTimer.UpdateTimer();

        if (ActivatedWorldEvents.Count <= 0) return;

        foreach (WorldEvent worldEvent in ActivatedWorldEvents)
        {
            worldEvent.UpdateWorldEvent();
        }
    }

    /// <summary>
    /// Draws gizmos for visualization in the Unity Editor.
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        foreach (WorldEvent worldEvent in BasicEnemyEvents)
        {
            worldEvent.DrawGizmos();
        }
        foreach (WorldEvent worldEvent in ObstacleEvents)
        {
            worldEvent.DrawGizmos();
        }
        foreach (WorldEvent worldEvent in BossEvents)
        {
            worldEvent.DrawGizmos();
        }
        foreach (WorldEvent worldEvent in ActivatedWorldEvents)
        {
            worldEvent.DrawGizmos();
        }
    }
}
