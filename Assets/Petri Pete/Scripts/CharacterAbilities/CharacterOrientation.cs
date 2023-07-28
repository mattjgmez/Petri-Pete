using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// This script provides functionality to orient a character based on mouse position, with options for horizontal flipping and rotation.
/// </summary>
public class CharacterOrientation : CharacterAbility
{
    /// <summary>
    /// Enum representing the different modes of character facing.
    /// </summary>
    public enum FacingModes { None, MovementDirection, WeaponDirection, Both }

    [Header("Orientation Settings")]
    [Tooltip("The mode determining how the character should face.")]
    public FacingModes FacingMode = FacingModes.None;

    private SpriteRenderer _spriteRenderer;

    [Header("Horizontal Flip")]
    [Tooltip("Should the model flip horizontally based on direction?")]
    public bool ModelShouldFlip = false;

    [Tooltip("Scale value applied to the model when facing left.")]
    public Vector3 ModelFlipValueLeft = new Vector3(-1, 1, 1);

    [Tooltip("Scale value applied to the model when facing right.")]
    public Vector3 ModelFlipValueRight = new Vector3(1, 1, 1);

    [Tooltip("Should the model rotate based on direction?")]
    public bool ModelShouldRotate;

    [Tooltip("Rotation value applied to the model when facing left.")]
    public Vector3 ModelRotationValueLeft = new Vector3(0f, 180f, 0f);

    [Tooltip("Rotation value applied to the model when facing right.")]
    public Vector3 ModelRotationValueRight = new Vector3(0f, 0f, 0f);

    [Tooltip("Speed at which the model rotates when changing direction. Set to 0 for instant rotation.")]
    public float ModelRotationSpeed = 0f;

    [Tooltip("The threshold at which movement is considered.")]
    public float AbsoluteThresholdMovement = 0.5f;

    [Tooltip("The threshold at which weapon gets considered.")]
    public float AbsoluteThresholdWeapon = 0.5f;

    /// whether or not this character is facing right
    public bool IsFacingRight = true;

    protected Vector3 _targetModelRotation;
    protected CharacterWeaponHandler _characterWeaponHandler;
    protected Vector3 _lastRegisteredVelocity;
    protected Vector3 _rotationDirection;
    protected Vector3 _lastMovement = Vector3.zero;
    protected Vector3 _lastAim = Vector3.zero;
    protected float _lastNonNullXMovement;
    protected int _direction;
    protected int _directionLastFrame = 0;
    protected float _horizontalDirection;
    protected float _verticalDirection;

    /// <summary>
    /// On awake we init our facing direction and grab components
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        IsFacingRight = true;
        _direction = 1;
        _directionLastFrame = 0;
        _characterWeaponHandler = _character.GetAbility<CharacterWeaponHandler>();
    }

    /// <summary>
    /// On process ability, we flip to face the direction set in settings
    /// </summary>
    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            return;
        }

        DetermineFacingDirection();
        FlipToFaceMovementDirection();
        FlipToFaceWeaponDirection();
        ApplyModelRotation();
        FlipAbilities();

        _directionLastFrame = _direction;
        _lastNonNullXMovement = (Mathf.Abs(_controller.CurrentDirection.x) > 0) ? _controller.CurrentDirection.x : _lastNonNullXMovement;
    }

    protected virtual void DetermineFacingDirection()
    {
        _horizontalDirection = Mathf.Abs(_controller.CurrentDirection.x) >= AbsoluteThresholdMovement ? _controller.CurrentDirection.x : 0f;
        _verticalDirection = Mathf.Abs(_controller.CurrentDirection.y) >= AbsoluteThresholdMovement ? _controller.CurrentDirection.y : 0f;
    }

    /// <summary>
    /// If the model should rotate, we modify its rotation 
    /// </summary>
    protected virtual void ApplyModelRotation()
    {
        if (!ModelShouldRotate)
        {
            return;
        }

        if (ModelRotationSpeed > 0f)
        {
            _character.CharacterModel.transform.localEulerAngles = Vector3.Lerp(_character.CharacterModel.transform.localEulerAngles, _targetModelRotation, Time.deltaTime * ModelRotationSpeed);
        }
        else
        {
            _character.CharacterModel.transform.localEulerAngles = _targetModelRotation;
        }
    }

    /// <summary>
    /// Flips the object to face direction
    /// </summary>
    protected virtual void FlipToFaceMovementDirection()
    {
        // if we're not supposed to face our direction, we do nothing and exit
        if ((FacingMode != FacingModes.MovementDirection) && (FacingMode != FacingModes.Both)) { return; }

        if (_controller.CurrentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
        {
            float checkedDirection = (Mathf.Abs(_controller.CurrentDirection.normalized.x) > 0) ? _controller.CurrentDirection.normalized.x : _lastNonNullXMovement;

            if (checkedDirection >= 0)
            {
                FaceDirection(1);
            }
            else
            {
                FaceDirection(-1);
            }
        }
    }

    /// <summary>
    /// Flips the character to face the current weapon direction
    /// </summary>
    protected virtual void FlipToFaceWeaponDirection()
    {
        if (_characterWeaponHandler == null)
        {
            return;
        }
        // if we're not supposed to face our direction, we do nothing and exit
        if ((FacingMode != FacingModes.WeaponDirection) && (FacingMode != FacingModes.Both)) { return; }

        Debug.Log($"{this.GetType()}.FlipToFaceWeaponDirection: WeaponAimComponent = {_characterWeaponHandler.WeaponAimComponent}.", gameObject);

        if (_characterWeaponHandler.WeaponAimComponent != null)
        {
            float weaponAngle = _characterWeaponHandler.WeaponAimComponent.CurrentAngleAbsolute;

            if ((weaponAngle > 90) || (weaponAngle < -90))
            {
                FaceDirection(-1);
            }
            else
            {
                FaceDirection(1);
            }
        }
    }

    /// <summary>
    /// Flips the character and its dependencies (jetpack for example) horizontally
    /// </summary>
    public virtual void FaceDirection(int direction)
    {
        if (ModelShouldFlip)
        {
            FlipModel(direction);
        }

        if (ModelShouldRotate)
        {
            RotateModel(direction);
        }

        _direction = direction;
        IsFacingRight = _direction == 1;
    }

    /// <summary>
    /// Rotates the model in the specified direction
    /// </summary>
    /// <param name="direction"></param>
    protected virtual void RotateModel(int direction)
    {
        if (_character.CharacterModel != null)
        {
            _targetModelRotation = (direction == 1) ? ModelRotationValueRight : ModelRotationValueLeft;
            _targetModelRotation.x = _targetModelRotation.x % 360;
            _targetModelRotation.y = _targetModelRotation.y % 360;
            _targetModelRotation.z = _targetModelRotation.z % 360;
        }
    }

    /// <summary>
    /// Flips the model only, no impact on weapons or attachments
    /// </summary>
    public virtual void FlipModel(int direction)
    {
        if (_character.CharacterModel != null)
        {
            _character.CharacterModel.transform.localScale = (direction == 1) ? ModelFlipValueRight : ModelFlipValueLeft;
        }
        else
        {
            _spriteRenderer.flipX = (direction == -1);
        }
    }

    /// <summary>
    /// Sends a flip event on all other abilities
    /// </summary>
    protected virtual void FlipAbilities()
    {
        if ((_directionLastFrame != 0) && (_directionLastFrame != _direction))
        {
            _character.FlipAllAbilities();
        }
    }
}
