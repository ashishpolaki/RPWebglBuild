using UnityEngine;

namespace CharacterCustomisation
{
    [CreateAssetMenu(fileName = "CharacterPartsCollection", menuName = "ScriptableObjects/Character/CharacterPartsCollection")]
    public class CharacterPartsCollectionSO : ScriptableObject
    {
        public CharacterGenderType CharacterGender;
        public CharacterPartCollection[] CharacterPartCollections;


        public bool CanTurnOffCharacterPart(CharacterPartType _partType)
        {
            foreach (var item in CharacterPartCollections)
            {
                if (item.partType == _partType)
                {
                    return item.canTurnOff;
                }
            }
            return false;
        }

        public int GetCharacterPartVariancesLength(CharacterPartType _partType)
        {
            foreach (var item in CharacterPartCollections)
            {
                if (item.partType == _partType)
                {
                    return item.characterParts.Length;
                }
            }
            return 0;
        }

        public Mesh GetMesh(CharacterPartType _partType, int _index)
        {
            foreach (var item in CharacterPartCollections)
            {
                if (item.partType == _partType)
                {
                    return item.characterParts[_index].mesh;
                }
            }
            return null;
        }
    }
    [System.Serializable]
    public struct CharacterPartCollection
    {
        public CharacterPartType partType;
        public CharacterPartSO[] characterParts;
        public bool canTurnOff;
        public bool canChangeColor;
    }
}
