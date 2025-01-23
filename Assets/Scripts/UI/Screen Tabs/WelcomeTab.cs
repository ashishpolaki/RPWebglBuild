using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class WelcomeTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private Button registerTabBtn;
        [SerializeField] private Button loginTabBtn;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            //Button Click Events
            registerTabBtn.onClick.AddListener(() => OpenRegisterTab());
            loginTabBtn.onClick.AddListener(() => OpenLoginTab());
            UGSManager.Instance.Authentication.OnSignedInEvent += OnSignInSuccess;
            UGSManager.Instance.Authentication.OnSignInFailed += OnSignFailed;
        }
        private void OnDisable()
        {
            //Button Click Events
            registerTabBtn.onClick.RemoveAllListeners();
            loginTabBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance.Authentication != null)
            {
                UGSManager.Instance.Authentication.OnSignedInEvent -= OnSignInSuccess;
                UGSManager.Instance.Authentication.OnSignInFailed -= OnSignFailed;
            }
        }
        protected override void Start()
        {
            base.Start();
            CheckCacheSignIn();
        }
        #endregion

        #region Subscribed Methods
        private async void OnSignInSuccess()
        {
            //Check if the user is a host
            bool isHost = await UGSManager.Instance.CloudSave.IsUserHostAsync();
            using (PlayerData playerData = new PlayerData())
            {
                playerData.isHost = isHost;
                UGSManager.Instance.SetPlayerData(playerData);
            }

            //Hide loading screen after checking if the user is a host or not.
            LoadingScreen.Instance.Hide();

            OpenScreen();
        }
        private void OnSignFailed(string message)
        {
            LoadingScreen.Instance.Hide();
            Debug.Log(message);
        }
        #endregion

        #region Private Methods
        private void OpenScreen()
        {
            //Open Host Screen
            if (UGSManager.Instance.PlayerData.isHost)
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
        private async void CheckCacheSignIn()
        {
            //Check if the user is active.
            if (UGSManager.Instance.Authentication.IsCurrentlySignedIn())
            {
                OpenScreen();
                return;
            }

            //Check if user is already signed in past
            if (UGSManager.Instance.Authentication.IsSignInCached())
            {
                LoadingScreen.Instance.Show();
                await UGSManager.Instance.Authentication.CacheSignInAsync();
            }
        }
        private void OpenLoginTab()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.LoginPlayer);
        }
        private void OpenRegisterTab()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.RegisterPlayer);
        }
        #endregion
    }
}


