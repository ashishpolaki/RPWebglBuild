using UnityEngine;

namespace HorseRace
{
    [CreateAssetMenu(fileName = "HorseJockeyMaterials", menuName = "ScriptableObjects/HorseJockeyMaterials")]
    public class HorseJockeyMaterials : ScriptableObject
    {
        public Material[] horseMaterials;
        public Material[] horseHairMaterials;
        public Material[] jockeyMaterials;
    }
}
