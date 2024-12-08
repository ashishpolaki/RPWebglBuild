using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class CharacterFaceCustomisationTab : BaseTab
    {
        [SerializeField] private Transform desirePosition;
        [SerializeField] private float scale;
        [SerializeField] private Button nextButton;

        private Character character;
        private bool isCharacterLoaded;

        #region Private Methods
        private void SetTransform()
        {
            character.EnableFace();
            character.transform.position = desirePosition.position;
            character.transform.localScale = Vector3.one * scale;
        }
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            nextButton.onClick.AddListener(() => OnNextButton());
        }
        private void OnDisable()
        {
            nextButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        private void OnNextButton()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.CharacterFaceCustomize);
        }
        private void OnChangePart()
        {

        }
        private void OnChangeColor()
        {

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