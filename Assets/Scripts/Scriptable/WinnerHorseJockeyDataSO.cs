using UnityEngine;

namespace HorseRace
{
    [CreateAssetMenu(fileName = "HorseJockeyData", menuName = "HorseRace/WinnerHorseData")]
    public class WinnerHorseJockeyDataSO : ScriptableObject
    {
        public Material[] winnerHorseMaterials;
        public Material[] WinnerJockeyMaterials; 

        /// <summary>
        /// Store the materials for the winner horse and jockey
        /// </summary>
        /// <param name="_horseMaterials"></param>
        /// <param name="_jockeyMaterials"></param>
        public void SetMaterials(Material[] _horseMaterials, Material[] _jockeyMaterials)
        {
            winnerHorseMaterials = _horseMaterials;
            WinnerJockeyMaterials = _jockeyMaterials;
        }
    }
}
