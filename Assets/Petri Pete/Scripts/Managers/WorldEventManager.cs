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

    [Header("Maximum Active Events")]
    public int MaxBasicEnemyEvents = 1;
    public int MaxObstacleEvents = 2;
    public int MaxBossEvents = 1;

    [Header("Timer")]
    public Timer EventTriggerTimer;
    [Tooltip("Duration of the timer in seconds before the next event gets triggered.")]
    public float EventTriggerTimerDuration = 120;
    [Tooltip("Duration of the timer in seconds before boss events can be spawned.")]
    public float BossSpawnThreshold = 600;

    protected List<WorldEvent> _availableWorldEvents = new List<WorldEvent>();
    protected float _timeSinceStart = 0f;

    /// <summary>
    /// Initialization method called at the start.
    /// </summary>
    protected virtual void Start()
    {
        Initialization();

        // Start the game with a random obstacle and two random basic enemies
        TriggerSpecificEvent(ObstacleEvents);
        TriggerSpecificEvent(BasicEnemyEvents, 2);
    }

    /// <summary>
    /// Sets up initial conditions for the world event manager.
    /// </summary>
    protected virtual void Initialization()
    {
        EventTriggerTimer = new Timer(EventTriggerTimerDuration, null, OnTimerCompleted);
        EventTriggerTimer.StartTimer();
        _availableWorldEvents.AddRange(BasicEnemyEvents);
        _availableWorldEvents.AddRange(ObstacleEvents);
    }

    /// <summary>
    /// Called when the event timer completes its duration.
    /// </summary>
    protected virtual void OnTimerCompleted()
    {
        if (_timeSinceStart > BossSpawnThreshold && !_availableWorldEvents.ContainsRange(BossEvents))
        {
            // The boss can be included in the random pool once the threshold is reached.
            _availableWorldEvents.AddRange(BossEvents);
        }

        TriggerRandomEvent();
    }

    /// <summary>
    /// Triggers a specific number of events from a given event category.
    /// </summary>
    /// <param name="eventCategory">The category of events to select from.</param>
    /// <param name="amount">The number of events to trigger.</param>
    public virtual void TriggerSpecificEvent(List<WorldEvent> eventCategory, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            // Exit if no events in the category
            if (eventCategory.Count <= 0) { return; } 

            int randomIndex = Random.Range(0, eventCategory.Count);
            WorldEvent triggeredEvent = eventCategory[randomIndex];

            ActivatedWorldEvents.Add(triggeredEvent);

            triggeredEvent.Initialization();
        }
    }

    /// <summary>
    /// Selects and triggers a specific number of random events from the available events.
    /// Will cancel if all Categories are at their maximum allowed spawns.
    /// </summary>
    /// <param name="amount">The number of events to trigger.</param>
    public virtual void TriggerRandomEvent(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            // Exit if reached the maximum allowed events
            if (ActivatedWorldEvents.Count >= (MaxBasicEnemyEvents + MaxObstacleEvents + MaxBossEvents)) { return; }

            bool successfullyTriggered = false;
            List<WorldEvent> checkedEvents = new List<WorldEvent>();

            // Keep trying to find a valid event to trigger
            while (!successfullyTriggered && checkedEvents.Count < _availableWorldEvents.Count)
            {
                int randomIndex = Random.Range(0, _availableWorldEvents.Count);
                WorldEvent triggeredEvent = _availableWorldEvents[randomIndex];

                // Avoid checking the same event multiple times
                if (checkedEvents.Contains(triggeredEvent)) { continue; }
                checkedEvents.Add(triggeredEvent);

                // Check if category has reached its limit
                if ((BasicEnemyEvents.Contains(triggeredEvent) && GetCountOfCategoryInActivatedEvents(BasicEnemyEvents) >= MaxBasicEnemyEvents) ||
                    (ObstacleEvents.Contains(triggeredEvent) && GetCountOfCategoryInActivatedEvents(ObstacleEvents) >= MaxObstacleEvents) ||
                    (BossEvents.Contains(triggeredEvent) && GetCountOfCategoryInActivatedEvents(BossEvents) >= MaxBossEvents))
                {
                    continue;
                }

                // Activate the event
                ActivatedWorldEvents.Add(triggeredEvent);
                triggeredEvent.Initialization();
                successfullyTriggered = true;

                EventTriggerTimer.ResetTimer();
                EventTriggerTimer.StartTimer();
            }

            // If all categories are maxed out
            if (!successfullyTriggered) { return; }
        }
    }

    /// <summary>
    /// Removes a specified world event from the list of activated events.
    /// </summary>
    /// <param name="worldEvent">The world event to remove.</param>
    public void RemoveActivatedEvent(WorldEvent worldEvent)
    {
        ActivatedWorldEvents.Remove(worldEvent);
    }

    /// <summary>
    /// Sets the maximum number of activated events for a given category.
    /// </summary>
    /// <param name="category">The category of events.</param>
    /// <param name="maxCount">The maximum number of events.</param>
    public void SetMaxForCategory(List<WorldEvent> category, int maxCount)
    {
        if (category == BasicEnemyEvents) MaxBasicEnemyEvents = maxCount;
        else if (category == ObstacleEvents) MaxObstacleEvents = maxCount;
        else if (category == BossEvents) MaxBossEvents = maxCount;
    }

    /// <summary>
    /// Helper method to get the count of a specific category in the activated events list.
    /// </summary>
    private int GetCountOfCategoryInActivatedEvents(List<WorldEvent> category)
    {
        int count = 0;
        foreach (var ev in ActivatedWorldEvents)
        {
            if (category.Contains(ev)) count++;
        }
        return count;
    }

    /// <summary>
    /// Update method to manage world events and timers.
    /// </summary>
    protected virtual void Update()
    {
        _timeSinceStart += Time.deltaTime;

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
