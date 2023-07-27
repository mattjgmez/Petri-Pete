using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidAcid : Liquid
{
    public float ExpansionRate = 1.0f;
    public ParticleSystem ParticleSystem;
    public Vector2 ParticleScaleCompensation;

    protected ParticleSystem.ShapeModule _particleShape;
    protected SpriteRenderer _modelSpriteRenderer;

    protected override void Initialization()
    {
        LiquidType = LiquidTypes.Acid;
        ParticleSystem = Model.GetComponent<ParticleSystem>();
        _modelSpriteRenderer = Model.GetComponent<SpriteRenderer>();
        _particleShape = ParticleSystem.shape;
    }

    protected override void UpdateScale()
    {
        base.UpdateScale();
        //Vector3 spriteScale = _modelSpriteRenderer.transform.lossyScale;
        //Vector3 spriteBoundsSize = _modelSpriteRenderer.sprite.bounds.size * 1.16f;
        //Vector3 spriteWorldSize = new Vector3((spriteScale.x * spriteBoundsSize.x) + ParticleScaleCompensation.x, 
        //                                      (spriteScale.y * spriteBoundsSize.y) + ParticleScaleCompensation.y, 
        //                                       spriteScale.z * spriteBoundsSize.z);

        //_particleShape.scale = spriteWorldSize;
    }

    protected virtual void Update()
    {
        RaiseCharge(Time.deltaTime * ExpansionRate);
    }
}
