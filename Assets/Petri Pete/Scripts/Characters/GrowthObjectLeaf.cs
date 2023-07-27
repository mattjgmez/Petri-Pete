using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthObjectLeaf : GrowthObject
{
    protected CharacterDrink _hubDrink;

    public override void SetGrowthHub(Character growthHub)
    {
        base.SetGrowthHub(growthHub);
        _hubDrink = growthHub.GetAbility<CharacterDrink>();
    }

    protected virtual void Colliding(GameObject collision)
    {
        if (_hubDrink.Condition.CurrentState != CharacterStates.CharacterConditions.Normal) { return; }

        Liquid hitLiquid = collision.transform.GetComponent<Liquid>();

        if (hitLiquid)
        {
            _hubDrink.SetLiquid(hitLiquid);
        }
        // If the character is drinking, don't nullify the target liquid.
        else if (_hubDrink.Movement.CurrentState != CharacterStates.MovementStates.Drinking)
        {
            _hubDrink.SetLiquid(null);
        }

        if (hitLiquid == null) { return; }

        AttemptDrink(hitLiquid);
    }

    protected virtual void AttemptDrink(Liquid liquid)
    {
        if (_hubDrink.Condition.CurrentState != CharacterStates.CharacterConditions.Normal) { return; }
        if (!_hubDrink.DrinkableLiquids.Contains(liquid.LiquidType)) { return; }

        _hubDrink.StartDrink();
    }

    #region COLLISION METHODS

    /// <summary>
    /// On trigger stay 2D, we call our colliding endpoint.
    /// </summary>
    /// <param name="collider">what's colliding with the object.</param>
    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger enter 2D, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>S
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger stay, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    public virtual void OnTriggerStay(Collider collider)
    {
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger enter, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    public virtual void OnTriggerEnter(Collider collider)
    {
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On collision stay 2D, we call our colliding endpoint.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        Colliding(collision.gameObject);
    }

    /// <summary>
    /// On collision enter 2D, we call our colliding endpoint.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Colliding(collision.gameObject);
    }

    /// <summary>
    /// On collision stay, we call our colliding endpoint.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    public virtual void OnCollisionStay(Collision collision)
    {
        Colliding(collision.gameObject);
    }

    /// <summary>
    /// On collision enter, we call our colliding endpoint.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    public virtual void OnCollisionEnter(Collision collision)
    {
        Colliding(collision.gameObject);
    }

    #endregion
}
