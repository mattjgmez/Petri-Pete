using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    /// the possible character types : player _characterController or AI (controlled by the computer)
    public enum CharacterTypes { Player, AI }

    #region VARIABLES

    [Header("Character Configuration")]
    [Tooltip("Is the character player-controlled or controlled by an AI?")]
    public CharacterTypes CharacterType = CharacterTypes.AI;
    [Tooltip("Used if the character is player controlled. The PlayerID must match the Input Manager's PlayerID.")]
    public string PlayerID = "";

    [Header("Model")]
    [Tooltip("3D Model representation of the character.")]
    public GameObject CharacterModel;

    [Header("State Configuration")]
    [Tooltip("StateMachine for controlling character's movement states.")]
    public StateMachine<CharacterStates.MovementStates> MovementState;
    [Tooltip("StateMachine for controlling character's condition states.")]
    public StateMachine<CharacterStates.CharacterConditions> ConditionState;

    [Header("Events")]
    [Tooltip("Send events when the state changes.")]
    public bool SendStateChangeEvents = true;
    [Tooltip("Send events for state updates.")]
    public bool SendStateUpdateEvents = true;

    [Header("Animation")]
    [Tooltip("Assign this manually if the relevant animator is nested somewhere.")]
    public Animator CharacterAnimator;
    [Tooltip("Reference to the input manager linked with this character.")]
    public InputManager LinkedInputManager;
    [Tooltip("Target object for the camera.")]
    public GameObject CameraTarget;

    public CharacterStates CharacterState { get; protected set; }

    public Health Health { get { return _health; } }

    public TopDownController TopDownController { get { return _controller; } }

    public Animator Animator { get; protected set; }

    public List<int> AnimatorParameters { get; set; }

    // Protected Variables Group
    protected List<CharacterAbility> _characterAbilities;
    protected Health _health;
    protected TopDownController _controller;
    protected AIBrain _aiBrain;
    protected const string _idleAnimationParameterName = "Idle";
    protected int _idleAnimationParameter;
    protected bool _animatorInitialized = false;

    #endregion

    #region UNITY LIFECYCLE

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        Initialization();
    }

    /// <summary>
    /// This is called every frame.
    /// </summary>
    protected virtual void Update()
    {
        EveryFrame();
    }

    #endregion

    /// <summary>
    /// Initializes the state machines, components, and other essential data.
    /// </summary>
    protected virtual void Initialization()
    {
        MovementState = new StateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new StateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);
        SetInputManager();
        _characterAbilities = new List<CharacterAbility>(GetComponents<CharacterAbility>());
        _controller = GetComponent<TopDownController>();
        _health = GetComponent<Health>();
        _aiBrain = GetComponent<AIBrain>();

        Transform model = transform.Find("Model");
        if (model != null)
        {
            CharacterModel = model.gameObject;
        }

        Transform cameraTarget = transform.Find("CameraTarget");
        if (cameraTarget != null)
        {
            CameraTarget = cameraTarget.gameObject;
        }

        AssignAnimator();
    }

    /// <summary>
    /// Initializes the animation parameters for the character's animator.
    /// </summary>
    protected virtual void InitializeAnimatorParameters()
    {
        if (Animator == null) { return; }
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, _idleAnimationParameterName, out _idleAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
    }

    /// <summary>
    /// Updates the input manager for all character abilities.
    /// </summary>
    protected virtual void UpdateAbilitiesInputManagers()
    {
        if (_characterAbilities == null)
        {
            return;
        }
        for (int i = 0; i < _characterAbilities.Count; i++)
        {
            _characterAbilities[i].SetInputManager(LinkedInputManager);
        }
    }

    /// <summary>
    /// Processes character's abilities and updates the animator.
    /// </summary>
    protected virtual void EveryFrame()
    {
        EarlyProcessAbilities();
        ProcessAbilities();
        LateProcessAbilities();
        UpdateAnimators();
    }

    #region SET AND ASSIGN METHODS

    /// <summary>
    /// Determines the input manager for the character, depending on if it's player or AI controlled.
    /// </summary>
    public virtual void SetInputManager()
    {
        if (CharacterType == CharacterTypes.AI)
        {
            LinkedInputManager = null;
            UpdateAbilitiesInputManagers();
            return;
        }

        if (!string.IsNullOrEmpty(PlayerID))
        {
            LinkedInputManager = null;
            InputManager[] foundInputManagers = FindObjectsOfType(typeof(InputManager)) as InputManager[];
            foreach (InputManager foundInputManager in foundInputManagers)
            {
                if (foundInputManager.PlayerID == PlayerID)
                {
                    LinkedInputManager = foundInputManager;
                }
            }
        }
        UpdateAbilitiesInputManagers();
    }

    /// <summary>
    /// Sets the input manager for the character to the provided input manager.
    /// </summary>
    /// <param name="inputManager">The input manager to assign to the character.</param>
    public virtual void SetInputManager(InputManager inputManager)
    {
        LinkedInputManager = inputManager;
        UpdateAbilitiesInputManagers();
    }

    /// <summary>
    /// Assigns the character's animator and initializes its parameters.
    /// </summary>
    public virtual void AssignAnimator()
    {
        if (_animatorInitialized)
        {
            return;
        }
        AnimatorParameters = new List<int>();

        if (CharacterAnimator != null)
        {
            Animator = CharacterAnimator;
        }
        else
        {
            Animator = GetComponent<Animator>();
        }

        if (Animator != null)
        {
            InitializeAnimatorParameters();
        }

        _animatorInitialized = true;
    }

    #endregion

    #region ABILITIES AND ANIMATOR METHODS

    /// <summary>
    /// Calls the early processing methods for all enabled abilities.
    /// </summary>
    protected virtual void EarlyProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.EarlyProcessAbility();
            }
        }
    }

    /// <summary>
    /// Calls the main processing methods for all enabled abilities.
    /// </summary>
    protected virtual void ProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.ProcessAbility();
            }
        }
    }

    /// <summary>
    /// Calls the late processing methods for all enabled abilities.
    /// </summary>
    protected virtual void LateProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.LateProcessAbility();
            }
        }
    }

    /// <summary>
    /// Updates the animator's parameters and calls the update method for all enabled abilities.
    /// </summary>
    protected virtual void UpdateAnimators()
    {
        if (Animator != null)
        {
            AnimatorExtensions.UpdateAnimatorBool(Animator, _idleAnimationParameter, (MovementState.CurrentState == CharacterStates.MovementStates.Idle), AnimatorParameters);
            foreach (CharacterAbility ability in _characterAbilities)
            {
                if (ability.enabled && ability.AbilityInitialized)
                {
                    ability.UpdateAnimator();
                }
            }
        }
    }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Enables or disables the character.
    /// </summary>
    /// <param name="state">Whether to enable (true) or disable (false) the character.</param>
    public virtual void SetEnable(bool state)
    {
        this.enabled = state;
        _controller.enabled = state;
    }

    /// <summary>
    /// Retrieves a specific ability from the character's list of abilities.
    /// </summary>
    /// <typeparam name="T">The type of the ability to retrieve.</typeparam>
    /// <returns>The requested ability, or null if not found.</returns>
    public virtual T GetAbility<T>() where T : CharacterAbility
    {
        return _characterAbilities.FirstOrDefault(ability => ability is T) as T;
    }

    /// <summary>
    /// Resets the character and all its abilities.
    /// </summary>
    public virtual void Reset()
    {
        if (_characterAbilities == null || _characterAbilities.Count == 0)
        {
            return;
        }
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled)
            {
                ability.ResetAbility();
            }
        }
    }

    #endregion
}
