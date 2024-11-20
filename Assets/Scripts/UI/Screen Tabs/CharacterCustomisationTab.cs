using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using CharacterCustomisation;
using TMPro;

namespace UI.Screen.Tab
{
    public class CharacterCustomisationTab : BaseTab
    {
        #region Inspector variables
        [Space(5), Header("Common")]
        [SerializeField] private Button setCharacterButton;
        [SerializeField] private Button previousScreenButton;
        [SerializeField] private Button nextScreenButton;
        [SerializeField] private List<GameObject> screens = new List<GameObject>();
        private int currentScreenIndex = 0;

        [Space(5), Header("FullBodyCustomisation")]
        [SerializeField] private Slider bodyTypeSlider;
        [SerializeField] private Slider bodySizeSlider;
        [SerializeField] private Slider musculatureSlider;
        [SerializeField] private Button skinTonePreviousColor;
        [SerializeField] private Button skinToneNextColor;
        [SerializeField] private ColorPresetSO skinToneColorPreset;
        [SerializeField] private Transform characterPositionFullBody;
        [SerializeField] private Vector3 characterScaleFullBody = new Vector3(500, 500, 500);
        private int skinToneColorIndex = 0;

        [Space(5), Header("FaceCustomisation")]
        [SerializeField] private Transform characterPositionFace;
        [SerializeField] private Vector3 characterScaleFace;
        [SerializeField] private GameObject changeColorObject;
        [SerializeField] private GameObject changePartObject;
        [SerializeField] private GameObject blendPartTxtObject;
        [SerializeField] private GameObject blendPartScrollObject;
        [SerializeField] private Transform blendScrollContent;
        [SerializeField] private BlendPartUI blendPartPrefab;
        [SerializeField] private Button selectPartPrevButton;
        [SerializeField] private Button selectPartNextButton;
        [SerializeField] private Button changeColorPrevButton;
        [SerializeField] private Button changeColorNextButton;
        [SerializeField] private Button changePartPrevButton;
        [SerializeField] private Button changePartNextButton;
        [SerializeField] private TextMeshProUGUI selectPartTxt;
        private CharacterPartSO CharacterPartSO;
        private int currentSelectedPartIndex = 0;
        private int currentColorIndex = 0;
        private int currentChangePartIndex = 0;
        private List<BlendPartUI> blendPartList = new List<BlendPartUI>();
        private int blendPartsInstanceCount = 5;

        [Space(5), Header("OutfitCustomisation")]
        [SerializeField] private Transform outfitCharacterPosition;
        [SerializeField] private Vector3 outfitCharacterScale;
        [SerializeField] private Button upperOutfitPreviousButton;
        [SerializeField] private Button upperOutfitNextButton;
        [SerializeField] private Button lowerOutfitPreviousButton;
        [SerializeField] private Button lowerOutfitNextButton;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            CommonEnable();
            FullBodyCustomisationEnable();
            OutfitCustomisationEnable();
            FaceCustomisationEnable();
        }
        private void OnDisable()
        {
            CommonDisable();
            FullBodyCustomisationDisable();
            OutfitCustomisationDisable();
            FaceCustomisationDisable();
        }
        protected override void Start()
        {
            base.Start();
            OnChangeSkinTone(0);
            ChangeScreen(0);
            CharacterCustomisationManager.Instance.ChangeUpperOutfit(0);
            CharacterCustomisationManager.Instance.ChangeLowerOutfit(0);
        }
        #endregion

