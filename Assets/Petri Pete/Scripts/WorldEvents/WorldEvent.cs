using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New WorldEvent", menuName = "ScriptableObjects/WorldEvent")]
public abstract class WorldEvent : ScriptableObject
{
    public string Label;

    [Header("Debug")]
    public Color GizmoColor = Color.green;
    public bool DisplayDebug = true;

    public abstract void PerformWorldEvent();

    [Header("Spawn Info")]
    public float Delay = 0.5f;
    public float MaxDistance = 20f;
    public float MinDistanceFromSelf = 0f;
    public const int MaxAttempts = 20;

    public GameObject EventObject;

    protected Vector2 _spawnPosition = Vector2.zero;
    protected Timer _delayTimer;
    protected List<Vector2> previousSpawnPositions = new List<Vector2>();

    public virtual void Initialization()
    {
        _delayTimer = new Timer(Delay, null, StartEvent);
        _delayTimer.StartTimer();
    }

    protected virtual void StartEvent()
    {
        RandomizeSpawnPosition();
        PerformWorldEvent();
    }

    protected virtual void RandomizeSpawnPosition()
    {
        Vector2 newPosition;
        int attempts = 0;

        do
        {
            newPosition = new Vector2(Random.Range(-MaxDistance, MaxDistance), Random.Range(-MaxDistance, MaxDistance));
            attempts++;
        }
        while (!IsValidSpawnPosition(newPosition) && attempts < MaxAttempts);


        previousSpawnPositions.Add(newPosition); // Store this new spawn position for future checks.
        _spawnPosition = newPosition;
    }

    protected virtual bool IsValidSpawnPosition(Vector2 position)
    {
        // Check if the distance from the origin is within the acceptable range.
        if (Vector2.Distance(Vector2.zero, position) > MaxDistance)
        {
            return false;
        }

        // Check if the distance from previous spawn positions is acceptable.
        foreach (Vector2 previousPosition in previousSpawnPositions)
        {
            if (Vector2.Distance(previousPosition, position) < MinDistanceFromSelf)
            {
                return false; // It's too close to a previously spawned position.
            }
        }

        return true; // The position is valid.
    }

    public virtual void UpdateWorldEvent()
    {
        _delayTimer.UpdateTimer();
    }

    public virtual void DrawGizmos()
    {
        if (!DisplayDebug) { return; }

        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(Vector3.zero, MaxDistance);
        Gizmos.DrawSphere(_spawnPosition, 0.5f);
    }
}
