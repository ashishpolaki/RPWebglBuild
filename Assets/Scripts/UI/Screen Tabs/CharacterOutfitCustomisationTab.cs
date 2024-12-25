using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class CharacterOutfitCustomisationTab : BaseTab
    {
        [SerializeField] private Button submitButton;
        [SerializeField] private Button lowerOutfitButton;
        [SerializeField] private Button upperOutfitButton;
        [SerializeField] private Outline lowerOutfitButtonOutline;
        [SerializeField] private Outline upperOutfitButtonOutline;
        [SerializeField] private OutfitCustomisationUI outfitCustomisationUI;
        [SerializeField] private TextureAndRotateHandler textureAndRotateHandler;


        [Space(10), Header("Character Render Settings")]
        [SerializeField] private Vector3 position;
        [SerializeField] private float scale = 1f;
        [SerializeField] private Camera characterCamera;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private float orthoGraphicSize;
        [SerializeField] private RenderTextureSettings renderTextureSettings;

        private Character character;
        private bool isCharacterLoaded;
        private RenderTexture renderTexture;

        #region Unity Methods
        private void OnEnable()
        {
            submitButton.onClick.AddListener(() => OnSubmitButton());
            lowerOutfitButton.onClick.AddListener(() => OnLowerOutfitButton());
            upperOutfitButton.onClick.AddListener(() => OnUpperOutfitButton());
        }
        private void OnDisable()
        {
            submitButton.onClick.RemoveAllListeners();
            lowerOutfitButton.onClick.RemoveAllListeners();
            upperOutfitButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Override Methods
        public override void Open()
        {
            base.Open();
            OnUpperOutfitButton();
            LoadCharacter();
        }
        #endregion

        #region Private Methods

        private void LoadCharacter()
        {
            if (!isCharacterLoaded)
            {
                isCharacterLoaded = true;
                character = UGSManager.Instance.PlayerData.character;
            }

            //Set Transform
            character.EnableFullBody();
            character.transform.position = position;
            character.transform.eulerAngles = new Vector3(0, 180, 0);
            character.transform.localScale = Vector3.one * scale;

            //Create a new Render Texture and assign it to camera.
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(renderTextureSettings.renderTextureSize.x, renderTextureSettings.renderTextureSize.y, renderTextureSettings.colorFormat, renderTextureSettings.depthStencilFormat);
            }

            //Set Camera Position
            characterCamera.transform.position = character.transform.position + cameraOffset;
            characterCamera.targetTexture = renderTexture;
            characterCamera.orthographicSize = orthoGraphicSize;
            characterCamera.gameObject.SetActive(true);
            textureAndRotateHandler.OnCharacterAssign(renderTexture, character.transform);
        }

        private void SetOutfitButtonOutline(OutfitType outfitType)
        {
            lowerOutfitButtonOutline.enabled = outfitType == OutfitType.Lower;
            upperOutfitButtonOutline.enabled = outfitType == OutfitType.Upper;
        }
        #endregion

        #region Button Listener Methods
        private async void OnSubmitButton()
        {
            await character.Save();
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.CharacterCustomize);
        }
        private void OnLowerOutfitButton()
        {
            outfitCustomisationUI.SetOutfitType(OutfitType.Lower);
            SetOutfitButtonOutline(OutfitType.Lower);
        }
        private void OnUpperOutfitButton()
        {
            outfitCustomisationUI.SetOutfitType(OutfitType.Upper);
            SetOutfitButtonOutline(OutfitType.Upper);
        }
        #endregion

    }
}
