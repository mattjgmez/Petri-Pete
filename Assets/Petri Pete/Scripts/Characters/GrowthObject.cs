using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableObject))]
public class GrowthObject : MonoBehaviour
{
    public Transform GrowthSpawnTransform;
    public Character GrowthHub;
    public int MaxGrowths = 3;
    public List<GameObject> CurrentGrowths = new List<GameObject>();

    public virtual void SetGrowthHub(Character growthHub)
    {
        if (growthHub != null) { return; }
        GrowthHub = growthHub;
    }

    public virtual void AddGrowthObject(GameObject growth)
    {
        if (growth == null) { return; }
        if (CurrentGrowths.Contains(growth)) { return; }
        if (CurrentGrowths.Count + 1 > MaxGrowths)
        {
            // This is an active issue but I already know
            //Debug.LogWarning($"{this.GetType()}.AddGrowthObject: Attempting to add growth object above maximum.", gameObject);
        }

        CurrentGrowths.Add(growth);
    }

    public virtual void RemoveGrowthObject(GameObject growth)
    {
        if (growth == null) { return; }
        if (!CurrentGrowths.Contains(growth)) {  return; }

        CurrentGrowths.Remove(growth);
    }

    public virtual void OnDisable()
    {
        if (GrowthHub != null)
        {
            GrowthHub.GetAbility<CharacterHandleGrowth>().RemoveActiveGrowthObject(this);
        }
    }
}
