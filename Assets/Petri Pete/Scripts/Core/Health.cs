using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // The model to disable (if set to).
    public GameObject Model;

    // The current health of the character.
    public int CurrentHealth;
    // If true, this object will not take damage.
    public bool Invulnerable = false;

    [Header("Health")]
    public int MaxHealth;
    public int InitialHealth;
    public float HealthPercentage { get { return ((float)CurrentHealth / (float)MaxHealth); } }

    [Header("Damage")]
    public bool ImmuneToKnockback = false;

    [Header("Death")]
    public bool DestroyOnDeath = true;
    public float DelayBeforeDestruction = 0f;
    public int PointsWhenDestroyed;
    public bool RespawnAtInitialLocation = false;
    public bool DisableControllerOnDeath = true;
    public bool DisableModelOnDeath = true;
    public bool DisableCollisionsOnDeath = true;

    // hit delegate
    public delegate void OnHitDelegate(GameObject instigator);
    public OnHitDelegate OnHit;

    // respawn delegate
    public delegate void OnReviveDelegate();
    public OnReviveDelegate OnRevive;

    // death delegate
    public delegate void OnDeathDelegate();
    public OnDeathDelegate OnDeath;

    // health change delegate
    public delegate void OnHealthChangeDelegate();
    public OnHealthChangeDelegate OnHealthChange;

    public Character Character { get { return _character; } }

    protected Vector3 _initialPosition;
    protected Character _character;
    protected TopDownController _controller;
    protected Collider _collider;
    protected bool _initialized = false;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _character = GetComponent<Character>();
        if (Model != null)
        {
            Model.SetActive(true);
        }

        _controller = GetComponent<TopDownController>();
        _collider = GetComponent<Collider>();

        _initialPosition = transform.position;
        _initialized = true;
        CurrentHealth = InitialHealth;
        DamageEnabled();
        OnHealthChange?.Invoke();
    }

    /// <summary>
    /// Destroys the object, or tries to, depending on the character's settings
    /// </summary>
    protected virtual void DestroyObject()
    {
        if (!DestroyOnDeath)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    #region PUBLIC METHODS

    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public virtual void Damage(int damage, GameObject instigator, float invincibilityDuration)
    {
        // If the object is invulnerable, or we're already below zero, we do nothing and exit.
        if (Invulnerable || ((CurrentHealth <= 0) && (InitialHealth != 0)))
        {
            return;
        }

        float previousHealth = CurrentHealth;
        CurrentHealth -= damage;
        OnHealthChange?.Invoke();

        OnHit?.Invoke(instigator);

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabledCoroutine(invincibilityDuration));
        }

        // we trigger a damage taken event
        CharacterEvents.DamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        if (_character.CharacterType == Character.CharacterTypes.Player)
        {
            UIManager.Instance.UpdateHealthBar(HealthPercentage);
        }

        // if health has reached zero
        if (CurrentHealth <= 0)
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;
            OnHealthChange?.Invoke();

            Kill();
        }
    }

    /// <summary>
    /// Kills the character, instantiates death effects, handles points, etc
    /// </summary>
    public virtual void Kill()
    {
        if (_character != null)
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
            _character.Reset();

            UIManager.Instance.AddJournalEntryWithID($"{this.gameObject.name}Slain");

            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                if (DelayBeforeDestruction > 0f)
                {
                    Invoke(nameof(TriggerGameOver), DelayBeforeDestruction);
                }
                else
                {
                    TriggerGameOver();
                }
            }
        }

        CurrentHealth = 0;
        OnHealthChange?.Invoke();

        DamageDisabled();

        if (PointsWhenDestroyed != 0)
        {
            // ADD GAMEMANAGER EVENT FOR GAINING POINTS?
        }

        if (DisableCollisionsOnDeath)
        {
            if (_collider != null)
            {
                _collider.enabled = false;
            }
            if (_controller != null)
            {
                _controller.CollisionsOff();
            }
        }

        OnDeath?.Invoke();

        if (DisableControllerOnDeath && (_controller != null))
        {
            _controller.enabled = false;
            //_characterController.SetKinematic(true);
        }

        if (DisableModelOnDeath && (Model != null))
        {
            Model.SetActive(false);
        }

        if (DelayBeforeDestruction > 0f)
        {
            Invoke(nameof(DestroyObject), DelayBeforeDestruction);
        }
        else
        {
            // finally we destroy the object
            DestroyObject();
        }
    }

    /// <summary>
    /// Helper method to delay game over in Kill method.
    /// </summary>
    protected virtual void TriggerGameOver()
    {
        GameManager.Instance.TriggerGameOver();
    }

    /// <summary>
    /// Revive this object.
    /// </summary>
    public virtual void Revive()
    {
        if (!_initialized)
        {
            return;
        }

        if (_collider != null)
        {
            _collider.enabled = true;
        }
        if (_controller != null)
        {
            _controller.enabled = true;
            _controller.CollisionsOn();
            _controller.Reset();
        }
        if (_character != null)
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        }

        if (RespawnAtInitialLocation)
        {
            transform.position = _initialPosition;
        }
        //if (_healthBar != null)
        //{
        //    _healthBar.Initialization();
        //}

        Initialization();
        OnHealthChange?.Invoke();

        OnRevive?.Invoke();
    }

    /// <summary>
    /// Called when the character gets health (from a stimpack for example)
    /// </summary>
    /// <param name="amountToHeal">The health the character gets.</param>
    /// <param name="instigator">The thing that gives the character health.</param>
    public virtual void Heal(int amountToHeal, GameObject instigator)
    {
        // this function adds health to the character's Health and prevents it to go above MaxHealth.
        CurrentHealth = Mathf.Min(CurrentHealth + amountToHeal, MaxHealth);
        OnHealthChange?.Invoke();

        if (_character.CharacterType == Character.CharacterTypes.Player)
        {
            UIManager.Instance.UpdateHealthBar(HealthPercentage);
        }
    }

    /// <summary>
    /// Resets the character's health to its max value
    /// </summary>
    public virtual void ResetHealthToMaxHealth()
    {
        CurrentHealth = MaxHealth;
        OnHealthChange?.Invoke();
    }

    /// <summary>
    /// Prevents the character from taking any damage.
    /// </summary>
    public virtual void DamageDisabled()
    {
        Invulnerable = true;
    }

    /// <summary>
    /// Allows the character to take damage.
    /// </summary>
    public virtual void DamageEnabled()
    {
        Invulnerable = false;
    }

    #endregion

    #region COROUTINES

    /// <summary>
    /// Makes the character able to take damage again after the specified delay.
    /// </summary>
    /// <returns>The layer collision.</returns>
    public virtual IEnumerator DamageEnabledCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Invulnerable = false;
    }

    #endregion

    protected virtual void OnEnable()
    {
        CurrentHealth = InitialHealth;
        if (Model != null)
        {
            Model.SetActive(true);
        }
        DamageEnabled();
        OnHealthChange?.Invoke();
    }

    /// <summary>
    /// On Disable, we prevent any delayed destruction from running
    /// </summary>
    protected virtual void OnDisable()
    {
        CancelInvoke();
    }
}