        #region Common
        private void CommonEnable()
        {
            setCharacterButton.onClick.AddListener(() => SetCharacterDataASync());
            nextScreenButton.onClick.AddListener(() => ChangeScreen(1));
            previousScreenButton.onClick.AddListener(() => ChangeScreen(-1));
        }
        private void CommonDisable()
        {
            setCharacterButton.onClick.RemoveAllListeners();
            nextScreenButton.onClick.RemoveAllListeners();
            previousScreenButton.onClick.RemoveAllListeners();
        }
        private void SetCharacterDataASync()
        {
            CharacterCustomisationManager.Instance.SaveCharacterData();
        }
        private void ChangeScreen(int value)
        {
            if (currentScreenIndex + value < 0 || currentScreenIndex + value >= screens.Count)
            {
                return;
            }
            screens[currentScreenIndex].SetActive(false);
            currentScreenIndex += value;
            screens[currentScreenIndex].SetActive(true);

            if (currentScreenIndex == 0)
            {
                CharacterCustomisationManager.Instance.SetPosition(characterPositionFullBody.position);
                CharacterCustomisationManager.Instance.SetScale(characterScaleFullBody);
                CharacterCustomisationManager.Instance.EnableFullBody();
            }

            if (currentScreenIndex == 1)
            {
                CharacterCustomisationManager.Instance.SetPosition(characterPositionFace.position);
                CharacterCustomisationManager.Instance.SetScale(characterScaleFace);
                CharacterCustomisationManager.Instance.EnableFace();
                OnSelectPart(currentSelectedPartIndex);
            }

            if (currentScreenIndex == 2)
            {
                CharacterCustomisationManager.Instance.SetPosition(outfitCharacterPosition.position);
                CharacterCustomisationManager.Instance.SetScale(outfitCharacterScale);
                CharacterCustomisationManager.Instance.EnableFullBody();
            }

            UpdateButtonsStatus();
        }

        private void UpdateButtonsStatus()
        {
            bool isLastScreen = currentScreenIndex == screens.Count - 1;
            nextScreenButton.gameObject.SetActive(!isLastScreen);
            setCharacterButton.gameObject.SetActive(isLastScreen);
        }
        #endregion

        #region FullBody Customisation
        private void FullBodyCustomisationEnable()
        {
            bodyTypeSlider.onValueChanged.AddListener((value) => OnBodyTypeValueChanged(value));
            bodySizeSlider.onValueChanged.AddListener((value) => OnBodySizeValueChanged(value));
            musculatureSlider.onValueChanged.AddListener((value) => OnMusculatureValueChanged(value));
            skinTonePreviousColor.onClick.AddListener(() => OnChangeSkinTone(-1));
            skinToneNextColor.onClick.AddListener(() => OnChangeSkinTone(1));
        }
        private void FullBodyCustomisationDisable()
        {
            bodyTypeSlider.onValueChanged.RemoveAllListeners();
            bodySizeSlider.onValueChanged.RemoveAllListeners();
            musculatureSlider.onValueChanged.RemoveAllListeners();
            skinTonePreviousColor.onClick.RemoveAllListeners();
            skinToneNextColor.onClick.RemoveAllListeners();
        }

        private void OnChangeSkinTone(int value)
        {
            if (skinToneColorIndex + value < 0 || skinToneColorIndex + value >= skinToneColorPreset.colors.Length)
            {
                return;
            }
            skinToneColorIndex += value;
            Color color = skinToneColorPreset.colors[skinToneColorIndex];
            CharacterCustomisationManager.Instance.ChangeSkinToneColor(color);
        }

        private void OnBodyTypeValueChanged(float value)
        {
            CharacterCustomisationManager.Instance.OnBodyTypeValueChange(value);
        }

        private void OnBodySizeValueChanged(float value)
        {
            CharacterCustomisationManager.Instance.OnBodySizeValueChange(value);
        }

        private void OnMusculatureValueChanged(float value)
        {
            CharacterCustomisationManager.Instance.OnMusculatureValueChange(value);
        }
        #endregion

        #region Face Customisation
        private void FaceCustomisationEnable()
        {
            selectPartPrevButton.onClick.AddListener(() => OnSelectPart(-1));
            selectPartNextButton.onClick.AddListener(() => OnSelectPart(1));
            changeColorPrevButton.onClick.AddListener(() => OnChangePartColor(-1));
            changeColorNextButton.onClick.AddListener(() => OnChangePartColor(1));
            changePartPrevButton.onClick.AddListener(() => OnChangeCurrentPart(-1));
            changePartNextButton.onClick.AddListener(() => OnChangeCurrentPart(1));
        }

