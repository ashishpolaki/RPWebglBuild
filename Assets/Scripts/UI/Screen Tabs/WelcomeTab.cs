using System;
using System.Threading.Tasks;
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
        }
        private void OnDisable()
        {
            //Button Click Events
            registerTabBtn.onClick.RemoveAllListeners();
            loginTabBtn.onClick.RemoveAllListeners();
        }
        protected override void Start()
        {
            base.Start();
            CheckCacheSignIn();
        }
        #endregion


        #region Private Methods
        private async void CheckCacheSignIn()
        {
            //Check if user is already signed in
            if (UGSManager.Instance.Authentication.IsSignInCached())
            {
                LoadingScreen.Instance.Show();
                await UGSManager.Instance.Authentication.CacheSignInAsync();
                //Check if the user is a host
                bool isHost = await UGSManager.Instance.CloudSave.IsHost();
                UGSManager.Instance.SetHost(isHost);
                LoadingScreen.Instance.Hide();
                OnSignInSuccess();
            }
        }

        private void OnSignInSuccess()
        {
            //Open New Screen
            if (StringUtils.IsStringEmpty(UGSManager.Instance.Authentication.PlayerName))
            {
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
            }
            else
            {
                ScreenType screenType = UGSManager.Instance.IsHost ? ScreenType.Host : ScreenType.Client;
                UIController.Instance.ScreenEvent(screenType, UIScreenEvent.Open);
            }

            //Close Current Screen
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
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


