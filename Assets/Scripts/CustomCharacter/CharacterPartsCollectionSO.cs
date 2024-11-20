using UnityEngine;

namespace CharacterCustomisation
{
    [CreateAssetMenu(fileName = "CharacterPartsCollection", menuName = "ScriptableObjects/Character/CharacterPartsCollection")]
    public class CharacterPartsCollectionSO : ScriptableObject
    {
        public CharacterPartSO[] characterParts;

        public CharacterPartSO GetCharacterPart(BlendPartType partType)
        {
            foreach (CharacterPartSO part in characterParts)
            {
                if (part.partType == partType)
                {
                    return part;
                }
            }
            return null;
        }
    }
}
