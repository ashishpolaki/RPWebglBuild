using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class CharacterBodyCustomisationTab : BaseTab
    {
        [SerializeField] private Slider bodyTypeSlider;
        [SerializeField] private Slider bodySizeSlider;
        [SerializeField] private Slider musculatureSlider;
        [SerializeField] private Button nextButton;
        [SerializeField] private ColorPresetSO skinToneColorPreset;
        [SerializeField] private Transform desirePosition;
        [SerializeField] private float scale;

        private Character character;
        private bool isCharacterLoaded;

        #region Unity Methods
        private void OnEnable()
        {
            bodyTypeSlider.onValueChanged.AddListener((value) => OnBodyTypeValueChanged(value));
            bodySizeSlider.onValueChanged.AddListener((value) => OnBodySizeValueChanged(value));
            musculatureSlider.onValueChanged.AddListener((value) => OnMusculatureValueChanged(value));
            nextButton.onClick.AddListener(() => OnNextButton());
        }
        private void OnDisable()
        {
            bodyTypeSlider.onValueChanged.RemoveAllListeners();
            bodySizeSlider.onValueChanged.RemoveAllListeners();
            musculatureSlider.onValueChanged.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        private void OnNextButton()
        {
            character.gameObject.SetActive(false);
            UIController.Instance.ScreenEvent(ScreenType.Client,UIScreenEvent.Open);
            //  UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.CharacterFaceCustomize);
        }
        private void OnBodyTypeValueChanged(float value)
        {
            character.BodyGenderBlendShape(value);
        }
        private void OnBodySizeValueChanged(float value)
        {
            character.BodySizeBlendShape(value);
        }
        private void OnMusculatureValueChanged(float value)
        {
            character.MusculatureBlendShape(value);
        }
        private void SetTransform()
        {
            character.EnableFullBody();
            character.transform.position = desirePosition.position;
            character.transform.localScale = Vector3.one * scale;
        }
        private void SetSkinToneColor()
        {
            character.ChangeSkinToneColor(skinToneColorPreset.colors[0]);
        }
        #endregion


        #region Override Methods
        public override void Open()
        {
            base.Open();
            if (!isCharacterLoaded)
            {
                isCharacterLoaded = true;
                character = UGSManager.Instance.PlayerData.character;
            }
            SetTransform();
        }
        #endregion

    }
}
