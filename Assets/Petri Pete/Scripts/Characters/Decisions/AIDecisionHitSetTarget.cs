using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools
{
    [RequireComponent(typeof(Health))]
    public class AIDecisionHitSetTarget : AIDecision
    {
        public int NumberOfHits = 1;

        protected int _hitCounter;
        protected Health _health;

        public override void Initialization()
        {
            _health = _brain.gameObject.GetComponent<Health>();
            _hitCounter = 0;
        }

        public override bool Decide()
        {
            return EvaluateHits();
        }

        protected virtual bool EvaluateHits()
        {
            return (_hitCounter >= NumberOfHits);
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _hitCounter = 0;
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _hitCounter = 0;
        }

        // Modified to accept GameObject instigator and set the brain's target
        protected virtual void OnHit(GameObject instigator)
        {
            if (instigator == null) { return; }

            _hitCounter++;

            if (_brain is AIBrain) 
            {
                (_brain as AIBrain).Target = instigator.transform;
            }
        }

        protected virtual void OnEnable()
        {
            if (_health == null)
            {
                _health = this.gameObject.GetComponent<Health>();
            }

            if (_health != null)
            {
                _health.OnHit += OnHit;
            }
        }

        protected virtual void OnDisable()
        {
            if (_health != null)
            {
                _health.OnHit -= OnHit;
            }
        }
    }
}
