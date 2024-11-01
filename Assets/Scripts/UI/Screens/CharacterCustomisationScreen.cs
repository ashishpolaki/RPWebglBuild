using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button signOutBtn;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            signOutBtn.onClick.AddListener(() => SignOut());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedOut += OnSignedOutEvent;
            }
            OnEnableScreen();
        }
        private void OnDisable()
        {
            signOutBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedOut -= OnSignedOutEvent;
            }
        }
        #endregion

        #region Private Methods
        private void OnEnableScreen()
        {
            //If player name is empty then open player name tab
            if (string.IsNullOrEmpty(UGSManager.Instance.PlayerData.playerName))
            {
                OpenTab(ScreenTabType.PlayerName);
            }
            else
            {
                //Open Role Selection Tab
                OpenTab(ScreenTabType.RoleSelection);
            }
        }
        private void SignOut()
        {
            UGSManager.Instance.Authentication.Signout();
        }
        /// <summary>
        /// Clear all the data and open login screen
        /// </summary>
        private void OnSignedOutEvent()
        {
            UGSManager.Instance.ResetData();
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
            Close();
        }
        #endregion
    }
}
