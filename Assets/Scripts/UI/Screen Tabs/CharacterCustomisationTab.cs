using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class CharacterCustomisationTab : BaseTab
    {
        [SerializeField] private Button submitButton;
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
        }
        private void OnDisable()
        {
            submitButton.onClick.RemoveAllListeners();
        }
        #endregion
        #region Button Listener Methods
        private void OnSubmitButton()
        {
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomisation, UIScreenEvent.Close);
        }

        #endregion
        #region Override Methods
        public override void Open()
        {
            base.Open();
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
        #endregion
    }
}