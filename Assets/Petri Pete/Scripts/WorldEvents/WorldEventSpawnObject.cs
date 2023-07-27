using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WorldEvent", menuName = "ScriptableObjects/WorldEvent/SpawnObject")]
public class WorldEventSpawnObject : WorldEvent
{
    [Header("Number of Spawns")]
    public int MinimumSpawn = 10;
    public int MaximumSpawn = 15;

    [Header("Initial Spread")]
    public Vector2 MinimumSpawnOffset = new Vector2(-1, -1);
    public Vector2 MaximumSpawnOffset = new Vector2(1, 1);

    public override void PerformWorldEvent()
    {
        SpawnObjects();
    }

    protected virtual void SpawnObjects()
    {
        for (int i = 0; i < Random.Range(MinimumSpawn, MaximumSpawn); i++)
        {
            Vector2 randomOffset = new Vector2(Random.Range(MinimumSpawnOffset.x, MaximumSpawnOffset.x),
                                               Random.Range(MinimumSpawnOffset.y, MaximumSpawnOffset.y));

            SpawnManager.Instance.SpawnAtPosition(_spawnPosition + randomOffset, EventObject.name);
        }
    }
}
