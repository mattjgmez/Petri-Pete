using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDecisionPriorityDetectTargetRadius2D : AIDecision
{
    public float Radius = 3f;
    public Vector3 DetectionOffset = Vector3.zero;
    public LayerMask TargetLayer;
    public bool ObstacleDetection = true;
    public LayerMask ObstacleMask;

    [Tooltip("List of tags in descending order of priority. When multiple targets are detected, the AI will choose the target with the highest-priority tag.")]
    public List<string> TagPriorityList = new List<string>(); // Example: { "Enemy", "Player", "NPC" }

    protected List<Transform> detectedTargets = new List<Transform>();
    protected Collider2D _collider;
    protected Vector2 _facingDirection;
    protected Vector2 _raycastOrigin;
    protected Character _character;
    protected SpriteRenderer _spriteRenderer;
    protected Color _gizmoColor = Color.yellow;
    protected bool _init = false;
    protected Vector2 _boxcastDirection;

    public override void Initialization()
    {
        _character = this.gameObject.GetComponent<Character>();
        _spriteRenderer = _character.CharacterModel.GetComponent<SpriteRenderer>();
        _collider = this.gameObject.GetComponent<Collider2D>();
        _gizmoColor.a = 0.25f;
        _init = true;
    }

    public override bool Decide()
    {
        return DetectTarget();
    }

    protected virtual bool DetectTarget()
    {
        detectedTargets.Clear();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_raycastOrigin, Radius, TargetLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            _boxcastDirection = (Vector2)(hitCollider.bounds.center - _collider.bounds.center);
            RaycastHit2D hit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, _boxcastDirection.normalized, _boxcastDirection.magnitude, ObstacleMask);
            if (!hit)
            {
                detectedTargets.Add(hitCollider.transform);
            }
        }

        if (detectedTargets.Count > 0)
        {
            Transform highestPriorityTarget = null;

            foreach (string tag in TagPriorityList)
            {
                foreach (Transform target in detectedTargets)
                {
                    if (target.CompareTag(tag))
                    {
                        highestPriorityTarget = target;
                        break;
                    }
                }
                if (highestPriorityTarget != null)
                    break;
            }

            _brain.Target = highestPriorityTarget;
            return true;
        }

        return false;
    }

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
