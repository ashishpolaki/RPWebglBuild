using System.Collections.Generic;
using UI;
using UnityEngine;

namespace CharacterCustomisation
{
    public class CharacterCustomisationManager : MonoBehaviour
    {
        public static CharacterCustomisationManager Instance { get; private set; }

        #region Inspector Variables
        [SerializeField] private CharacterPartsCollectionSO characterPartCollection;
        [SerializeField] private Character myCharacter;
        #endregion

        #region Private Variables
        [SerializeField] private FullBodyEconomy currentCharacterData = new FullBodyEconomy();
        private int upperPartOutfitIndex;
        private int lowerPartOutfitIndex;
        private List<int> createdBlendShapeList = new List<int>();
        private Dictionary<string, int> createdBlendShapeSubDictionary = new Dictionary<string, int>();
        #endregion

        #region Texture UV's Colors
        private static readonly Vector2Int HairUV = new Vector2Int(6, 4);
        private static readonly Vector2Int FacialHairUV = new Vector2Int(7, 4);
        #endregion

        #region Property
        public CharacterPartSO CurrentPartSO { get; private set; }
        public int CharacterPartsLength { get => characterPartCollection.characterParts.Length; }
        public int CurrentPartVariancesCount { get => CurrentPartSO.parts.Length; }
        public int CurrentPartColorCount { get => CurrentPartSO.colorPreset.colors.Length; }
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

        #region Color 
        public void ChangeSkinToneColor(Color color)
        {
            currentCharacterData.skinToneColor = ToHex(color);
            myCharacter.ChangeSkinToneColor(color);
        }

        public void ChangePartColor(int value)
        {
            Color color = CurrentPartSO.colorPreset.colors[value];
            Vector2Int textureUV = new Vector2Int(0, 0);

            switch (CurrentPartSO.partType)
            {
                case BlendPartType.Nose:
                    break;
                case BlendPartType.Eyebrows:
                    break;
                case BlendPartType.Hair:
                    textureUV = HairUV;
                    break;
                case BlendPartType.FacialHair:
                    textureUV = FacialHairUV;
                    break;
                default:
                    break;
            }

            //Add partType in list if not exist
            if (!createdBlendShapeList.Contains((int)CurrentPartSO.partType))
            {
                createdBlendShapeList.Add((int)CurrentPartSO.partType);
                CustomPartEconomy customPartEconomy = new CustomPartEconomy();
                customPartEconomy.type = (int)CurrentPartSO.partType;
                currentCharacterData.customParts.Add(customPartEconomy);
            }
            int currentpartIndex = createdBlendShapeList.IndexOf((int)CurrentPartSO.partType);
            currentCharacterData.customParts[currentpartIndex].color = ToHex(color);
            myCharacter.ChangePartColor(color, textureUV);
        }
        public string ToHex(Color color)
        {
            Color32 color32 = color;
            return $"{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
        }
        public Color FromHex(string hex)
        {
            if (hex.Length != 8)
            {
                return Color.white;
            }

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }
        #endregion

        #region BlendShape
        public void OnBodySizeValueChange(float value)
        {
            currentCharacterData.bodyType = value;
            float heavyBlendValue = 0;
            float skinnyBlendValue = 0;
            if (value < 50)
            {
                heavyBlendValue = 0;
                skinnyBlendValue = (50 - value) * 2;
            }
            else if (value > 50)
            {
                skinnyBlendValue = 0;
                heavyBlendValue = (value - 50) * 2;
            }
            UpdateBlendShape(heavyBlendValue, StringUtils.BLEND_SHAPE_HEAVY);
            UpdateBlendShape(skinnyBlendValue, StringUtils.BLEND_SHAPE_SKINNY);
        }

        public void OnBodyTypeValueChange(float value)
        {
            currentCharacterData.bodyGenderType = value;
            UpdateBlendShape(value, StringUtils.BLEND_GENDER);
        }

        public void OnMusculatureValueChange(float value)
        {
            currentCharacterData.bodyMuscleType = value;
            UpdateBlendShape(value, StringUtils.BLEND_MUSCLE);
        }

        public void UpdateBlendShape(float value, string _blendPartName)
        {
            myCharacter.SetBlendShape(value, _blendPartName);
        }

