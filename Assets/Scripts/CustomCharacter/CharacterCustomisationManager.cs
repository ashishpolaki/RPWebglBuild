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

        [SerializeField] private CharacterPartsCollectionSO[] characterPartCollections;
        [SerializeField] private CharacterGenderType currentCharacterGender;
        [SerializeField] private Character myCharacter;
        [SerializeField] private CharacterPartUIType currentCharacterPartUIType;
        [SerializeField] private int[] characterPartsIndexes;
        [SerializeField] private string[] characterPartColorIndexes;
        #endregion

        #region Property
        public CharacterGenderType CurrentCharacterGender { get => currentCharacterGender; }
        #endregion

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
        private void Start()
        {
            characterPartsIndexes = new int[Enum.GetValues(typeof(CharacterPartType)).Length];
            characterPartColorIndexes = new string[Enum.GetValues(typeof(CharacterPartUIType)).Length];
            InitializeData();
        }

        private async void InitializeData()
        {
            SaveCharacterData saveCharacterData = await UGSManager.Instance.CloudSave.GetCharacterDataAsync("CharacterData");
            if (saveCharacterData != null)
            {
                //Set Gender
                currentCharacterGender = (CharacterGenderType)saveCharacterData.characterGenderIndex;
                ChangeGender(currentCharacterGender);

                //Set Body Parts
                for (int i = 0; i < characterPartsIndexes.Length; i++)
                {
                    characterPartsIndexes[i] = saveCharacterData.characterPartIndexes[i];
                    ChangeBodyPart((CharacterPartType)i);
                }

                //Set Body Part Color
                for (int i = 0; i < characterPartColorIndexes.Length; i++)
                {
                    characterPartColorIndexes[i] = saveCharacterData.characterPartColorIndexes[i];
                    if (!StringUtils.IsStringEmpty(characterPartColorIndexes[i]))
                    {
                        ChangeBodyPartColor(FromHex(characterPartColorIndexes[i]));
                    }
                }
            }
            else
            {
                ChangeGender(currentCharacterGender);

                //Set Body Parts
                for (int i = 0; i < characterPartsIndexes.Length; i++)
                {
                    ChangeBodyPart((CharacterPartType)i, characterPartsIndexes[i]);
                }

                //Set Body Part Color
                for (int i = 0; i < characterPartColorIndexes.Length; i++)
                {
                    if (!StringUtils.IsStringEmpty(characterPartColorIndexes[i]))
                    {
                        ChangeBodyPartColor(FromHex(characterPartColorIndexes[i]));
                    }
                }
            }
        }

        public void ChangeGenderAndBodyParts(CharacterGenderType characterGenderType)
        {
            ChangeGender(characterGenderType);

            //Set Body Parts
            for (int i = 0; i < characterPartsIndexes.Length; i++)
            {
                ChangeBodyPart((CharacterPartType)i);
            }
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
        public void ChangeBodyPartColor(Color color)
        {
            characterPartColorIndexes[(int)currentCharacterPartUIType] = ToHex(color);
            myCharacter.SetPartColor(currentCharacterPartUIType, color);
        }

        public async void SaveCharacterData()
        {
            SaveCharacterData saveCharacterData = new SaveCharacterData();
            saveCharacterData.SetCharacterPartsList((int)currentCharacterGender, characterPartsIndexes, characterPartColorIndexes);
            Func<Task> method = async () => await UGSManager.Instance.CloudSave.SetCharacterDataAsync("CharacterData", saveCharacterData);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
        }

        private void ChangeGender(CharacterGenderType _characterGenderType)
        {
            currentCharacterGender = _characterGenderType;
        }

        public void SetCurrentSelectedPart(CharacterPartUIType characterPartType)
        {
            currentCharacterPartUIType = characterPartType;
        }

        public void ChangeBodyPart(CharacterPartUIType characterPartUIType, int changeDirection)
        {
            switch (characterPartUIType)
            {
                case CharacterPartUIType.FACE:
                    ChangeBodyPart(CharacterPartType.Head, changeDirection);
                    break;
                case CharacterPartUIType.HAIR:
                    ChangeBodyPart(CharacterPartType.Hair, changeDirection);
                    break;
                case CharacterPartUIType.EYEBROWS:
                    ChangeBodyPart(CharacterPartType.Eyebrows, changeDirection);
                    break;
                case CharacterPartUIType.BEARD:
                    ChangeBodyPart(CharacterPartType.FacialHair, changeDirection);
                    break;
                case CharacterPartUIType.HAT:
                    ChangeBodyPart(CharacterPartType.Hat, changeDirection);
                    break;
                case CharacterPartUIType.MASK:
                    ChangeBodyPart(CharacterPartType.Mask, changeDirection);
                    break;
                case CharacterPartUIType.TORSO:
                    ChangeBodyPart(CharacterPartType.Torso, changeDirection);
                    break;
                case CharacterPartUIType.ARMS:
                    ChangeBodyPart(CharacterPartType.RightUpperArm, changeDirection);
                    ChangeBodyPart(CharacterPartType.LeftUpperArm, changeDirection);
                    break;
                case CharacterPartUIType.HANDS:
                    ChangeBodyPart(CharacterPartType.RightHand, changeDirection);
                    ChangeBodyPart(CharacterPartType.LeftHand, changeDirection);
                    break;
                case CharacterPartUIType.LEGS:
                    ChangeBodyPart(CharacterPartType.Hips, changeDirection);
                    ChangeBodyPart(CharacterPartType.RightLeg, changeDirection);
                    ChangeBodyPart(CharacterPartType.LeftLeg, changeDirection);
                    break;
                default:
                    break;
            }
        }

        private void ChangeBodyPart(CharacterPartType partType, int changeDirection = 0)
        {
            int partVariancesCount = characterPartCollections[(int)currentCharacterGender].GetCharacterPartVariancesLength(partType);
            int currentPartIndex = characterPartsIndexes[(int)partType];

            //If there are no variances for the part, turn off the mesh
            if (partVariancesCount <= 0)
            {
                myCharacter.TurnOffCharacterPart(partType);
                return;
            }

            //Calculate the next part index
            int nextPartIndex = (currentPartIndex + changeDirection + partVariancesCount) % partVariancesCount;

            //Set the part index and change mesh
            characterPartsIndexes[(int)partType] = nextPartIndex;
            myCharacter.ChangeCharacterPart(partType, characterPartCollections[(int)currentCharacterGender].GetMesh(partType, nextPartIndex));
        }
    }
}
