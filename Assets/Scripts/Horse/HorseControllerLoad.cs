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
        protected override void ControlPointChange()
        {
            GameManager.Instance.RaceManager.ChangeControlPoint(HorseNumber);
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
