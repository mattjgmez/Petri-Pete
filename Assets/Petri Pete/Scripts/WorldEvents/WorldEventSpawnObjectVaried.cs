using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WorldEventSpawnObjectVaried", menuName = "ScriptableObjects/WorldEvent/SpawnObjectVaried")]
public class WorldEventSpawnObjectVaried : WorldEventSpawnObject
{
    [Header("ObjectsToSpawn")]
    public List<GameObject> EventObjects = new List<GameObject>();

    protected override void SpawnObjects()
    {
        for (int i = 0; i < Random.Range(MinimumSpawn, MaximumSpawn); i++)
        {
            GetRandomObject();

            Vector2 randomOffset = new Vector2(Random.Range(MinimumSpawnOffset.x, MaximumSpawnOffset.x),
                                               Random.Range(MinimumSpawnOffset.y, MaximumSpawnOffset.y));

            SpawnManager.Instance.SpawnAtPosition(_spawnPosition + randomOffset, EventObject.name);
        }
    }

    protected virtual void GetRandomObject()
    {
        EventObject = EventObjects[Random.Range(0, EventObjects.Count)];
    }
}
