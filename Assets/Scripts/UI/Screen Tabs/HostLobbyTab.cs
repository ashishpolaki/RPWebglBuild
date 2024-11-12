using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HostLobbyTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private Button startRace_btn;
        [SerializeField] private LobbyPlayerUI lobbyPlayerUIPrefab;
        [SerializeField] private Transform playersUIContent;
        [SerializeField] private GameObject lobbyPlayersScrollObject;
        [SerializeField] private TextMeshProUGUI messageTxt;
        #endregion

        #region Private Variables
        private List<LobbyPlayerUI> lobbyPlayerUIList = new List<LobbyPlayerUI>();
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            startRace_btn.onClick.AddListener(() => StartRace());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnRaceStartSuccessEvent += OnStartRaceSuccess;
                UGSManager.Instance.CloudCode.OnRaceStartFailEvent += OnStartRaceFail;
            }
            RaceStatus();
        }
        private void OnDisable()
        {
            startRace_btn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnRaceStartSuccessEvent -= OnStartRaceSuccess;
                UGSManager.Instance.CloudCode.OnRaceStartFailEvent -= OnStartRaceFail;
            }
        }
        #endregion

        #region Subscribe Event Methods
        public void OnStartRaceSuccess()
        {
            UnityEngine.Screen.orientation = ScreenOrientation.LandscapeLeft;
            GameManager.Instance.HorsesToSpawnList = UGSManager.Instance.HostRaceData.qualifiedPlayers.Select(x => x.HorseNumber).ToList();
            LoadingScreen.Instance.LoadSceneAdditiveAsync((int)Scene.Race);
        }
        public void OnStartRaceFail(string message)
        {
            Debug.LogError(message);
        }
        #endregion

        private async Task GetRaceCheckInPlayersAsync()
        {
            LoadingScreen.Instance.Show();
            List<CurrentRacePlayerCheckIn> racePlayerCheckIns = await UGSManager.Instance.CloudCode.GetRaceCheckIns();

            //no players checked in
            if (racePlayerCheckIns.Count == 0)
            {
                messageTxt.text = "No Players Checked In";
            }

            //Check Min 2 players to start a race
            else if (racePlayerCheckIns.Count > 1)
            {
                lobbyPlayersScrollObject.gameObject.SetActive(true);
                startRace_btn.interactable = true;
                RaceLobbyHandler raceLobbyHandler = new RaceLobbyHandler(racePlayerCheckIns);
                DisplayRaceCheckins(racePlayerCheckIns);
                List<RaceLobbyParticipant> raceLobbyParticipants = await raceLobbyHandler.GetQualifiedPlayers();
                List<CurrentRacePlayerCheckIn> currentRacePlayerCheckIns = await raceLobbyHandler.GetUnQualifiedPlayers();

                HostRaceData hostRaceData = new HostRaceData();
                hostRaceData.qualifiedPlayers = raceLobbyParticipants;
                hostRaceData.unQualifiedPlayersList = currentRacePlayerCheckIns;
                UGSManager.Instance.SetHostRaceData(hostRaceData);
            }
            LoadingScreen.Instance.Hide();
        }

        private void DisplayRaceCheckins(List<CurrentRacePlayerCheckIn> currentRacePlayerCheckIns)
        {
            foreach (var lobbyPlayer in currentRacePlayerCheckIns)
            {
                var lobbyPlayerUI = Instantiate(lobbyPlayerUIPrefab, playersUIContent);
                lobbyPlayerUI.SetData(lobbyPlayer.PlayerName);
                lobbyPlayerUIList.Add(lobbyPlayerUI);
            }
        }

        #region Private Methods
        private async void RaceStatus()
        {
            ResetData();
            ResetFields();

            await GetRaceCheckInPlayersAsync();
        }
        private async void StartRace()
        {
            Func<Task> response = () => UGSManager.Instance.CloudCode.StartRace(UGSManager.Instance.HostRaceData.qualifiedPlayers, UGSManager.Instance.HostRaceData.unQualifiedPlayersList);
            await LoadingScreen.Instance.PerformAsyncWithLoading(response);
            UnityEngine.Screen.orientation = ScreenOrientation.LandscapeLeft;
            GameManager.Instance.HorsesToSpawnList = UGSManager.Instance.HostRaceData.qualifiedPlayers.Select(x => x.HorseNumber).ToList();
            LoadingScreen.Instance.LoadSceneAdditiveAsync((int)Scene.Race);
        }
        private void ResetData()
        {
            //Clear Data
            for (int i = 0; i < lobbyPlayerUIList.Count; i++)
            {
                Destroy(lobbyPlayerUIList[i].gameObject);
            }
            lobbyPlayerUIList.Clear();
        }
        private void ResetFields()
        {
            startRace_btn.interactable = false;
            lobbyPlayersScrollObject.gameObject.SetActive(false);
            messageTxt.text = string.Empty;
        }
        #endregion
    }
}
