using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidIron : Solid
{
    public List<LiquidTypes> RustingLiquidTypes;
    public List<GasTypes> RustingGasTypes;

    [Header("Rust Properties")]
    public Color RustColor;
    public float TimeToRust = 4f;

    protected SpriteRenderer _spriteRenderer;
    protected Timer _rustTimer;
    protected Color _initialColor;
    protected Health _health;

    protected override void Initialization()
    {
        _rustTimer = new Timer(TimeToRust, null, ActivateRusted);
        _spriteRenderer = Model.GetComponent<SpriteRenderer>();
        _initialColor = _spriteRenderer.color;
        _health = GetComponent<Health>();
        SolidType = SolidTypes.Iron;
    }

    protected virtual void Colliding(GameObject collision)
    {
        Liquid collidedLiquid = collision.GetComponent<Liquid>();
        Gas collidedGas = collision.GetComponent<Gas>();

        // We check to see if either the Liquid or Gas we collide with is null, or not a rusting type.
        if ((collidedLiquid == null
        || !RustingLiquidTypes.Contains(collidedLiquid.LiquidType))
        && (collidedGas == null
        || !RustingGasTypes.Contains(collidedGas.GasType))) 
        {
            return; 
        }

        _rustTimer.StartTimer();
        float lerpValue = _rustTimer.ElapsedTime / _rustTimer.Duration;

        _spriteRenderer.color = Color.Lerp(_initialColor, RustColor, lerpValue);
    }

    protected virtual void ActivateRusted()
    {
        _spriteRenderer.color = RustColor;
        _health.Invulnerable = false;
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
