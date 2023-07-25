using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class TopDownController : MonoBehaviour
{
    #region PUBLIC VARIABLES

    public Vector3 Speed;
    public Vector3 Velocity;
    public Vector3 VelocityLastFrame;
    public Vector3 Acceleration;
    public Vector3 CurrentMovement;
    public Vector3 CurrentDirection;
    public float Friction;
    public Vector3 AddedForce;

    [Header("Layer Masks")]
    public LayerMask ObstaclesLayerMask;

    #endregion

    protected Vector3 _positionLastFrame;
    protected Vector3 _impact;
    protected Rigidbody2D _rigidBody;
    protected BoxCollider2D _collider;
    protected Vector2 _originalColliderSize;
    protected Vector3 _originalColliderCenter;
    protected Vector3 _orientedMovement;
    protected SpriteRenderer _spriteRenderer;

    protected virtual void Awake()
    {
        CurrentDirection = transform.forward;

        Initialization();
    }

    protected virtual void Initialization()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _originalColliderSize = _collider.size;
        _originalColliderCenter = _collider.offset; 

        _spriteRenderer = GetComponent<Character>().CharacterModel.GetComponent<SpriteRenderer>();
    }

    #region UPDATE METHODS

    protected virtual void Update()
    {
        DetermineDirection();
        Velocity = _rigidBody.velocity;
        Acceleration = (_rigidBody.velocity - (Vector2)VelocityLastFrame) / Time.fixedDeltaTime;
    }

    protected virtual void DetermineDirection()
    {
        if (CurrentMovement != Vector3.zero)
        {
            CurrentDirection = CurrentMovement.normalized;

            if (_spriteRenderer != null && CurrentDirection.x != 0)
            {
                _spriteRenderer.flipX = CurrentDirection.x < 0;
            }
        }
    }

    protected virtual void LateUpdate()
    {
        ComputeSpeed();
        VelocityLastFrame = _rigidBody.velocity;
    }

    protected virtual void ComputeSpeed()
    {
        Speed = (this.transform.position - _positionLastFrame) / Time.deltaTime;
        // we round the speed to 2 decimals
        Speed.x = Mathf.Round(Speed.x * 100f) / 100f;
        Speed.y = Mathf.Round(Speed.y * 100f) / 100f;
        Speed.z = Mathf.Round(Speed.z * 100f) / 100f;
        _positionLastFrame = this.transform.position;
    }

    protected virtual void FixedUpdate()
    {
        ApplyImpact();

        if (Friction > 1)
        {
            CurrentMovement = CurrentMovement / Friction;
        }

        // if we have a low friction we lerp the speed accordingly
        if (Friction > 0 && Friction < 1)
        {
            CurrentMovement = Vector3.Lerp(Speed, CurrentMovement, Time.deltaTime * Friction);
        }

        Vector2 newMovement = _rigidBody.position + (Vector2)(CurrentMovement + AddedForce) * Time.fixedDeltaTime;

        _rigidBody.MovePosition(newMovement);
    }

    protected virtual void ApplyImpact()
    {
        if (_impact.magnitude > 0.2f)
        {
            _rigidBody.AddForce(_impact);
        }
        _impact = Vector3.Lerp(_impact, Vector3.zero, 5f * Time.deltaTime);
    }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Use this to apply an impact to a controller, moving it in the specified direction at the specified force
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public virtual void Impact(Vector3 direction, float force)
    {
        direction = direction.normalized;
        _impact += direction.normalized * force;
    }

    /// <summary>
    /// Sets the current movement
    /// </summary>
    /// <param name="movement"></param>
    public virtual void SetMovement(Vector3 movement)
    {
        _orientedMovement = movement;
        _orientedMovement.y = _orientedMovement.z;
        _orientedMovement.z = 0f;
        CurrentMovement = _orientedMovement;
    }

    /// <summary>
    /// Adds a force of the specified vector
    /// </summary>
    /// <param name="movement"></param>
    public virtual void AddForce(Vector3 movement)
    {
        Impact(movement.normalized, movement.magnitude);
    }

    /// <summary>
    /// Tries to move to the specified position
    /// </summary>
    /// <param name="newPosition"></param>
    public virtual void MovePosition(Vector3 newPosition)
    {
        _rigidBody.MovePosition(newPosition);
    }

    /// <summary>
    /// Resizes the collider to the new size set in parameters
    /// </summary>
    /// <param name="newSize">New size.</param>
    public virtual void ResizeCollider(float newHeight)
    {
        float newYOffset = _originalColliderCenter.y - (_originalColliderSize.y - newHeight) / 2;
        Vector2 newSize = _collider.size;
        newSize.y = newHeight;
        _collider.size = newSize;
        _collider.offset = newYOffset * Vector3.up;
    }

    /// <summary>
    /// Returns the collider to its initial size
    /// </summary>
    public virtual void ResetColliderSize()
    {
        _collider.size = _originalColliderSize;
        _collider.offset = _originalColliderCenter;
    }

    /// <summary>
    /// Sets this rigidbody as kinematic
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetKinematic(bool state)
    {
        _rigidBody.isKinematic = state;
    }

    /// <summary>
    /// Enables the collider
    /// </summary>
    public virtual void CollisionsOn()
    {
        _collider.enabled = true;
    }

    /// <summary>
    /// Disables the collider
    /// </summary>
    public virtual void CollisionsOff()
    {
        _collider.enabled = false;
    }

    /// <summary>
    /// Resets all values for this controller
    /// </summary>
    public virtual void Reset()
    {
        _impact = Vector3.zero;
        Speed = Vector3.zero;
        Velocity = Vector3.zero;
        VelocityLastFrame = Vector3.zero;
        Acceleration = Vector3.zero;
        CurrentMovement = Vector3.zero;
        CurrentDirection = Vector3.zero;
        AddedForce = Vector3.zero;
    }

    #endregion
}
