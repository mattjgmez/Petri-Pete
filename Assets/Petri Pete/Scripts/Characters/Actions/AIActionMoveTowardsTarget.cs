using UnityEngine;

namespace JadePhoenix.Tools
{
    public class AIActionMoveTowardsTarget : AIAction
    {
        /// the minimum distance from the target this Character can reach.
        public float MinimumDistance = 1f;

        protected CharacterMovement _characterMovement;
        protected int _numberOfJumps = 0;

        /// <summary>
        /// On init we grab our CharacterMovement ability
        /// </summary>
        protected override void Initialization()
        {
            _characterMovement = this.gameObject.GetComponent<CharacterMovement>();
        }

        /// <summary>
        /// On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Moves the character towards the target if needed
        /// </summary>
        protected virtual void Move()
        {
            if (_brain.Target == null)
            {
                return;
            }

            if (this.transform.position.x < _brain.Target.position.x)
            {
                _characterMovement.SetHorizontalMovement(1f);
            }
            else
            {
                _characterMovement.SetHorizontalMovement(-1f);
            }

            if (this.transform.position.y < _brain.Target.position.y)
            {
                _characterMovement.SetVerticalMovement(1f);
            }
            else
            {
                _characterMovement.SetVerticalMovement(-1f);
            }

            if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) < MinimumDistance)
            {
                _characterMovement.SetHorizontalMovement(0f);
            }

            if (Mathf.Abs(this.transform.position.y - _brain.Target.position.y) < MinimumDistance)
            {
                _characterMovement.SetVerticalMovement(0f);
            }
        }

        /// <summary>
        /// On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
        }
    }
}
