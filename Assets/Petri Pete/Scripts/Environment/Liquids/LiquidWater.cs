using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidWater : Liquid
{
    protected bool _isMerging = false;
    protected List<LiquidWater> _collidingWaters = new List<LiquidWater>();
    protected bool _collisionProcessedThisFrame = false;

    protected override void Initialization()
    {
        LiquidType = LiquidTypes.Water;
    }

    protected virtual void Colliding(GameObject collision)
    {
        if (_isMerging || _collisionProcessedThisFrame) return;

        LiquidWater collidedWater = collision.GetComponent<LiquidWater>();
        if (collidedWater == null || collidedWater._collisionProcessedThisFrame) { return; }

        _collisionProcessedThisFrame = true;
        collidedWater._collisionProcessedThisFrame = true;

        if (!_collidingWaters.Contains(collidedWater))
        {
            _collidingWaters.Add(collidedWater);
            StartCoroutine(ProcessCollisionsEndOfFrame());
        }
    }

    protected virtual IEnumerator ProcessCollisionsEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        float totalCharge = Charge;

        foreach (LiquidWater water in _collidingWaters)
        {
            totalCharge += water.Charge;
            water.DestroyObject();
        }

        SetCharge((int)totalCharge);
        _collidingWaters.Clear();
        _isMerging = false;
        _collisionProcessedThisFrame = false; // Reset the flag for the next frame
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

    #endregion
}
