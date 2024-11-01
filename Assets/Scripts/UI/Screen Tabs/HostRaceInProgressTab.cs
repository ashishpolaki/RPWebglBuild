using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HostRaceInProgressTab : BaseTab
    {
        [SerializeField] private Button raceResultsButton;

        private void OnEnable()
        {
            raceResultsButton.onClick.AddListener(() => ShowRaceResults());
        }
        private void OnDisable()
        {
            raceResultsButton.onClick.RemoveAllListeners();
        }
        private async void ShowRaceResults()
        {
            ////Get the horses with race Positions
            //List<int> racePositionHorseNumbers = GameManager.Instance.HorsesInRaceOrderList;

            ////Get the lobby players
            //string lobbyPlayers = await UGSManager.Instance.TryGetRaceParticipant(UGSManager.Instance.GameData.PlayerID);
            //List<UGS.RaceLobbyParticipant> raceLobbyParticipants = JsonConvert.DeserializeObject<List<UGS.RaceLobbyParticipant>>(lobbyPlayers);

            ////Create race result data.
            //UGS.CloudSave.RaceResult raceResult = new UGS.CloudSave.RaceResult();
            //foreach (var raceLobbyParticipant in raceLobbyParticipants)
            //{
            //    raceResult.playerRaceResults.Add(new UGS.CloudSave.PlayerRaceResult
            //    {
            //        PlayerID = raceLobbyParticipant.PlayerID,
            //        HorseNumber = raceLobbyParticipant.HorseNumber,
            //        RacePosition = racePositionHorseNumbers.IndexOf(raceLobbyParticipant.HorseNumber) + 1
            //    });
            //}
            
            ////Upload race results in cloud
            //string raceResultSerialize = JsonConvert.SerializeObject(raceResult);
            //Func<Task> raceResultResponse = () => UGSManager.Instance.CloudCode.SendRaceResults(raceResultSerialize);
            //await LoadingScreen.Instance.PerformAsyncWithLoading(raceResultResponse);

            ////Change the screen after uploading results in cloud.
            //UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Close);
            //UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
        }
    }
}