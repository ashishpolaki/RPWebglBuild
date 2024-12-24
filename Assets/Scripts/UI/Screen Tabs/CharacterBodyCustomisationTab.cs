using CharacterCustomisation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class CharacterBodyCustomisationTab : BaseTab
    {
        [SerializeField] private Button submitButton;
        [SerializeField] private ColorPresetSO skinToneColorPreset;
        [SerializeField] private CharacterPartUI[] characterBodyCustomisationDatas;
        [SerializeField] private CharacterPartUIType currentCharacterPartType;
        [SerializeField] private CharacterPartButton characterPartButtonPrefab;
        [SerializeField] private Transform characterPartSelectionParent;

        [SerializeField] private GameObject characterFullBodyBGGameobject;
        [SerializeField] private GameObject characterHeadBGGameobject;
        [SerializeField] private BodyShapeCustomisationUI bodyShapeCustomisationUI;
        [SerializeField] private SkinColorCustomisationUI skinColorCustomisationUI;
        [SerializeField] private CharacterHeadCustomisationUI characterHeadCustomisationUI;
        //  [SerializeField] private 

        private CharacterPartButton[] characterPartButtons;

        #region Unity Methods
        private void OnEnable()
        {
            submitButton.onClick.AddListener(() => OnSubmitButton());
        }
        private void OnDisable()
        {
            submitButton.onClick.RemoveAllListeners();
            CleanupPartSelectionUI();
        }
        protected override void Start()
        {
            base.Start();
            GeneratePartSelectionUI();
        }
        #endregion

        #region Button Listener Methods
        private void OnSubmitButton()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.CharacterOutfitCustomize);
        }
        #endregion

        #region Event Listener Methods
        private void OnCharacterPartSelected(CharacterPartUIType _currentCharacterPartType)
        {
            //Enable outline for Selected Button.
            foreach (CharacterPartButton characterPartButton in characterPartButtons)
            {
                characterPartButton.SetSelected(characterPartButton.CurrentCharacterPartType == _currentCharacterPartType);
            }

            //Deactivate Objects
            bodyShapeCustomisationUI.gameObject.SetActive(false);
            skinColorCustomisationUI.gameObject.SetActive(false);
            characterHeadCustomisationUI.gameObject.SetActive(false);
            characterFullBodyBGGameobject.SetActive(false);
            characterHeadBGGameobject.SetActive(false);

            //Activate the selected part gameobjects
            if (_currentCharacterPartType == CharacterPartUIType.Body_Shape)
            {
                characterFullBodyBGGameobject.SetActive(true);
                bodyShapeCustomisationUI.gameObject.SetActive(true);
            }
            else if (_currentCharacterPartType == CharacterPartUIType.Body_SkinTone)
            {
                characterFullBodyBGGameobject.SetActive(true);
                skinColorCustomisationUI.SetData(skinToneColorPreset);
                skinColorCustomisationUI.gameObject.SetActive(true);
            }
            else
            {
                characterHeadBGGameobject.SetActive(true);
                characterHeadCustomisationUI.gameObject.SetActive(true);
                BlendPartType blendPartType = default;
                switch (_currentCharacterPartType)
                {
                    case CharacterPartUIType.Body_Shape:
                        break;
                    case CharacterPartUIType.Body_SkinTone:
                        break;
                    case CharacterPartUIType.Head_EyeBrows:
                        blendPartType = BlendPartType.Eyebrows;
                        break;
                    case CharacterPartUIType.Head_Ears:
                        blendPartType = BlendPartType.Ears;
                        break;
                    case CharacterPartUIType.Head_Eyes:
                        blendPartType = BlendPartType.Eyes;
                        break;
                    case CharacterPartUIType.Head_FacialHair:
                        blendPartType = BlendPartType.FacialHair;
                        break;
                    case CharacterPartUIType.Head_Hair:
                        blendPartType = BlendPartType.Hair;
                        break;
                    case CharacterPartUIType.Head_Nose:
                        blendPartType = BlendPartType.Nose;
                        break;
                    default:
                        break;
                }
                CharacterPartSO characterPartSO = CharacterCustomisationManager.Instance.GetCharacterPartSO(blendPartType);
                characterHeadCustomisationUI.OnPartSelected(characterPartSO);
            }
        }
        #endregion

        #region Private Methods
        private void GeneratePartSelectionUI()
        {
            characterPartButtons = new CharacterPartButton[characterBodyCustomisationDatas.Length];
            for (int i = 0; i < characterBodyCustomisationDatas.Length; i++)
            {
                CharacterPartButton characterPartButton = Instantiate(characterPartButtonPrefab, characterPartSelectionParent);
                characterPartButton.onCharacterPartButtonClicked += OnCharacterPartSelected;
                characterPartButton.SetData(characterBodyCustomisationDatas[i].characterPartType, characterBodyCustomisationDatas[i].icon);
                characterPartButtons[i] = characterPartButton;
            }

            //Select the first button by default
            OnCharacterPartSelected(characterBodyCustomisationDatas[0].characterPartType);
        }
        private void CleanupPartSelectionUI()
        {
            if (characterPartButtons == null) return;
            foreach (CharacterPartButton characterPartButton in characterPartButtons)
            {
                characterPartButton.onCharacterPartButtonClicked -= OnCharacterPartSelected;
            }
            characterPartButtons = null; // Clear the array
        }
        #endregion
    }
}
