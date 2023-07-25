using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools
{
    public abstract class Debuff : MonoBehaviour
    {
        public float Duration = 5f;
        public bool Stackable = false;
        public Character TargetCharacter;

        /// <summary>
        /// Effects triggered during the Debuff's duration.
        /// </summary>
        public abstract void ProcessDebuff();

        protected Timer _debuffTimer;

        public Debuff() { }

        public Debuff(float duration, bool stackable, Character targetCharacter)
        {
            Duration = duration;
            Stackable = stackable;
            TargetCharacter = targetCharacter;
            Initialize();
        }

        public Debuff(Debuff debuff)
        {
            Duration = debuff.Duration;
            Stackable = debuff.Stackable;
            TargetCharacter = debuff.TargetCharacter;
            Initialize();
        }

        public Debuff(Debuff debuff, Character targetCharacter)
        {
            Duration = debuff.Duration;
            Stackable = debuff.Stackable;
            TargetCharacter = targetCharacter;
            Initialize();
        }

        /// <summary>
        /// Initializes the debuff.
        /// </summary>
        public virtual void Initialize()
        {
            _debuffTimer = new Timer(Duration, OnActivated, RemoveDebuff);
        }

        /// <summary>
        /// Effects triggered at the start of a Debuff's duration.
        /// </summary>
        protected virtual void OnActivated() { }

        /// <summary>
        /// Effects triggered at the end of a Debuff's duration.
        /// </summary>
        protected virtual void OnDeactivated() { }

        /// <summary>
        /// Removes the debuff, triggering its OnDeactivated effect.
        /// </summary>
        public virtual void RemoveDebuff()
        {
            _debuffTimer.StopTimer();
            OnDeactivated();
            Destroy(this);
        }

        /// <summary>
        /// Refreshes the debuff timer, triggering its OnActivated effect.
        /// </summary>
        public virtual void RefreshDebuff()
        {
            _debuffTimer.ResetTimer();
            _debuffTimer.StartTimer();
            OnActivated();
        }
    }
}
