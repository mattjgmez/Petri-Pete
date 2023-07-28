using JadePhoenix.Tools;
using UnityEngine;

namespace JadePhoenix.Tools
{
    public class AIActionDrinkTargetPlant : AIAction
    {
        private CharacterDrink _characterDrink;
        private Liquid _targetLiquid;

        protected override void Initialization()
        {
            base.Initialization();
            _characterDrink = this.gameObject.GetComponent<CharacterDrink>();
        }

        public override void PerformAction()
        {
            DrinkTarget();
        }

        protected void DrinkTarget()
        {
            if (_brain.Target == null)
            {
                return;
            }

            _targetLiquid = _brain.Target.GetComponent<Liquid>();

            if (_targetLiquid != null && _characterDrink.DrinkableLiquids.Contains(_targetLiquid.LiquidType))
            {
                // Set the liquid and start drinking
                _characterDrink.SetLiquid(_targetLiquid);
                _characterDrink.StartDrink();
            }
        }
    }
}
