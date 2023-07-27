using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Liquid : EnvironmentBehavior
{
    public LiquidTypes LiquidType { get; protected set; }

    public float Charge = 1f;

    protected override void Start()
    {
        Initialization();
        _poolableObject = GetComponent<PoolableObject>();
        UpdateScale();
    }

    /// <summary>
    /// Used for applying possible effects from special liquids like acid
    /// </summary>
    public virtual void ProcessDrinkEffect() { }

    public virtual void SetCharge(int amount)
    {
        //Debug.Log($"{this.GetType()}.SetCharge: Setting charge to {amount}.", gameObject);

        Charge = amount;
        if (Charge <= 0)
        {
            Charge = 0;
            Destroy(this.gameObject);
            return;
        }
        UpdateScale();
    }

    public virtual void RaiseCharge(float amount)
    {
        Charge += amount;
        UpdateScale();
    }

    public virtual void LowerCharge(float amount)
    {
        Charge -= amount;
        if (Charge <= 0)
        {
            Charge = 0;
            Destroy(this.gameObject);
            return;
        }
        UpdateScale();
    }

    protected virtual void UpdateScale()
    {
        transform.localScale = new Vector3(Charge, Charge, Charge);
    }
}
