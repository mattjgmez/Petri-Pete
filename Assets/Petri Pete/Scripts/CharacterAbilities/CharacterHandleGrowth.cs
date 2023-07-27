using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandleGrowth : CharacterAbility
{
    [Header("Growth Info")]
    public GameObject GrowthObject;
    public List<GrowthObject> ActiveGrowthObjects = new List<GrowthObject>();
    public int StartingGrowths = 5;
    public float GrowthInterval = 10f;
    public int MinGrowthAmount = 1;
    public int MaxGrowthAmount = 3;

    protected Timer _growthTimer;
    protected List<GrowthObject> _validGrowthObjects = new List<GrowthObject>();
    protected int AvailableGrowthPoints = 0;

    protected override void Initialization()
    {
        base.Initialization();

        Debug.Log($"{this.GetType()}.Initialization: Initializing Growth Hub.", gameObject);

        GetValidGrowthObjects();
        SpawnGrowth(StartingGrowths);

        _growthTimer = new Timer(GrowthInterval, null, TriggerGrowth);
        _growthTimer.StartTimer();
    }

    public override void ProcessAbility()
    {
        _growthTimer.UpdateTimer();
    }

    public virtual void TriggerGrowth()
    {
        SpawnGrowth();
        _growthTimer.ResetTimer();
        _growthTimer.StartTimer();
    }

    protected virtual void SpawnGrowth()
    {
        if (GrowthObject == null) 
        {
            Debug.LogWarning($"{this.GetType()}.SpawnGrowth: Attempting to spawn growth with no GrowthObject set.", gameObject);
            return; 
        }

        GetValidGrowthObjects();

        // We get random growth amount.
        int growthAmount = Mathf.Clamp(Random.Range(MinGrowthAmount, MaxGrowthAmount), 0, AvailableGrowthPoints);

        for (int i = 0; i < growthAmount; i++)
        {
            GrowthObject randomGrowthObject = _validGrowthObjects.GetRandom();

            Vector2 spawnPosition = randomGrowthObject.GrowthSpawnTransform.position;

            GameObject newObject = SpawnManager.Instance.SpawnAtPosition(spawnPosition, GrowthObject.name);
            newObject.transform.rotation = JP_Math.GetRandomRotationAwayFromPoint2D(spawnPosition, transform.position, 45f);
            randomGrowthObject.AddGrowthObject(newObject);

            GrowthObject newGrowth = newObject.GetComponent<GrowthObject>();
            ActiveGrowthObjects.Add(newGrowth);
            _validGrowthObjects.Remove(randomGrowthObject);
        }

        GetValidGrowthObjects();
    }

    protected virtual void SpawnGrowth(int growthAmount)
    {
        if (GrowthObject == null) { return; }

        GetValidGrowthObjects();

        if (growthAmount > AvailableGrowthPoints) { growthAmount = AvailableGrowthPoints; }

        for (int i = 0; i < growthAmount; i++)
        {
            GrowthObject randomGrowthObject = _validGrowthObjects.GetRandom();

            Vector2 spawnPosition = randomGrowthObject.GrowthSpawnTransform.position;

            GameObject newObject = SpawnManager.Instance.SpawnAtPosition(spawnPosition, GrowthObject.name);
            newObject.transform.rotation = JP_Math.GetRandomRotationAwayFromPoint2D(spawnPosition, transform.position, 45f);
            randomGrowthObject.AddGrowthObject(newObject);

            GrowthObject newGrowth = newObject.GetComponent<GrowthObject>();
            ActiveGrowthObjects.Add(newGrowth);
            _validGrowthObjects.Remove(randomGrowthObject);
        }

        GetValidGrowthObjects();
    }

    protected virtual void GetValidGrowthObjects()
    {
        AvailableGrowthPoints = 0;

        _validGrowthObjects = new List<GrowthObject>();

        foreach (GrowthObject growthObject in ActiveGrowthObjects)
        {
            if (growthObject.CurrentGrowths.Count > growthObject.MaxGrowths) { continue; }

            _validGrowthObjects.Add(growthObject);
            AvailableGrowthPoints += growthObject.MaxGrowths - growthObject.CurrentGrowths.Count;
            growthObject.SetGrowthHub(_character);
        }
    }
}
