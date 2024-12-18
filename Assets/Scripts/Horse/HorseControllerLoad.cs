using UnityEngine;

namespace HorseRace
{
    public class HorseControllerLoad : HorseController
    {
        [SerializeField] private Character character;
        public Character Character => character;

        public override void UpdateState()
        {
            base.UpdateState();
        }
        protected override void OnControlPointChange()
        {
            base.OnControlPointChange();
        }
        public override void SetSpline(SplineData splineData)
        {
            currentSplineData = splineData;
            currentSplineIndex = splineData.splineIndex;
        }
        public void SetCharacter(CharacterCustomisationEconomy characterCustomisationEconomy)
        {
            character.Load(characterCustomisationEconomy);
        }
    }
}
