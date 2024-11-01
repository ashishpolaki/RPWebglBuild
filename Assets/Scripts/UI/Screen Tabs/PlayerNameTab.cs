using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class PlayerNameTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private Button setPlayerNameBtn;
        [SerializeField] private Button generateRandomNameBtn;
        [SerializeField] private TextMeshProUGUI errorMessageText;
        #endregion

        #region Private Variables
        private int playerNameMaxCharacters = 50;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            setPlayerNameBtn.onClick.AddListener(() => SetPlayerName());
            generateRandomNameBtn.onClick.AddListener(() => GenerateRandomPlayerName());
        }
        private void OnDisable()
        {
            setPlayerNameBtn.onClick.RemoveAllListeners();
            generateRandomNameBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set Player Name 
        /// </summary>
        private async void SetPlayerName()
        {
            errorMessageText.text = string.Empty;

            //Set Player Name
            Func<Task<string>> method = () => UGSManager.Instance.Authentication.SetPlayerNameAsync(playerNameInput.text);
            errorMessageText.text = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show, ScreenTabType.RoleSelection);
        }

        /// <summary>
        /// Generate Random Name for the Player
        /// </summary>
        private async void GenerateRandomPlayerName()
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.GenerateRandomPlayerName();
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show, ScreenTabType.RoleSelection);
        }
        #endregion
    }
}