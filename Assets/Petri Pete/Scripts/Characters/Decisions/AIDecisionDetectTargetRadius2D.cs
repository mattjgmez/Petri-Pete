using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDecisionDetectTargetRadius2D : AIDecision
{
    public float Radius = 3f;
    public Vector3 DetectionOffset = Vector3.zero;
    public LayerMask TargetLayer;
    public bool ObstacleDetecion = true;
    public LayerMask ObstacleMask;

    protected Collider2D _collider;
    protected Vector2 _facingDirection;
    protected Vector2 _raycastOrigin;
    protected Character _character;
    protected SpriteRenderer _spriteRenderer;
    protected Collider2D _detectionCollider = null;
    protected Color _gizmoColor = Color.yellow;
    protected bool _init = false;
    protected Vector2 _boxcastDirection;

    /// <summary>
    /// On init we grab our Character component
    /// </summary>
    public override void Initialization()
    {
        _character = this.gameObject.GetComponent<Character>();
        _spriteRenderer = _character.CharacterModel.AddComponent<SpriteRenderer>();
        _collider = this.gameObject.GetComponent<Collider2D>();
        _gizmoColor.a = 0.25f;
        _init = true;
    }

    /// <summary>
    /// On Decide we check for our target
    /// </summary>
    /// <returns></returns>
    public override bool Decide()
    {
        return DetectTarget();
    }

    /// <summary>
    /// Returns true if a target is found within the circle
    /// </summary>
    /// <returns></returns>
    protected virtual bool DetectTarget()
    {
        _detectionCollider = null;

        if (_spriteRenderer != null)
        {
            _facingDirection = _spriteRenderer.flipX ? Vector2.left : Vector2.right;
            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOffset.y;
        }
        else
        {
            _raycastOrigin = transform.position + DetectionOffset;
        }

        // we cast a ray to the left of the agent to check for a Player

        _detectionCollider = Physics2D.OverlapCircle(_raycastOrigin, Radius, TargetLayer);
        if (_detectionCollider == null)
        {
            return false;
        }
        else
        {
            // we cast a ray to make sure there's no obstacle
            _boxcastDirection = (Vector2)(_detectionCollider.gameObject.GetComponent<Collider2D>().bounds.center - _collider.bounds.center);
            RaycastHit2D hit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, _boxcastDirection.normalized, _boxcastDirection.magnitude, ObstacleMask);
            if (!hit)
            {
                _brain.Target = _detectionCollider.gameObject.transform;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Draws gizmos for the detection circle
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOffset.x / 2;
        _raycastOrigin.y = transform.position.y + DetectionOffset.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_raycastOrigin, Radius);
        if (_init)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(_raycastOrigin, Radius);
        }
    }
}
