using System.Collections.Generic;
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
        public UpperOutfitSO[] UpperOutfits { get => characterPartCollection.characterUpperOutfits; }
        public LowerOutfitSO[] LowerOutfits { get => characterPartCollection.characterLowerOutfits; }
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
        public List<SyntyCharacterPartType> GetCharacterPartType(BlendPartType blendPartType)
        {
            List<SyntyCharacterPartType> syntyCharacterPartList = new List<SyntyCharacterPartType>();
            switch (blendPartType)
            {
                case BlendPartType.Nose:
                    syntyCharacterPartList.Add(SyntyCharacterPartType.Nose);
                    break;
                case BlendPartType.Eyebrows:
                    syntyCharacterPartList.Add(SyntyCharacterPartType.EyebrowLeft);
                    syntyCharacterPartList.Add(SyntyCharacterPartType.EyebrowRight);
                    break;
                case BlendPartType.Hair:
                    syntyCharacterPartList.Add(SyntyCharacterPartType.Hair);
                    break;
                case BlendPartType.FacialHair:
                    syntyCharacterPartList.Add(SyntyCharacterPartType.FacialHair);
                    break;
                case BlendPartType.Ears:
                    syntyCharacterPartList.Add(SyntyCharacterPartType.EarLeft);
                    syntyCharacterPartList.Add(SyntyCharacterPartType.EarRight);
                    break;
                default:
                    break;
            }
            return syntyCharacterPartList;
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
                case SyntyCharacterPartType.Nose:
                    path = $"CharacterParts/Nose/Nose_{meshIndex}";
                    break;
                case SyntyCharacterPartType.EyebrowLeft:
                    path = $"CharacterParts/LeftEyeBrow/LeftEyeBrow_{meshIndex}";
                    break;
                case SyntyCharacterPartType.EyebrowRight:
                    path = $"CharacterParts/RightEyeBrow/RightEyeBrow_{meshIndex}";
                    break;
                case SyntyCharacterPartType.EarLeft:
                    path = $"CharacterParts/LeftEar/LeftEar_{meshIndex}";
                    break;
                case SyntyCharacterPartType.EarRight:
                    path = $"CharacterParts/RightEar/RightEar_{meshIndex}";
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

        public UpperOutfitEconomy GetUpperOutfitEconomy(int index)
        {
            UpperOutfitSO upperOutfitSO = characterPartCollection.characterUpperOutfits[index];
            UpperOutfitEconomy upperOutfitEconomy = new UpperOutfitEconomy();

            //Set the selected outfit
            upperOutfitEconomy.torso = upperOutfitSO.torsoIndex;
            upperOutfitEconomy.leftUpperArm = upperOutfitSO.upperLeftArmIndex;
            upperOutfitEconomy.rightUpperArm = upperOutfitSO.upperRightArmIndex;

            //Set Colors
            upperOutfitEconomy.torsoColors = new OutfitColorEconomy[upperOutfitSO.torsoColors.Length];
            upperOutfitEconomy.upperArmColors = new OutfitColorEconomy[upperOutfitSO.upperArmColors.Length];

            for (int i = 0; i < upperOutfitEconomy.torsoColors.Length; i++)
            {
                upperOutfitEconomy.torsoColors[i] = new OutfitColorEconomy();
                upperOutfitEconomy.torsoColors[i].color = Utils.ToHex(upperOutfitSO.torsoColors[i].textureColor);
                upperOutfitEconomy.torsoColors[i].u = upperOutfitSO.torsoColors[i].uvCoordinates.x;
                upperOutfitEconomy.torsoColors[i].v = upperOutfitSO.torsoColors[i].uvCoordinates.y;
            }

            for (int i = 0; i < upperOutfitEconomy.upperArmColors.Length; i++)
            {
                upperOutfitEconomy.upperArmColors[i] = new OutfitColorEconomy();
                upperOutfitEconomy.upperArmColors[i].color = Utils.ToHex(upperOutfitSO.upperArmColors[i].textureColor);
                upperOutfitEconomy.upperArmColors[i].u = upperOutfitSO.upperArmColors[i].uvCoordinates.x;
                upperOutfitEconomy.upperArmColors[i].v = upperOutfitSO.upperArmColors[i].uvCoordinates.y;
            }

            return upperOutfitEconomy;
        }

        public LowerOutfitEconomy GetLowerOutfitEconomy(int index)
        {
            LowerOutfitSO lowerOutfitSO = characterPartCollection.characterLowerOutfits[index];
            LowerOutfitEconomy lowerOutfitEconomy = new LowerOutfitEconomy();

            //Set the selected outfit
            lowerOutfitEconomy.hips = lowerOutfitSO.hipsIndex;
            lowerOutfitEconomy.rightLeg = lowerOutfitSO.rightLegIndex;
            lowerOutfitEconomy.leftLeg = lowerOutfitSO.leftLegIndex;
            lowerOutfitEconomy.rightFoot = lowerOutfitSO.rightFootIndex;
            lowerOutfitEconomy.leftFoot = lowerOutfitSO.leftFootIndex;

            //Set Colors
            lowerOutfitEconomy.hipsColors = new OutfitColorEconomy[lowerOutfitSO.hipsColors.Length];
            lowerOutfitEconomy.legColors = new OutfitColorEconomy[lowerOutfitSO.legColors.Length];
            lowerOutfitEconomy.footColors = new OutfitColorEconomy[lowerOutfitSO.footColors.Length];

            for (int i = 0; i < lowerOutfitEconomy.hipsColors.Length; i++)
            {
                lowerOutfitEconomy.hipsColors[i] = new OutfitColorEconomy();
                lowerOutfitEconomy.hipsColors[i].color = Utils.ToHex(lowerOutfitSO.hipsColors[i].textureColor);
                lowerOutfitEconomy.hipsColors[i].u = lowerOutfitSO.hipsColors[i].uvCoordinates.x;
                lowerOutfitEconomy.hipsColors[i].v = lowerOutfitSO.hipsColors[i].uvCoordinates.y;
            }

            for (int i = 0; i < lowerOutfitEconomy.legColors.Length; i++)
            {
                lowerOutfitEconomy.legColors[i] = new OutfitColorEconomy();
                lowerOutfitEconomy.legColors[i].color = Utils.ToHex(lowerOutfitSO.legColors[i].textureColor);
                lowerOutfitEconomy.legColors[i].u = lowerOutfitSO.legColors[i].uvCoordinates.x;
                lowerOutfitEconomy.legColors[i].v = lowerOutfitSO.legColors[i].uvCoordinates.y;
            }

            for (int i = 0; i < lowerOutfitEconomy.footColors.Length; i++)
            {
                lowerOutfitEconomy.footColors[i] = new OutfitColorEconomy();
                lowerOutfitEconomy.footColors[i].color = Utils.ToHex(lowerOutfitSO.footColors[i].textureColor);
                lowerOutfitEconomy.footColors[i].u = lowerOutfitSO.footColors[i].uvCoordinates.x;
                lowerOutfitEconomy.footColors[i].v = lowerOutfitSO.footColors[i].uvCoordinates.y;
            }

            return lowerOutfitEconomy;
        }

        #endregion
    }
}
