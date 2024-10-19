using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class ClientScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button checkedInBtn;
        [SerializeField] private Button joinRaceBtn;
        [SerializeField] private Button backButton;
        [SerializeField] private Button raceStatusButton;
        [SerializeField] private TextMeshProUGUI raceStatusText;
        [SerializeField] private TextMeshProUGUI messageText;
        #endregion

        #region Private Variables
        private bool canShowRaceLobby = false;
        private bool canShowRaceResults = false;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            checkedInBtn.onClick.AddListener(async () => await LoadingScreen.Instance.PerformAsyncWithLoading(VenueCheckIn));
            joinRaceBtn.onClick.AddListener(async () => await LoadingScreen.Instance.PerformAsyncWithLoading(EnterRace));
            backButton.onClick.AddListener(() => OnScreenBack());
            raceStatusButton.onClick.AddListener(() => OnRaceStatusHandle());
            UGSManager.Instance.CloudCode.OnRaceStarted += OnRaceStart;
            UGSManager.Instance.CloudCode.OnRaceResult += OnRaceResult;
            PlayerRaceStatus();
        }
        private void OnDisable()
        {
            checkedInBtn.onClick.RemoveAllListeners();
            joinRaceBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
            raceStatusButton.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnRaceStarted -= OnRaceStart;
                UGSManager.Instance.CloudCode.OnRaceResult -= OnRaceResult;
            }
        }
        #endregion

        #region Inherited Methods
        public override void OnScreenBack()
        {
            //If no tab is opened and the back button is pressed, then close this screen.
            if (CurrentOpenTab == ScreenTabType.None)
            {
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
                Close();
                return;
            }
            base.OnScreenBack();

            //If there is no tabs opened, then check the race status.
            if (CurrentOpenTab == ScreenTabType.None)
            {
                PlayerRaceStatus();
            }
        }
        #endregion

        #region Private Methods
        private async void PlayerRaceStatus()
        {
            LoadingScreen.Instance.Show();
            ResetData();
            //Fetch Current Location
            await UGSManager.Instance.FetchCurrentLocation();
            //Get host ID from the currentlocation
            string hostID = await UGSManager.Instance.GetHostID();
            if (!string.IsNullOrEmpty(hostID))
            {
                //Check if the player has previous Race Data.
                canShowRaceResults = await VerifyRaceResults();
                if (!canShowRaceResults)
                {
                    canShowRaceLobby = await VerifyRaceLobby();
                }
            }
            LoadingScreen.Instance.Hide();
        }

        /// <summary>
        /// Reset the data of the screen.
        /// </summary>
        private void ResetData()
        {
            //Set Default Values
            canShowRaceLobby = false;
            canShowRaceResults = false;
            raceStatusButton.gameObject.SetActive(false);
            joinRaceBtn.interactable = true;
            messageText.text = string.Empty;
        }

        private async Task<bool> VerifyRaceLobby()
        {
            //Check if the race lobby data exists. 
            List<UGS.RaceLobbyParticipant> raceLobbyParticipants = await UGSManager.Instance.CloudSave.TryGetRaceLobby(UGSManager.Instance.PlayerData.hostID, StringUtils.RACELOBBY);

            if (raceLobbyParticipants != null)
            {
                int horseNumber = 0;

                //Check if the player is in the ongoing race lobby.
                if (raceLobbyParticipants != null)
                {
                    foreach (var raceLobbyParticipant in raceLobbyParticipants)
                    {
                        if (raceLobbyParticipant.PlayerID == UGSManager.Instance.PlayerData.playerID)
                        {
                            horseNumber = raceLobbyParticipant.HorseNumber;
                            break;
                        }
                    }
                }

                //Set the player's race data and dispose the object after usage.
                using (RaceData raceData = new RaceData())
                {
                    raceData.horseNumber = horseNumber;
                    UGSManager.Instance.SetRaceData(raceData);
                }

                //Check if the player is in race lobby. 
                if (horseNumber != 0)
                {
                    raceStatusText.text = "Continue to Race";
                    raceStatusButton.gameObject.SetActive(true);
                    joinRaceBtn.interactable = false;
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> VerifyRaceResults()
        {
            UGS.PlayerRaceResult raceResult = await UGSManager.Instance.CloudSave.TryGetPlayerRaceResult(UGSManager.Instance.PlayerData.hostID, UGSManager.Instance.PlayerData.playerID, StringUtils.RACERESULT);
            if (raceResult != null)
            {
                //Set the player's race Data and dispose the object after usage.
                using (RaceData raceData = new RaceData())
                {
                    raceData.horseNumber = raceResult.HorseNumber;
                    raceData.racePosition = raceResult.RacePosition;
                    UGSManager.Instance.SetRaceData(raceData);
                }
                raceStatusText.text = "See Race Results";
                raceStatusButton.gameObject.SetActive(true);
                return true;
            }
            return false;
        }
        #endregion

        #region CloudCode Trigger Methods
        private void OnRaceStart(string message)
        {
            using (RaceData raceData = new RaceData())
            {
                raceData.horseNumber = int.Parse(message);
                UGSManager.Instance.SetRaceData(raceData);
            }
            CloseAllTabs();
            OpenTab(ScreenTabType.RaceInProgress);
        }

        private void OnRaceResult(string _raceResult)
        {
            UGS.PlayerRaceResult raceResult = JsonConvert.DeserializeObject<UGS.PlayerRaceResult>(_raceResult);
            using (RaceData raceData = new RaceData())
            {
                raceData.horseNumber = raceResult.HorseNumber;
                raceData.racePosition = raceResult.RacePosition;
                UGSManager.Instance.SetRaceData(raceData);
            }
            CloseAllTabs();
            OpenTab(ScreenTabType.RaceResults);
        }
        #endregion

        #region Button Listener Methods
        /// <summary>
        /// Checks the availability of race results or race lobby data and opens the corresponding UI tab.
        /// </summary>
        private void OnRaceStatusHandle()
        {
            if (canShowRaceResults)
            {
                OpenTab(ScreenTabType.RaceResults);
            }
            else if (canShowRaceLobby)
            {
                OpenTab(ScreenTabType.RaceInProgress);
            }
        }

        /// <summary>
        /// Venue Checkin In Host Location.
        /// </summary>
        private async Task VenueCheckIn()
        {
            await UGSManager.Instance.FetchCurrentLocation();
            //If cheat is enabled, pass cheat datetime to the cloud code.
            string dateTime = string.Empty;
            if (CheatCode.Instance.IsCheatEnabled)
            {
                dateTime = CheatCode.Instance.CheatDateTime;
            }

            //Get host ID from the currentRaceCheckins location
            string hostID = await UGSManager.Instance.GetHostID();
            if (StringUtils.IsStringEmpty(hostID))
            {
                messageText.text = "No venue found at this location";
                return;
            }

            //Check if the host is trying to checkin in its own venue.
            if (hostID == UGSManager.Instance.PlayerData.playerID)
            {
                messageText.text = "Host can't checkin its own venue";
                return;
            }

            //Check if the date time format is valid.
            if (DateTimeUtils.IsValidDateTimeFormat(dateTime) == false)
            {
                messageText.text = "Invalid Date Time Format";
                return;
            }

            //Send VenueCheckIn Request to the Host.
            string checkInMessage = await UGSManager.Instance.CloudCode.VenueCheckIn(hostID, dateTime);
            messageText.text = checkInMessage;
        }

        /// <summary>
        /// Request to the server to enter in to the race.
        /// </summary>
        private async Task EnterRace()
        {
            //Get Current Location
            await UGSManager.Instance.FetchCurrentLocation();

            //If cheat is enabled, pass cheat datetime to the cloud code.
            string dateTime = string.Empty;
            if (CheatCode.Instance.IsCheatEnabled)
            {
                dateTime = CheatCode.Instance.CheatDateTime;
            }

            //Get host ID from the current location.
            string hostID = await UGSManager.Instance.GetHostID();
            if (string.IsNullOrEmpty(hostID) || string.IsNullOrWhiteSpace(hostID))
            {
                messageText.text = "No venue found at this location";
                return;
            }

            //Check if the host is trying to join its own venue.
            if (hostID == UGSManager.Instance.PlayerData.playerID)
            {
                messageText.text = "Host can't join its own venue";
                return;
            }

            //Check if the race lobby data exists. 
            List<UGS.RaceLobbyParticipant> raceLobbyParticipants = await UGSManager.Instance.CloudSave.TryGetRaceLobby(UGSManager.Instance.PlayerData.hostID, StringUtils.RACELOBBY);
            if (raceLobbyParticipants != null)
            {
                messageText.text = "Race is currently in progress. Please wait to join the next race.";
                return;
            }

            //Request Host to Enter the race
            await RequestRaceJoinAsync(hostID, dateTime);
        }

        /// <summary>
        /// Sends a request to the server to join the current race.
        /// </summary>
        /// <param name="hostID"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private async Task RequestRaceJoinAsync(string hostID, string dateTime)
        {
            Func<Task<JoinRaceResponse>> raceJoinResponse = () => UGSManager.Instance.CloudCode.RequestRaceJoin(hostID, dateTime);
            JoinRaceResponse response = await LoadingScreen.Instance.PerformAsyncWithLoading(raceJoinResponse);

            if (response != null)
            {
                //Get the RaceResponse from the server and update the data locally.
                DateTime raceTime = DateTime.Parse(response.RaceTime);
                using (RaceData raceData = new RaceData())
                {
                    raceData.raceTime = raceTime;
                    UGSManager.Instance.SetRaceData(raceData);
                }

                // If the player can wait in the lobby, show the lobby screen.
                if (response.CanWaitInLobby)
                {
                    OpenTab(ScreenTabType.Lobby);
                }
                // Otherwise, display the message explaining why the player couldn't join the race.
                else
                {
                    messageText.text = response.Message;
                }
            }
        }
        #endregion
    }
}
