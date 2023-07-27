using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for Solids, Liquids, and Gases.
/// Used to store universal behaviors between the 3 types.
/// </summary>
[RequireComponent(typeof(PoolableObject))]
public abstract class EnvironmentBehavior : MonoBehaviour
{
    /// Enums to track type properties across different states.
    public enum GasTypes { Oxygen, CarbonDioxide, }
    public enum LiquidTypes { Water, Acid, }
    public enum SolidTypes { Iron }

    public GameObject Model;

    protected PoolableObject _poolableObject;

    /// <summary>
    /// Used to Initialize the Type enum variable.
    /// </summary>
    protected abstract void Initialization();

    protected virtual void Start()
    {
        Initialization();
        _poolableObject = GetComponent<PoolableObject>();
    }

    public virtual void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
