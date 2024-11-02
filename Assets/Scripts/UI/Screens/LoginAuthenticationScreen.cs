using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class LoginAuthenticationScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private ToggleUI hostToggle;
        [SerializeField] private Image backGroundImage;

        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            UGSManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;
        }
        private void OnDisable()
        {
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
            }
        }
        private void Start()
        {
            hostToggle.SetThemeColor();
            backGroundImage.sprite = UIController.Instance.CurrentTheme.backGround;
            backGroundImage.color = UIController.Instance.CurrentTheme.backGroundTintColor;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sign in successful event
        /// </summary>
        private void SignInSuccessful()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
            Close();
        }
        #endregion
    }
}