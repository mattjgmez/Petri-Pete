using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class VirusDebuff : Debuff
{
    [Header("Number of Viruses")]
    public int MinimumSpawn = 5;
    public int MaximumSpawn = 5;

    public int DamagePerTick = 2;
    public GameObject VirusPrefab;

    protected Health _targetHealth;

    public VirusDebuff(VirusDebuff debuff, Character targetCharacter)
    : base(debuff, targetCharacter)
    {
        Duration = debuff.Duration;
        Stackable = debuff.Stackable;
        TargetCharacter = targetCharacter;

        MinimumSpawn = debuff.MinimumSpawn;
        MaximumSpawn = debuff.MaximumSpawn;

        DamagePerTick = debuff.DamagePerTick;
        VirusPrefab = debuff.VirusPrefab;

        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        _targetHealth = TargetCharacter.Health;

        _targetHealth.OnDeath += SpawnViruses;
    }

    public override void ProcessDebuff()
    {
        _targetHealth.Damage(DamagePerTick, this.gameObject, 0);
    }

    protected override void OnDeactivated()
    {
        _targetHealth.OnDeath -= SpawnViruses;
    }

    protected virtual void SpawnViruses()
    {
        for (int i = 0; i < Random.Range(MinimumSpawn, MaximumSpawn); i++)
        {
            Instantiate(VirusPrefab, (Vector2)TargetCharacter.transform.position, Quaternion.identity);
        }
    }
}
