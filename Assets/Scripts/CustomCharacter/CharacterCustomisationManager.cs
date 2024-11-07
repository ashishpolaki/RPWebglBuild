using System;
using UnityEngine;

namespace CharacterCustomisation
{
    public class CharacterCustomisationManager : MonoBehaviour
    {
        public static CharacterCustomisationManager Instance { get; private set; }
        [SerializeField] private CharacterPartsCollectionSO[] characterPartCollections;
        [SerializeField] private CharacterGenderType currentCharacterGender;
        [SerializeField] private Character myCharacter;
        [SerializeField] private CharacterPartUIType currentCharacterPartUIType;

        private int hatIndex = 0;
        private int hairIndex;
        private int eyesBrowsIndex;
        private int facialHairIndex = 0;
        private int headIndex;
        private int torsoIndex;
        private int maskIndex = 0;
        private int rightUpperArmIndex;
        private int rightLowerArmIndex;
        private int rightHandIndex;
        private int leftUpperArmIndex;
        private int leftLowerArmIndex;
        private int leftHandIndex;
        private int hipsIndex;
        private int rightLegIndex;
        private int leftLegIndex;

        public CharacterGenderType CurrentCharacterGender { get => currentCharacterGender; }

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
            LoadCharacterMesh();
        }
        public void InitializeData()
        {

        }

        public void ChangeBodyPartColor(Color color)
        {
            myCharacter.SetPartColor(currentCharacterPartUIType, color);

        }


        public void LoadCharacterMesh()
        {
            ChangeBodyPart(CharacterPartType.Head, ref headIndex, headIndex);
            ChangeBodyPart(CharacterPartType.Hair, ref hairIndex, hairIndex);
            ChangeBodyPart(CharacterPartType.Eyebrows, ref eyesBrowsIndex, eyesBrowsIndex);
            ChangeBodyPart(CharacterPartType.FacialHair, ref facialHairIndex, facialHairIndex);
            ChangeBodyPart(CharacterPartType.Hat, ref hatIndex, hatIndex);
            ChangeBodyPart(CharacterPartType.Mask, ref maskIndex, maskIndex);
            ChangeBodyPart(CharacterPartType.Torso, ref torsoIndex, torsoIndex);
            ChangeBodyPart(CharacterPartType.RightUpperArm, ref rightUpperArmIndex, rightUpperArmIndex);
            ChangeBodyPart(CharacterPartType.LeftUpperArm, ref leftUpperArmIndex, leftUpperArmIndex);
            ChangeBodyPart(CharacterPartType.RightHand, ref rightHandIndex, rightHandIndex);
            ChangeBodyPart(CharacterPartType.LeftHand, ref leftHandIndex, leftHandIndex);
            ChangeBodyPart(CharacterPartType.Hips, ref hipsIndex, hipsIndex);
            ChangeBodyPart(CharacterPartType.RightLeg, ref rightLegIndex, rightLegIndex);
            ChangeBodyPart(CharacterPartType.LeftLeg, ref leftLegIndex, leftLegIndex);
        }

        public void ChangeGender(CharacterGenderType _characterGenderType)
        {
            currentCharacterGender = _characterGenderType;
            ChangeBodyPart(CharacterPartType.Head, ref headIndex, 0);
            ChangeBodyPart(CharacterPartType.Hair, ref hairIndex, 0);
            ChangeBodyPart(CharacterPartType.Eyebrows, ref eyesBrowsIndex, 0);
            ChangeBodyPart(CharacterPartType.FacialHair, ref facialHairIndex, 0);
            ChangeBodyPart(CharacterPartType.Hat, ref hatIndex, 0);
            ChangeBodyPart(CharacterPartType.Mask, ref maskIndex, 0);
            ChangeBodyPart(CharacterPartType.Torso, ref torsoIndex, 0);
            ChangeBodyPart(CharacterPartType.RightUpperArm, ref rightUpperArmIndex, 0);
            ChangeBodyPart(CharacterPartType.LeftUpperArm, ref leftUpperArmIndex, 0);
            ChangeBodyPart(CharacterPartType.RightHand, ref rightHandIndex, 0);
            ChangeBodyPart(CharacterPartType.LeftHand, ref leftHandIndex, 0);
            ChangeBodyPart(CharacterPartType.Hips, ref hipsIndex, 0);
            ChangeBodyPart(CharacterPartType.RightLeg, ref rightLegIndex, 0);
            ChangeBodyPart(CharacterPartType.LeftLeg, ref leftLegIndex, 0);
        }