        private void FaceCustomisationDisable()
        {
            selectPartPrevButton.onClick.RemoveAllListeners();
            selectPartNextButton.onClick.RemoveAllListeners();
            changeColorPrevButton.onClick.RemoveAllListeners();
            changeColorNextButton.onClick.RemoveAllListeners();
            changePartPrevButton.onClick.RemoveAllListeners();
            changePartNextButton.onClick.RemoveAllListeners();
        }
        private void OnChangePartColor(int index)
        {
            if (currentColorIndex + index < 0 || currentColorIndex + index >= CharacterCustomisationManager.Instance.CurrentPartColorCount)
            {
                return;
            }
            currentColorIndex += index;
            CharacterCustomisationManager.Instance.ChangePartColor(currentColorIndex);
        }
        private void OnChangeCurrentPart(int index)
        {
            if (currentChangePartIndex + index < 0 || currentChangePartIndex + index >= CharacterCustomisationManager.Instance.CurrentPartVariancesCount)
            {
                return;
            }
            currentChangePartIndex += index;
            CharacterCustomisationManager.Instance.ChangePart(currentChangePartIndex);
        }
        private void OnSelectPart(int index)
        {
            if (currentSelectedPartIndex + index < 0 || currentSelectedPartIndex + index >= CharacterCustomisationManager.Instance.CharacterPartsLength)
            {
                return;
            }
            currentChangePartIndex = 0;
            currentColorIndex = 0;
            DisableBlendParts();
            currentSelectedPartIndex += index;
            CharacterPartSO = CharacterCustomisationManager.Instance.GetCharacterPart(currentSelectedPartIndex);
            ShowCurrentBlendShapePart(CharacterPartSO);
        }

        private void ShowCurrentBlendShapePart(CharacterPartSO characterPart)
        {
            changeColorObject.gameObject.SetActive(characterPart.colorPreset);
            changePartObject.gameObject.SetActive(characterPart.parts.Length > 0);
            blendPartTxtObject.gameObject.SetActive(characterPart.blendPartData.partData.Length > 0);
            blendPartScrollObject.gameObject.SetActive(characterPart.blendPartData.partData.Length > 0);
            selectPartTxt.text = characterPart.partName;

            foreach (var blendShapePartData in characterPart.blendPartData.partData)
            {
                BlendPartUI blendPart = GetBlendPart();
                blendPart.SetData(blendShapePartData);
                blendPart.gameObject.SetActive(true);
            }
        }

        private void InitializeBlendPartList()
        {
            for (int i = 0; i < blendPartsInstanceCount; i++)
            {
                BlendPartUI blendPart = Instantiate(blendPartPrefab, blendScrollContent);
                blendPartList.Add(blendPart);
                blendPartList[i].gameObject.SetActive(false);
            }
        }
        private BlendPartUI GetBlendPart()
        {
            BlendPartUI blendPartUI = null;
            foreach (var item in blendPartList)
            {
                if (!item.gameObject.activeSelf)
                {
                    blendPartUI = item;
                    break;
                }
            }
            if (blendPartUI == null)
            {
                InitializeBlendPartList();
                return GetBlendPart();
            }
            return blendPartUI;
        }
        private void DisableBlendParts()
        {
            foreach (var item in blendPartList)
            {
                if (item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region Outfit Customisation
        private void OutfitCustomisationEnable()
        {
            lowerOutfitNextButton.onClick.AddListener(() => CharacterCustomisationManager.Instance.ChangeLowerOutfit(1));
            lowerOutfitPreviousButton.onClick.AddListener(() => CharacterCustomisationManager.Instance.ChangeLowerOutfit(-1));

            upperOutfitNextButton.onClick.AddListener(() => CharacterCustomisationManager.Instance.ChangeUpperOutfit(1));
            upperOutfitPreviousButton.onClick.AddListener(() => CharacterCustomisationManager.Instance.ChangeUpperOutfit(-1));
        }

        private void OutfitCustomisationDisable()
        {
            lowerOutfitNextButton.onClick.RemoveAllListeners();
            lowerOutfitPreviousButton.onClick.RemoveAllListeners();

            upperOutfitNextButton.onClick.RemoveAllListeners();
            upperOutfitPreviousButton.onClick.RemoveAllListeners();
        }
        #endregion

    }
}