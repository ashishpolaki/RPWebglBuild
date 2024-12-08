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
        [SerializeField] private Vector2 layoutSpace;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
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
            verticalLayoutGroup.spacing = layoutSpace.y;
            errorMessageTxt.gameObject.SetActive(true);
            errorMessageTxt.text = message;
        }
        private void OnValidationFailed(string obj)
        {
            verticalLayoutGroup.spacing = layoutSpace.y;
            errorMessageTxt.gameObject.SetActive(true);
            errorMessageTxt.text = obj;
        }
        /// <summary>
        /// Sign in successful event
        /// </summary>
        private async void SignInSuccessful()
        {
            //Check if the user is a host
            bool isHost = await UGSManager.Instance.CloudSave.IsUserHostAsync();
            using (PlayerData playerData = new PlayerData())
            {
                playerData.isHost = isHost;
                UGSManager.Instance.SetPlayerData(playerData);
            }

            if (isHost)
            {
                UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
                UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
            }
            else
            {
                bool isPlayerNameEmpty = StringUtils.IsStringEmpty(UGSManager.Instance.PlayerData.playerName);
                bool isCharacterNotCustomized = true;

                if (isPlayerNameEmpty)
                {
                    UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.PlayerName);
                }
                else
                {
                    ScreenType screenType = isCharacterNotCustomized ? ScreenType.CharacterCustomisation : ScreenType.Client;
                    UIController.Instance.ScreenEvent(screenType, UIScreenEvent.Open);
                    UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
                }
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
            verticalLayoutGroup.spacing = layoutSpace.x;
            username_Input.text = string.Empty;
            password_Input.text = string.Empty;
            errorMessageTxt.text = string.Empty;
            errorMessageTxt.gameObject.SetActive(false);
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
