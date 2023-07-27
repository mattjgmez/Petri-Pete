using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New DebuffSpawnOnKill", menuName = "ScriptableObjects/Debuff/SpawnOnKill", order = 1)]
public class DebuffSpawnOnKill : Debuff, ICloneable<DebuffSpawnOnKill>
{
    [Header("SpawnOnKill")]
    public int MinimumSpawn = 5;
    public int MaximumSpawn = 5;
    public Vector2 MinimumSpawnOffset = new Vector2(-1, -1);
    public Vector2 MaximumSpawnOffset = new Vector2(1, 1);

    public int DamagePerTick = 2;
    public GameObject ObjectToSpawn;

    protected Health _targetHealth;

    public new DebuffSpawnOnKill Clone()
    {
        return Instantiate(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        _targetHealth = TargetCharacter.Health;

        _targetHealth.OnDeath += SpawnObjects;
    }

    protected override void ProcessDebuff()
    {
        _targetHealth.Damage(DamagePerTick, null, 0);
    }

    protected override void OnDeactivated()
    {
        _targetHealth.OnDeath -= SpawnObjects;
    }

    protected virtual void SpawnObjects()
    {
        RemoveDebuff();

        for (int i = 0; i < Random.Range(MinimumSpawn, MaximumSpawn); i++)
        {
            Vector2 randomOffset = new Vector2(Random.Range(MinimumSpawnOffset.x, MaximumSpawnOffset.x),
                                               Random.Range(MinimumSpawnOffset.y, MaximumSpawnOffset.y));

            SpawnManager.Instance.SpawnAtPosition((Vector2)TargetCharacter.transform.position + randomOffset, ObjectToSpawn.name);
        }
    }
}
