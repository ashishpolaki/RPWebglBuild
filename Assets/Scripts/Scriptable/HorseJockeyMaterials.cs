using UnityEngine;

namespace HorseRace
{
    [CreateAssetMenu(fileName = "HorseJockeyMaterials", menuName = "HorseRace/HorseJockeyMaterials")]
    public class HorseJockeyMaterials : ScriptableObject
    {
        public Material[] horseMaterials;
        public Material[] horseHairMaterials;
        public Material[] jockeyMaterials;
    }
}
