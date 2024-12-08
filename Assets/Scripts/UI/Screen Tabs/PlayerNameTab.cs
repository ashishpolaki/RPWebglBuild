using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
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
        [SerializeField] private Vector2 layoutSpace;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
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
                verticalLayoutGroup.spacing = layoutSpace.y;
                errorMessageText.gameObject.SetActive(true);
                errorMessageText.text = StringUtils.PLAYERFIRSTNAMEERROR;
            }
            if (StringUtils.IsStringEmpty(playerLastNameInput.text))
            {
                verticalLayoutGroup.spacing = layoutSpace.y;
                errorMessageText.gameObject.SetActive(true);
                errorMessageText.text = StringUtils.PLAYERLASTNAMEERROR;
            }
            string playerName = playerFirstNameInput.text + "" + playerLastNameInput.text;
            //Check if player playerName meets the criteria.
            if (!CheckPlayerNameCriteria(playerName))
            {
                verticalLayoutGroup.spacing = layoutSpace.y;
                errorMessageText.gameObject.SetActive(true);
                errorMessageText.text = StringUtils.PLAYERNAMEERROR;
                return;
            }

            //Set Player Name
            Func<Task<string>> method = () => UGSManager.Instance.Authentication.SetPlayerNameAsync(playerName);
            string message = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            if (!StringUtils.IsStringEmpty(message))
            {
                errorMessageText.gameObject.SetActive(true);
                errorMessageText.text = message;
            }
            else
            {
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomisation, UIScreenEvent.Open);
                UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close);
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
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.Welcome);
            Close();
        }
        public bool CheckPlayerNameCriteria(string playerName)
        {
            return new Regex(StringUtils.PLAYERNAMEPATTERN).IsMatch(playerName);
        }
        #endregion
    }
}