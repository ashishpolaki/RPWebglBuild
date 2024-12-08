using System;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace CharacterCustomisation
{
    public class CharacterCustomisationManager : MonoBehaviour
    {
        public static CharacterCustomisationManager Instance { get; private set; }

        #region Inspector Variables
        [SerializeField] private CharacterPartsCollectionSO characterPartCollection;
        [SerializeField] private ColorPresetSO skinToneColorPreset;
        [SerializeField] private Character characterPrefab;
        #endregion

        #region Property
        public ColorPresetSO SkinToneColorPreset { get => skinToneColorPreset; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Public Methods
        public SyntyCharacterPartType GetCharacterPartType(BlendPartType blendPartType)
        {
            SyntyCharacterPartType syntyCharacterPartType = SyntyCharacterPartType.None;
            switch (blendPartType)
            {
                case BlendPartType.Nose:
                    break;
                case BlendPartType.Eyebrows:
                    break;
                case BlendPartType.Hair:
                    syntyCharacterPartType = SyntyCharacterPartType.Hair;
                    break;
                case BlendPartType.FacialHair:
                    syntyCharacterPartType = SyntyCharacterPartType.FacialHair;
                    break;
                default:
                    break;
            }
            return syntyCharacterPartType;
        }
        public string GetMeshPath(SyntyCharacterPartType characterPartType, int meshIndex)
        {
            string path = string.Empty;
            switch (characterPartType)
            {
                case SyntyCharacterPartType.Hair:
                    path = $"CharacterParts/Hair/Hair_{meshIndex}";
                    break;
                case SyntyCharacterPartType.FacialHair:
                    path = $"CharacterParts/FacialHair/FacialHair_{meshIndex}";
                    break;
                default:
                    break;
            }
            return path;
        }
        public CharacterPartSO GetCharacterPartSO(BlendPartType blendPartType)
        {
            CharacterPartSO characterPartSO = characterPartCollection.GetCharacterPart(blendPartType);
            return characterPartSO;
        }
        public Character InstantiateCharacter()
        {
            Character character = Instantiate(characterPrefab);
            return character;
        }
        #endregion

    }
}
