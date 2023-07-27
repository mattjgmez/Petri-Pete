using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region VARIABLES

    /// the possible character types : player _characterController or AI (controlled by the computer)
    public enum CharacterTypes { Player, AI }
    /// Is the character player-controlled or controlled by an AI ?
    public CharacterTypes CharacterType = CharacterTypes.AI;
    /// Used if the character is player controller. The PlayerID must match the Input Manager's PlayerID.
    public string PlayerID = "";

    public CharacterStates CharacterState { get; protected set; }
    public Health Health { get {  return _health; } }
    public TopDownController TopDownController { get { return _controller; } }

    [Header("Model")]
    public GameObject CharacterModel;

    /// State Machines
    public StateMachine<CharacterStates.MovementStates> MovementState;
    public StateMachine<CharacterStates.CharacterConditions> ConditionState;

    [Header("Events")]
    public bool SendStateChangeEvents = true;
    public bool SendStateUpdateEvents = true;

    [Header("Animator"), Tooltip("Assign this manually if the relevant animator is nested somewhere.")]
    public Animator CharacterAnimator;
    public InputManager LinkedInputManager;
    public Animator Animator { get; protected set; }
    public List<int> AnimatorParameters { get; set; }

    public GameObject CameraTarget;

    protected List<CharacterAbility> _characterAbilities;
    protected Health _health;
    protected TopDownController _controller;
    protected AIBrain _aiBrain;

    protected const string _idleAnimationParameterName = "Idle";
    protected int _idleAnimationParameter;
    protected bool _animatorInitialized = false;

    #endregion

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        // Initialize state machines
        MovementState = new StateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new StateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);

        // Get the current input manager
        SetInputManager();

        // Initialize components
        _characterAbilities = new List<CharacterAbility>(GetComponents<CharacterAbility>());
        _controller = GetComponent<TopDownController>();
        _health = GetComponent<Health>();
        _aiBrain = GetComponent<AIBrain>();

        AssignAnimator();
    }

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

    public virtual void SetInputManager(InputManager inputManager)
    {
        LinkedInputManager = inputManager;
        UpdateAbilitiesInputManagers();
    }

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

    protected virtual void InitializeAnimatorParameters()
    {
        if (Animator == null) { return; }
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, _idleAnimationParameterName, out _idleAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
    }

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
    /// This is called every frame.
    /// </summary>
    protected virtual void Update()
    {
        EveryFrame();
    }

    /// <summary>
    /// We do this every frame. This is separate from Update for more flexibility.
    /// </summary>
    protected virtual void EveryFrame()
    {
        // we process our abilities
        EarlyProcessAbilities();
        ProcessAbilities();
        LateProcessAbilities();

        UpdateAnimators();
    }

    /// <summary>
    /// Calls all registered abilities' Early Process methods
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
    /// Calls all registered abilities' Process methods
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
    /// Calls all registered abilities' Late Process methods
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

    #region PUBLIC METHODS

    /// <summary>
    /// Called to toggle the player (at the end of a level for example. 
    /// It won't move and respond to input after this.
    /// </summary>
    public virtual void SetEnable(bool state)
    {
        this.enabled = state;
        _controller.enabled = state;
    }

    /// <summary>
    /// Retrieves the given CharacterAbility from the CharacterAbilities List.
    /// </summary>
    /// <typeparam name="T">The ability type to get.</typeparam>
    public virtual T GetAbility<T>() where T : CharacterAbility
    {
        return _characterAbilities.FirstOrDefault(ability => ability is T) as T;
    }

    /// <summary>
    /// Called when the Character dies. 
    /// Calls every abilities' Reset() method, so you can restore settings to their original value if needed
    /// </summary>
    public virtual void Reset()
    {
        if (_characterAbilities == null)
        {
            return;
        }
        if (_characterAbilities.Count == 0)
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
