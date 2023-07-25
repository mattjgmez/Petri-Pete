using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldEvent : MonoBehaviour
{
    public string Label;
    public Color GizmoColor = Color.green;
    public abstract void PerformWorldEvent();
    public float Delay = 0.5f;
    public float MaxDistance = 45f;
    public Vector2 SpawnPosition = Vector2.zero;

    public GameObject EventObject;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        RandomizeSpawnPosition();
    }

    protected virtual void RandomizeSpawnPosition()
    {
        Vector2 newPosition = new Vector2(Random.Range(-MaxDistance, MaxDistance), Random.Range(-MaxDistance, MaxDistance));

        while (Vector2.Distance(Vector2.zero, newPosition) > MaxDistance)
        {
            newPosition = new Vector2(Random.Range(-MaxDistance, MaxDistance), Random.Range(-MaxDistance, MaxDistance));
        }

        SpawnPosition = newPosition;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
        Gizmos.DrawSphere(SpawnPosition, 0.5f);
    }
}
