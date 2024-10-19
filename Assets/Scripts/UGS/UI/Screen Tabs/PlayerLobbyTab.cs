using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class PlayerLobbyTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI displayTxt;
        [SerializeField] private Button raceCheckInBtn;
        #endregion

        #region Private Variables
        private TimeSpan timeLeft = new TimeSpan();
        private Coroutine coroutine;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            raceCheckInBtn.onClick.AddListener(() => ConfirmRaceCheckIn());
            PlayerLobbyStatus();
        }
        private void OnDisable()
        {
            raceCheckInBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Check the player lobby status and update the UI accordingly.
        /// </summary>
        private void PlayerLobbyStatus()
        {
            UpdateTimeLeft();
            StartCountDown();
            VerifyRaceCheckIn();
        }

        //Check if player already checkin for the race 
        private async void VerifyRaceCheckIn()
        {
            Func<Task<bool>> response = () => UGSManager.Instance.CloudSave.IsPlayerAlreadyCheckIn(UGSManager.Instance.PlayerData.hostID, UGSManager.Instance.PlayerData.playerID, StringUtils.RACECHECKIN);
            bool isCheckedInAlready = await LoadingScreen.Instance.PerformAsyncWithLoading(response);
            raceCheckInBtn.interactable = !isCheckedInAlready;
        }

        /// <summary>
        /// Update how much time is left for a race.
        /// </summary>
        private void UpdateTimeLeft()
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                DateTime currentDateTime = DateTime.UtcNow.Add(DateTime.Parse(CheatCode.Instance.CheatDateTime) - DateTime.UtcNow);
                timeLeft = UGSManager.Instance.RaceData.raceTime - currentDateTime;
            }
            else
            {
                timeLeft = UGSManager.Instance.RaceData.raceTime - DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Start the countdown timer for the race. 
        /// </summary>
        private void StartCountDown()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(IEStartCountDown());
        }

        /// <summary>
        ///  Updates the display text with the remaining time until the race starts.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEStartCountDown()
        {
            while (timeLeft.TotalSeconds >= 0)
            {
                displayTxt.text = $"Race Starts in : \n {timeLeft.Hours.ToString("D2")}:{timeLeft.Minutes.ToString("D2")}:{timeLeft.Seconds.ToString("D2")}";
                yield return new WaitForSecondsRealtime(1f);
                timeLeft = timeLeft.Add(TimeSpan.FromSeconds(-1));
            }
            displayTxt.text = "Race Will Start Soon";
        }
        #endregion

        #region Button Listener Methods
        /// <summary>
        /// Confirm the player's race check-in for the current race.
        /// </summary>
        private async void ConfirmRaceCheckIn()
        {
            Func<Task<bool>> confirmCheckinResponse = () => UGSManager.Instance.CloudCode.ConfirmRaceCheckIn(UGSManager.Instance.PlayerData.hostID, UGSManager.Instance.PlayerData.playerName);
            bool isCheckedIn = await LoadingScreen.Instance.PerformAsyncWithLoading(confirmCheckinResponse);
            if (isCheckedIn)
            {
                raceCheckInBtn.interactable = false;
            }
        }
        #endregion
    }
}