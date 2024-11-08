using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LoginTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TMP_InputField username_Input;
        [SerializeField] private TMP_InputField password_Input;
        [SerializeField] private Button loginBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            ResetTextFields();
            loginBtn.onClick.AddListener(() => Login());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed += OnSignInFailed;
                UGSManager.Instance.Authentication.OnValidationFail += OnValidationFailed;
                UGSManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;
            }
        }
        private void OnDisable()
        {
            loginBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed -= OnSignInFailed;
                UGSManager.Instance.Authentication.OnValidationFail -= OnValidationFailed;
                UGSManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
            }
        }
        #endregion

        #region Subscribed Events
        /// <summary>
        /// Sign in failed event
        /// </summary>
        /// <param name="message"></param>
        private void OnSignInFailed(string message)
        {
            errorMessageTxt.text = message;
        }
        private void OnValidationFailed(string obj)
        {
            errorMessageTxt.text = obj;
        }
        /// <summary>
        /// Sign in successful event
        /// </summary>
        private async void SignInSuccessful()
        {
            //Check if the user is a host
            bool isHost = await UGSManager.Instance.CloudSave.IsUserHostAsync();
            UGSManager.Instance.SetHost(isHost);

            if (UGSManager.Instance.IsHost)
            {
                UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
                UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
            }
            else
            {
                bool isPlayerNameEmpty = StringUtils.IsStringEmpty(UGSManager.Instance.PlayerData.playerName);
                ScreenTabType nextScreenTabType = isPlayerNameEmpty ? ScreenTabType.PlayerName : ScreenTabType.CharacterCustomize;
                UIController.Instance.ChangeCurrentScreenTab(nextScreenTabType);
            }
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
        private void ResetTextFields()
        {
            username_Input.text = string.Empty;
            password_Input.text = string.Empty;
            errorMessageTxt.text = string.Empty;
        }

        /// <summary>
        /// Login Request
        /// </summary>
        private async void Login()
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.SignInWithUsernamePasswordAsync(username_Input.text, password_Input.text);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion
    }
}
