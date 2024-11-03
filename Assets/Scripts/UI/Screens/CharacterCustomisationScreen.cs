using UnityEngine;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        #region Unity Methods
        private void OnEnable()
        {
            OnEnableScreen();
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
                //If player is host then open host screen
                if (UGSManager.Instance.IsHost)
                {
                    UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
                }
                //If player is not host then open client screen
                else
                {
                    UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
                }
                Close();
            }
        }
        #endregion
    }
}