        public void SetBlendShapes(BlendShapePartData blendShapePartData, float value)
        {
            //Add partType in list if not exist
            if (!createdBlendShapeList.Contains((int)CurrentPartSO.partType))
            {
                createdBlendShapeList.Add((int)CurrentPartSO.partType);
                CustomPartEconomy customPartEconomy = new CustomPartEconomy();
                customPartEconomy.type = (int)CurrentPartSO.partType;
                currentCharacterData.customParts.Add(customPartEconomy);
            }

            //Store name in dictionary if not exist
            int currentpartIndex = createdBlendShapeList.IndexOf((int)CurrentPartSO.partType);
            if (!createdBlendShapeSubDictionary.ContainsKey(blendShapePartData.name))
            {
                createdBlendShapeSubDictionary[blendShapePartData.name] = currentCharacterData.customParts[currentpartIndex].blendShapes.Count;
                currentCharacterData.customParts[currentpartIndex].blendShapes.Add(new BlendShapeEconomy { name = blendShapePartData.name, value = 0 });
                return;
            }

            //Update Value for saving it in cloud
            int index = createdBlendShapeSubDictionary[blendShapePartData.name];
            currentCharacterData.customParts[currentpartIndex].blendShapes[index].value = value;

            //Update Mesh 
            foreach (var blendShapeName in blendShapePartData.blendShapeNames)
            {
                foreach (var syntyCharacterPartType in blendShapePartData.syntyCharacterPartTypes)
                {
                    myCharacter.SetBlendShape(syntyCharacterPartType, value, blendShapeName);
                }
            }
        }
        #endregion

        #region Transform
        public void SetPosition(Vector3 position)
        {
            myCharacter.transform.position = position;
        }
        public void SetScale(Vector3 scale)
        {
            myCharacter.transform.localScale = scale;
        }
        #endregion

        #region Change Part
        public void ChangePart(int index)
        {
            int meshIndex = CurrentPartSO.parts[index];
            string path = string.Empty;
            SyntyCharacterPartType syntyCharacterPartType = SyntyCharacterPartType.None;
            switch (CurrentPartSO.partType)
            {
                case BlendPartType.Nose:
                    break;
                case BlendPartType.Eyebrows:
                    break;
                case BlendPartType.Hair:
                    path = $"CharacterParts/Hair/Hair_{meshIndex}";
                    syntyCharacterPartType = SyntyCharacterPartType.Hair;
                    break;
                case BlendPartType.FacialHair:
                    path = $"CharacterParts/FacialHair/FacialHair_{meshIndex}";
                    syntyCharacterPartType = SyntyCharacterPartType.FacialHair;
                    break;
                default:
                    break;
            }
            //Add partType in list if not exist
            if (!createdBlendShapeList.Contains((int)CurrentPartSO.partType))
            {
                createdBlendShapeList.Add((int)CurrentPartSO.partType);
                CustomPartEconomy customPartEconomy = new CustomPartEconomy();
                customPartEconomy.type = (int)CurrentPartSO.partType;
                currentCharacterData.customParts.Add(customPartEconomy);
            }
            int currentpartIndex = createdBlendShapeList.IndexOf((int)CurrentPartSO.partType);
            currentCharacterData.customParts[currentpartIndex].styleNumber = meshIndex;
            if (meshIndex == -1)
            {
                myCharacter.TurnOffPart(syntyCharacterPartType);
            }
            else
            {
                myCharacter.ChangePart(syntyCharacterPartType, path);
            }
        }
        #endregion

        #region Change Outfits
        public void ChangeUpperOutfit(int index)
        {
            if (upperPartOutfitIndex + index < 0 || upperPartOutfitIndex + index > 2)
            {
                return;
            }
            upperPartOutfitIndex += index;

            currentCharacterData.upperOutfit.torso += index;
            currentCharacterData.upperOutfit.rightUpperArm += index;
            currentCharacterData.upperOutfit.leftUpperArm += index;
            currentCharacterData.upperOutfit.rightHand += index;
            currentCharacterData.upperOutfit.leftHand += index;
            currentCharacterData.upperOutfit.rightLowerArm += index;
            currentCharacterData.upperOutfit.leftLowerArm += index;
            myCharacter.ChangeUpperPart(currentCharacterData.upperOutfit);
        }
        public void ChangeLowerOutfit(int index)
        {
            if (lowerPartOutfitIndex + index < 0 || lowerPartOutfitIndex + index > 2)
            {
                return;
            }
            lowerPartOutfitIndex += index;

            currentCharacterData.lowerOutfit.hips += index;
            currentCharacterData.lowerOutfit.rightLeg += index;
            currentCharacterData.lowerOutfit.leftLeg += index;
            currentCharacterData.lowerOutfit.rightFoot += index;
            currentCharacterData.lowerOutfit.leftFoot += index;
            myCharacter.ChangeLowerPart(currentCharacterData.lowerOutfit);
        }
        #endregion

        public CharacterPartSO GetCharacterPart(int blendPartTypeIndex)
        {
            CurrentPartSO = characterPartCollection.GetCharacterPart((BlendPartType)blendPartTypeIndex);
            return CurrentPartSO;
        }

        public void EnableFace()
        {
            myCharacter.EnableFace();
        }

        public void EnableFullBody()
        {
            myCharacter.EnableFullBody();
        }

        public async void SaveCharacterData()
        {
            //saveCharacterData.SetCharacterPartsList((int)currentCharacterGender, characterPartsIndexes, characterPartColorIndexes);
            //Func<Task> method = async () => await UGSManager.Instance.CloudSave.SetCharacterDataAsync("CharacterData", saveCharacterData);
            //await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
        }
    }
}
