using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventVirus : WorldEvent
{
    [Header("Number of Viruses")]
    public int MinimumSpawn = 5;
    public int MaximumSpawn = 5;

    [Header("Initial Spread")]
    public Vector2 MinimumSpawnOffset = new Vector2 (-1, -1);
    public Vector2 MaximumSpawnOffset = new Vector2 (1, 1);

    public override void PerformWorldEvent()
    {
        GameObject container = new GameObject($"Container_{Label}");

        for (int i = 0; i < Random.Range(MinimumSpawn, MaximumSpawn); i++)
        {
            Vector2 randomOffset = new Vector2(Random.Range(MinimumSpawnOffset.x, MaximumSpawnOffset.x), 
                                               Random.Range(MinimumSpawnOffset.y, MaximumSpawnOffset.y));

            Instantiate(EventObject, SpawnPosition + randomOffset, Quaternion.identity, container.transform);
        }
    }
}
