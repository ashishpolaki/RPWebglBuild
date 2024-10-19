using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HostLobbyTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private Button startRace_btn;
        [SerializeField] private TextMeshProUGUI messageTxt;
        [SerializeField] private LobbyPlayerUI lobbyPlayerUIPrefab;
        [SerializeField] private Transform playersUIContent;
        #endregion

        #region Private Variables
        private List<LobbyPlayerUI> lobbyPlayerUIList = new List<LobbyPlayerUI>();
        [Tooltip("Key: PlayerID, Value: (PlayerName, HorseNumber) ")]
        private Dictionary<string, (string, int)> lobbyPlayers = new Dictionary<string, (string, int)>();
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            startRace_btn.onClick.AddListener(() => StartRace());
            if(UGSManager.Instance != null)
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
            GameManager.Instance.HorsesToSpawnList = lobbyPlayers.Select(x => x.Value.Item2).ToList();
            LoadingScreen.Instance.LoadSceneAdditiveAsync((int)Scene.Race);
        }
        public void OnStartRaceFail(string message)
        {
            Debug.LogError(message);
        }
        #endregion

        #region Lobby Players
        private void DisplayLobbyPlayers()
        {
            if (lobbyPlayers.Count > 0)
            {
                foreach (var lobbyPlayer in lobbyPlayers)
                {
                    var lobbyPlayerUI = Instantiate(lobbyPlayerUIPrefab, playersUIContent);
                    lobbyPlayerUI.SetData($" Horse #{lobbyPlayer.Value.Item2}", lobbyPlayer.Value.Item1);
                    lobbyPlayerUIList.Add(lobbyPlayerUI);
                }
            }
        }

        private void ShuffleLobbyPlayersList()
        {
            System.Random random = new System.Random();

            // Get the keys and shuffle them
            var keys = new List<string>(lobbyPlayers.Keys);
            int n = keys.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var temp = keys[k];
                keys[k] = keys[n];
                keys[n] = temp;
            }

            // Rebuild the dictionary with shuffled keys
            var shuffledDictionary = new Dictionary<string, (string, int)>();
            foreach (var key in keys)
            {
                shuffledDictionary[key] = lobbyPlayers[key];
            }

            // Assign the shuffled dictionary back to lobbyPlayers
            lobbyPlayers = shuffledDictionary;
        }
        #endregion

        #region Private Methods
        private void RaceStatus()
        {
            ResetData();

            //Check Race Checkins
            lobbyPlayers = UGSManager.Instance.RaceData.lobbyQualifiedPlayers;
            int lobbyPlayersCount = lobbyPlayers.Count;
            if (lobbyPlayersCount == 0)
            {
                //If no players are checked in, display message and return.
                messageTxt.text = "No Players Checked In";
                startRace_btn.interactable = false;
                return;
            }

            //Display Lobby Players
            ShuffleLobbyPlayersList();
            DisplayLobbyPlayers();

            //Enable PlayerLobbyStatus button if minimum 2 players are checked in.
            startRace_btn.interactable = (lobbyPlayersCount > 1);
        }

        private async void StartRace()
        {
            Func<Task> response = () => UGSManager.Instance.CloudCode.StartRace(UGSManager.Instance.RaceData.lobbyQualifiedPlayers, UGSManager.Instance.RaceData.unQualifiedPlayers);
            await LoadingScreen.Instance.PerformAsyncWithLoading(response);
        }

        private void ResetData()
        {
            //Clear Data
            for (int i = 0; i < lobbyPlayerUIList.Count; i++)
            {
                Destroy(lobbyPlayerUIList[i].gameObject);
            }
            lobbyPlayerUIList.Clear();
            lobbyPlayers.Clear();
            messageTxt.text = string.Empty;
        }
        #endregion
    }
}
