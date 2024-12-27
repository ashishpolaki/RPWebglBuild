using HorseRace.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class RaceResultsScreen : BaseScreen
    {
        private int minWinnersCount = 3;

        #region Inspector Variables
        [SerializeField] private Button gotoHomeButton;
        [SerializeField] private List<RaceWinnerUIBoard> raceWinnerUIBoardList;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            gotoHomeButton.onClick.AddListener(Home);
        }
        private void OnDisable()
        {
            gotoHomeButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Inherited Methods
        public override async void Open(ScreenTabType screenTabType)
        {
            base.Open(screenTabType);

            await ShowRaceResults();
            int count = Mathf.Min(GameManager.Instance.HorsesInRaceOrderList.Count, minWinnersCount);
            for (int i = 0; i < count; i++)
            {
                int horseNumber = GameManager.Instance.HorsesInRaceOrderList[i];
                raceWinnerUIBoardList[i].SetRaceWinner(horseNumber, UGSManager.Instance.HostRaceData.currentRaceAvatars[horseNumber]);
            }
            UGSManager.Instance.HostRaceData.Dispose();
            UGSManager.Instance.SetHostRaceData(new HostRaceData(),true);
        }
        #endregion

        #region Button Listener Methods
        private void Home()
        {
            Action action = () =>
            {
                UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
            };
            UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
            //Change the screen orientation to Portrait, after uploading the results in cloud.
            LoadingScreen.Instance.LoadSceneAdditiveAsync((int)Scene.Menu, action);
        }
        #endregion

        private async Task ShowRaceResults()
        {
            //Get the horses with race Positions
            List<int> racePositionHorseNumbers = GameManager.Instance.HorsesInRaceOrderList;

            //Get the lobby players
            //List<RaceLobbyParticipant> qualifiedPlayers = new List<RaceLobbyParticipant>();
            //qualifiedPlayers.Add(new RaceLobbyParticipant() { HorseNumber = GameManager.Instance.HorsesInPreRaceOrderList[0], PlayerName = "NareshReddy", PlayerID = "nVTTUdMqqjqnmoJhFrid0Yh2mt7I" });
            //qualifiedPlayers.Add(new RaceLobbyParticipant() { HorseNumber = GameManager.Instance.HorsesInPreRaceOrderList[1], PlayerName = "AjithReddy", PlayerID = "dvWxMXzQv0kHYagsTvbqVLflrzaT" });
            List<RaceLobbyParticipant> raceLobbyParticipants = UGSManager.Instance.HostRaceData.qualifiedPlayers;

            //Set Race Result data.
            RaceResult raceResult = new RaceResult();
            foreach (var raceLobbyParticipant in raceLobbyParticipants)
            {
                raceResult.playerRaceResults.Add(new PlayerRaceResult
                {
                    PlayerID = raceLobbyParticipant.PlayerID,
                    HorseNumber = raceLobbyParticipant.HorseNumber,
                    RacePosition = racePositionHorseNumbers.IndexOf(raceLobbyParticipant.HorseNumber) + 1
                });
            }

            //Upload race results in cloud
            Func<Task> raceResultResponse = () => UGSManager.Instance.CloudCode.SendRaceResults(raceResult);
            await LoadingScreen.Instance.PerformAsyncWithLoading(raceResultResponse);
        }
    }
}