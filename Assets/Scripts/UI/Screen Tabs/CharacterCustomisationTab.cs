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
        private int blendPartsInstanceCount = 12;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            CommonEnable();
            FaceCustomisationEnable();
        }
        private void OnDisable()
        {
            CommonDisable();
            FaceCustomisationDisable();
        }
        protected override void Start()
        {
            base.Start();
            ChangeScreen(0);
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

            if (currentScreenIndex == 1)
            {
                OnSelectPart(currentSelectedPartIndex);
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
            //if (currentColorIndex + index < 0 || currentColorIndex + index >= CharacterCustomisationManager.Instance.CurrentPartColorCount)
            //{
            //    return;
            //}
            currentColorIndex += index;
        }
        private void OnChangeCurrentPart(int index)
        {
            //if (currentChangePartIndex + index < 0 || currentChangePartIndex + index >= CharacterCustomisationManager.Instance.CurrentPartVariancesCount)
            //{
            //    return;
            //}
            currentChangePartIndex += index;
        }
        private void OnSelectPart(int index)
        {
            //if (currentSelectedPartIndex + index < 0 || currentSelectedPartIndex + index >= CharacterCustomisationManager.Instance.CharacterPartsLength)
            //{
            //    return;
            //}
            currentChangePartIndex = 0;
            currentColorIndex = 0;
            DisableBlendParts();
            currentSelectedPartIndex += index;
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


    }
}