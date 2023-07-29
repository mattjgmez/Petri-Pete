using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDrink : CharacterAbility
{
    [Header("CharacterDrink")]
    public List<EnvironmentBehavior.LiquidTypes> DrinkableLiquids;
    public int HealAmount = 5;
    public float DrinkDuration = 1;

    [Header("Liquid Detection")]
    public bool DetectionActive = true;
    public Vector2 CastOffset = Vector2.zero;
    public float CastRadius = 0.1f;
    public Vector2 CastDirection = Vector2.zero;
    public float CastDistance = 0f;
    public LayerMask CastLayerMask;

    protected Timer _drinkTimer;
    protected Liquid _targetLiquid;
    protected float _initialCharge;
    protected float _targetCharge;

    protected const string _drinkingAnimationParameterName = "Drinking";
    protected int _drinkingAnimationParameter;
    protected int _currentDirection = 1;

    protected override void Initialization()
    {
        base.Initialization();
        _drinkTimer = new Timer(DrinkDuration);
    }

    protected override void HandleInput()
    {
        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) { return; }
        if (_movement.CurrentState == CharacterStates.MovementStates.Drinking) { return; }

        if (Input.GetButtonDown(_inputManager.DrinkButton.ButtonID))
        {
            StartDrink();
        }
    }

    public virtual void StartDrink()
    {
        if (_targetLiquid == null) { return; }

        _movement.ChangeState(CharacterStates.MovementStates.Drinking);

        _initialCharge = _targetLiquid.Charge;
        _targetCharge = _initialCharge - 1;

        _drinkTimer.ResetTimer();
        _drinkTimer.StartTimer();

        _controller.FreeMovement = false;
    }

    public virtual void EndDrink()
    {
        _drinkTimer.StopTimer();

        _movement.ChangeState(CharacterStates.MovementStates.Idle);
        _controller.FreeMovement = true;

        if(_character.PlayerID == "Player")
        {
            UIManager.Instance.AddJournalEntryWithID("Drink");
        }
    }

    protected virtual void FinishDrink()
    {
        if (_character.Health != null)
        {
            _character.Health.Heal(HealAmount, this.gameObject);
        }

        _targetLiquid.ProcessDrinkEffect();
        EndDrink();
    }

    public override void ProcessAbility()
    {
        //Debug.Log($"{this.GetType()}.ProcessAbility: Character Movement State: {_movement.CurrentState}.", gameObject);

        _drinkTimer.UpdateTimer();

        FindLiquid();
        DrinkLiquid();
    }

    protected virtual void FindLiquid()
    {
        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) { return; }
        if (_controller == null) { return; }
        if (_controller.SpriteRenderer == null) { return; }
        if (!DetectionActive) { return; }

        Vector2 castOffsetCalculated = new(CastOffset.x * _currentDirection, CastOffset.y);
        Vector2 castDirectionCalculated = new(CastDirection.x * _currentDirection, CastDirection.y);

        RaycastHit2D hit = Physics2D.CircleCast((Vector2)transform.position + castOffsetCalculated,
                                                CastRadius,
                                                castDirectionCalculated,
                                                CastDistance,
                                                CastLayerMask);

        if (!hit)
        {
            // If the character is drinking, don't nullify the target liquid.
            if (_movement.CurrentState != CharacterStates.MovementStates.Drinking)
            {
                _targetLiquid = null;
            }
            return;
        }

        Liquid hitLiquid = hit.transform.GetComponent<Liquid>();
        _targetLiquid = hitLiquid;
    }

    public virtual void SetLiquid(Liquid liquid)
    {
        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) { return; }

        _targetLiquid = liquid;
    }

    protected virtual void DrinkLiquid()
    {
        if (_targetLiquid == null) { return; }
        if (_movement.CurrentState != CharacterStates.MovementStates.Drinking) { return; }

        float elapsedTime = _drinkTimer.ElapsedTime;

        // If the drink duration has passed, finish drinking
        if (elapsedTime >= DrinkDuration)
        {
            // Make a final adjustment
            _targetLiquid.SetCharge((int)_targetCharge);

            FinishDrink();
            return;
        }

        // Calculate the amount to decrease every frame to smooth out over DrinkDuration
        float amountToDecrease = Time.deltaTime * (1.0f / DrinkDuration);

        // Debug logging
        //Debug.Log($"{this.GetType()}.DrinkLiquid: ElapsedTime = {elapsedTime}. AmountToDecrease = {amountToDecrease}. LiquidCharge = {_targetLiquid.Charge}", gameObject);

        // Lower the charge by the computed amount
        _targetLiquid.LowerCharge(amountToDecrease);
    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter(_drinkingAnimationParameterName, AnimatorControllerParameterType.Bool, out _drinkingAnimationParameter);
    }

    /// <summary>
    /// At the end of each cycle, we send our Running status to the character's animator
    /// </summary>
    public override void UpdateAnimator()
    {
        AnimatorExtensions.UpdateAnimatorBool(_animator, _drinkingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Drinking), _character.AnimatorParameters);
    }

    public override void Flip()
    {
        _currentDirection = _currentDirection == 1 ? -1 : 1;
    }

    public virtual void OnDrawGizmos()
    {
        if (_controller == null) { return; }
        if (_controller.SpriteRenderer == null) { return; }

        Vector2 castOffsetCalculated = new(CastOffset.x * _currentDirection, CastOffset.y);
        Vector2 castDirectionCalculated = new(CastDirection.x * _currentDirection, CastDirection.y);

        JP_Debug.DebugCircleCast2D((Vector2)transform.position + castOffsetCalculated,
                                                CastRadius,
                                                castDirectionCalculated,
                                                CastDistance,
                                                CastLayerMask);
    }
}
