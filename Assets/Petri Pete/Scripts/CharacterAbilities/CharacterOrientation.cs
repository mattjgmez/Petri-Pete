using UnityEngine;

/// <summary>
/// This script provides functionality to orient a character based on mouse position, with options for horizontal flipping and rotation.
/// </summary>
public class CharacterOrientation : MonoBehaviour
{
    /// <summary>
    /// Enum representing the different modes of character facing.
    /// </summary>
    public enum FacingModes { None, MousePosition }

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

    private Vector3 _mousePosition;
    private int _direction;
    private Vector3 _targetModelRotation;

    /// <summary>
    /// Initialization method to get required components.
    /// </summary>
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Updates the character's orientation every frame based on the selected facing mode.
    /// </summary>
    private void Update()
    {
        if (FacingMode == FacingModes.MousePosition)
        {
            FlipCharacterBasedOnMousePosition();
        }
    }

    /// <summary>
    /// Flips the character based on the current mouse position.
    /// </summary>
    protected virtual void FlipCharacterBasedOnMousePosition()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_mousePosition.x < transform.position.x)
        {
            FaceDirection(-1);
        }
        else if (_mousePosition.x > transform.position.x)
        {
            FaceDirection(1);
        }
    }

    /// <summary>
    /// Determines the orientation of the character based on the provided direction.
    /// </summary>
    /// <param name="direction">-1 for left, 1 for right.</param>
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
    }

    /// <summary>
    /// Rotates the character model based on the provided direction.
    /// </summary>
    /// <param name="direction">-1 for left, 1 for right.</param>
    protected virtual void RotateModel(int direction)
    {
        _targetModelRotation = (direction == 1) ? ModelRotationValueRight : ModelRotationValueLeft;

        if (ModelRotationSpeed > 0f)
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, _targetModelRotation, Time.deltaTime * ModelRotationSpeed);
        }
        else
        {
            transform.localEulerAngles = _targetModelRotation;
        }
    }

    /// <summary>
    /// Flips the character model based on the provided direction.
    /// </summary>
    /// <param name="direction">-1 for left, 1 for right.</param>
    public virtual void FlipModel(int direction)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = (direction == -1);
        }
        else
        {
            transform.localScale = (direction == 1) ? ModelFlipValueRight : ModelFlipValueLeft;
        }
    }
}
