using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public bool CanSpawn = true;
    public ObjectPooler ObjectPooler {  get; set; }

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        if (GetComponent<MultipleObjectPooler>() != null)
        {
            ObjectPooler = GetComponent<MultipleObjectPooler>();
        }
        if (GetComponent<SimpleObjectPooler>() != null)
        {
            ObjectPooler = GetComponent<SimpleObjectPooler>();
        }
        if (ObjectPooler == null)
        {
            Debug.LogWarning($"{this.GetType()}.Initialization: no object pooler (simple or multiple) is attached to the SpawnManager, it won't be able to spawn anything.", gameObject);
        }
    }

    public virtual void SpawnAtPosition(Vector3 positionToSpawn)
    {
        GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

        if (nextGameObject == null ) { return; }
        if (nextGameObject.GetComponent<PoolableObject>() == null)
        {
            throw new Exception($"{this.GetType()}.SpawnAtPosition: {this.gameObject.name} is attempting to spawn an object {nextGameObject.name} that does not have a PoolableObject component.");
        }

        nextGameObject.SetActive(true);
        nextGameObject.GetComponent<PoolableObject>().TriggerOnSpawnComplete();

        Health objectHealth = nextGameObject.GetComponent<Health>();
        if (objectHealth != null)
        {
            objectHealth.Revive();
        }

        nextGameObject.transform.position = positionToSpawn;
    }

    public virtual void SpawnAtPosition(Vector3 positionToSpawn, string objectName)
    {
        if (ObjectPooler.GetType() != typeof(MultipleObjectPooler))
        {
            SpawnAtPosition(positionToSpawn);
            return;
        }

        MultipleObjectPooler objectPooler = (MultipleObjectPooler)ObjectPooler;

        GameObject nextGameObject = objectPooler.GetPooledGameObjectOfType(objectName);

        if (nextGameObject == null) { return; }
        if (nextGameObject.GetComponent<PoolableObject>() == null)
        {
            throw new Exception($"{this.GetType()}.SpawnAtPosition: {this.gameObject.name} is attempting to spawn an object {nextGameObject.name} that does not have a PoolableObject component.");
        }

        nextGameObject.SetActive(true);
        nextGameObject.GetComponent<PoolableObject>().TriggerOnSpawnComplete();

        Health objectHealth = nextGameObject.GetComponent<Health>();
        if (objectHealth != null)
        {
            objectHealth.Revive();
        }

        nextGameObject.transform.position = positionToSpawn;
    }
}
