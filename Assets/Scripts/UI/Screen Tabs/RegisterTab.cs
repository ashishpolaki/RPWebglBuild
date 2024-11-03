using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RegisterTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TMP_InputField username_Input;
        [SerializeField] private TMP_InputField newPassword_Input;
        [SerializeField] private TMP_InputField confirmPassword_Input;
        [SerializeField] private Button registerPlayer_btn;
        [SerializeField] private TextMeshProUGUI errorMessage_txt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            registerPlayer_btn.onClick.AddListener(() => RegisterPlayer());
            ClearInputFields();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed += OnSignUpFailed;
                UGSManager.Instance.Authentication.OnValidationFail += OnValidationFailed;
                UGSManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;
            }
        }
        private void OnDisable()
        {
            registerPlayer_btn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed -= OnSignUpFailed;
                UGSManager.Instance.Authentication.OnValidationFail -= OnValidationFailed;
                UGSManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
            }
        }
        #endregion

        #region Subscribed Events
        /// <summary>
        /// Sign up failed event, Show error message
        /// </summary>
        /// <param name="message"></param>
        private void OnSignUpFailed(string message)
        {
            errorMessage_txt.text = message;
        }

        private void OnValidationFailed(string obj)
        {
            errorMessage_txt.text = obj;
        }

        /// <summary>
        /// Sign in successful event
        /// </summary>
        private void SignInSuccessful()
        {
            UGSManager.Instance.CloudSave.SetHost(UGSManager.Instance.IsHost);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
        }
        #endregion

        #region Protected Methods
        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.Welcome);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reset UI Fields
        /// </summary>
        private void ClearInputFields()
        {
            username_Input.text = string.Empty;
            newPassword_Input.text = string.Empty;
            confirmPassword_Input.text = string.Empty;
            errorMessage_txt.text = string.Empty;
        }

        /// <summary>
        /// Check Username and Password Criteria and Sign up
        /// </summary>
        private async void RegisterPlayer()
        {
            if(!IsConfirmPasswordValid())
            {
                return;
            }
            Func<Task> method = () => UGSManager.Instance.Authentication.SignUpAsync(username_Input.text, newPassword_Input.text);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }

        private bool IsConfirmPasswordValid()
        {
            if (newPassword_Input.text != confirmPassword_Input.text)
            {
                errorMessage_txt.text = StringUtils.PASSWORDMATCHERROR;
                return false;
            }
            return true;
        }
        #endregion
    }
}