        public void SetCharacterPartType(CharacterPartUIType characterPartType)
        {
            currentCharacterPartUIType = characterPartType;
        }

        public void ChangeBodyPart(CharacterPartUIType characterPartUIType, int changeDirection)
        {
            switch (characterPartUIType)
            {
                case CharacterPartUIType.FACE:
                    ChangeBodyPart(CharacterPartType.Head, ref headIndex, changeDirection);
                    break;
                case CharacterPartUIType.HAIR:
                    ChangeBodyPart(CharacterPartType.Hair, ref hairIndex, changeDirection);
                    break;
                case CharacterPartUIType.EYEBROWS:
                    ChangeBodyPart(CharacterPartType.Eyebrows, ref eyesBrowsIndex, changeDirection);
                    break;
                case CharacterPartUIType.BEARD:
                    ChangeBodyPart(CharacterPartType.FacialHair, ref facialHairIndex, changeDirection);
                    break;
                case CharacterPartUIType.HAT:
                    ChangeBodyPart(CharacterPartType.Hat, ref hatIndex, changeDirection);
                    break;
                case CharacterPartUIType.MASK:
                    ChangeBodyPart(CharacterPartType.Mask, ref maskIndex, changeDirection);
                    break;
                case CharacterPartUIType.TORSO:
                    ChangeBodyPart(CharacterPartType.Torso, ref torsoIndex, changeDirection);
                    break;
                case CharacterPartUIType.ARMS:
                    ChangeBodyPart(CharacterPartType.RightUpperArm, ref rightUpperArmIndex, changeDirection);
                    ChangeBodyPart(CharacterPartType.LeftUpperArm, ref leftUpperArmIndex, changeDirection);
                    break;
                case CharacterPartUIType.HANDS:
                    ChangeBodyPart(CharacterPartType.RightHand, ref rightHandIndex, changeDirection);
                    ChangeBodyPart(CharacterPartType.LeftHand, ref leftHandIndex, changeDirection);
                    break;
                case CharacterPartUIType.LEGS:
                    ChangeBodyPart(CharacterPartType.Hips, ref hipsIndex, changeDirection);
                    ChangeBodyPart(CharacterPartType.RightLeg, ref rightLegIndex, changeDirection);
                    ChangeBodyPart(CharacterPartType.LeftLeg, ref leftLegIndex, changeDirection);
                    break;
                default:
                    break;
            }
        }

        private void ChangeBodyPart(CharacterPartType partType, ref int currentPartIndex, int changeDirection)
        {
            int partVariancesCount = characterPartCollections[(int)currentCharacterGender].GetCharacterPartVariancesLength(partType);

            //If there is no variance for the part, turn it off
            if (partVariancesCount <= 0)
            {
                myCharacter.TurnOffCharacterPart(partType);
                return;
            }

            //Calculate the next part index
            int nextPartIndex = (currentPartIndex + changeDirection) % partVariancesCount;
            if (nextPartIndex < 0)
            {
                nextPartIndex = partVariancesCount - 1;
            }

            currentPartIndex = nextPartIndex;
            myCharacter.ChangeCharacterPart(partType, characterPartCollections[(int)currentCharacterGender].GetMesh(partType, currentPartIndex));
        }
    }
}
