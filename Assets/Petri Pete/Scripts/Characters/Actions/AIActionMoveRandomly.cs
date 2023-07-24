using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class AIActionMoveRandomly : AIAction
{
    public float MaximumDurationInADirection = 2f;
    public LayerMask ObstacleLayerMask;
    public float ObstaclesDetectionDistance = 1f;
    public float ObstaclesCheckFrequency = 1;
    public Vector2 MinimumRandomDirection = new Vector2(-1f, -1f);
    public Vector2 MaximumRandomDirection = new Vector2(1f, 1f);

    protected CharacterMovement _characterMovement;
    protected Vector2 _direction;
    protected Collider2D _collider;
    protected float _lastDirectionChangeTimestamp = 0f;
    protected float _lastObstacleDetectionTimestamp = 0f;

    protected override void Initialization()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _collider = GetComponent<Collider2D>();
        PickRandomDirection();
    }

    protected virtual void PickRandomDirection()
    {
        _direction.x += UnityEngine.Random.Range(MinimumRandomDirection.x, MaximumRandomDirection.x);
        _direction.y += UnityEngine.Random.Range(MinimumRandomDirection.y, MaximumRandomDirection.y);
        _lastDirectionChangeTimestamp = Time.time;
    }

    public override void PerformAction()
    {
        CheckForObstacles();
        CheckForDuration();
        Move();
    }

    protected virtual void CheckForObstacles()
    {
        // Check to see if enough time has passed since last check
        if (Time.time - _lastObstacleDetectionTimestamp > ObstaclesCheckFrequency) { return; }

        // If there is an obstacle in the direction we want to move, get a new direction
        RaycastHit2D hit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, _direction.normalized, 2, ObstacleLayerMask);
        if (hit)
        {
            PickRandomDirection();
        }

        _lastObstacleDetectionTimestamp = Time.time;
    }

    protected virtual void CheckForDuration()
    {
        if (Time.time - _lastDirectionChangeTimestamp < MaximumDurationInADirection) { return; }

        PickRandomDirection();
    }

    protected virtual void Move()
    {
        _characterMovement.SetMovement(_direction);
    }

    public override void OnExitState()
    {
        base.OnExitState();

        _characterMovement.SetMovement(Vector2.zero);
    }

    protected virtual void OnDrawGizmos()
    {
        if (_collider != null)
        {
            JP_Debug.DebugBoxCast(_collider.bounds.center, _collider.bounds.size, _direction.normalized, 2, ObstacleLayerMask);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere((Vector2)transform.position + _direction.normalized, 0.1f);
    }
}
