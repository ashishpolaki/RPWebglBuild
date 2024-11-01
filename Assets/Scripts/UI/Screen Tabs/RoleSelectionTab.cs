using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RoleSelectionTab : BaseTab
    {
        #region Inspector variables
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button joinBtn;
        [SerializeField] private TextMeshProUGUI playerNameTxt;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            hostBtn.onClick.AddListener(() => HostMode());
            joinBtn.onClick.AddListener(() => PlayerMode());
            ShowPlayerName();
        }
        private void OnDisable()
        {
            hostBtn.onClick.RemoveAllListeners();
            joinBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        private void ShowPlayerName()
        {
            playerNameTxt.text = "Player Name : " + UGSManager.Instance.PlayerData.playerName;
        }

        /// <summary>
        /// Join as a player
        /// </summary>
        private void PlayerMode()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
        }

        /// <summary>
        /// Host a game
        /// </summary>
        private void HostMode()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
        }
        #endregion
    }
}