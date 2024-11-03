using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class PlayerNameTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TMP_InputField playerFirstNameInput;
        [SerializeField] private TMP_InputField playerLastNameInput;
        [SerializeField] private Button setPlayerNameBtn;
        [SerializeField] private Button signOutBtn;
        [SerializeField] private TextMeshProUGUI errorMessageText;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            setPlayerNameBtn.onClick.AddListener(() => SetPlayerName());
            signOutBtn.onClick.AddListener(() => SignOut());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedOut += OnSignedOutEvent;
            }
        }
        private void OnDisable()
        {
            setPlayerNameBtn.onClick.RemoveAllListeners();
            signOutBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedOut -= OnSignedOutEvent;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set Player Name 
        /// </summary>
        private async void SetPlayerName()
        {
            errorMessageText.text = string.Empty;

            if (StringUtils.IsStringEmpty(playerFirstNameInput.text))
            {
                errorMessageText.text = StringUtils.PLAYERFIRSTNAMEERROR;
            }
            if (StringUtils.IsStringEmpty(playerLastNameInput.text))
            {
                errorMessageText.text = StringUtils.PLAYERLASTNAMEERROR;
            }
            string playerName = playerFirstNameInput.text + "" + playerLastNameInput.text;
            //Check if player playerName meets the criteria.
            if (!CheckPlayerNameCriteria(playerName))
            {
                errorMessageText.text = StringUtils.PLAYERNAMEERROR;
                return;
            }

            //Set Player Name
            Func<Task<string>> method = () => UGSManager.Instance.Authentication.SetPlayerNameAsync(playerName);
            errorMessageText.text = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
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
        public bool CheckPlayerNameCriteria(string playerName)
        {
            return new Regex(StringUtils.PLAYERNAMEPATTERN).IsMatch(playerName);
        }
        #endregion
    }
}