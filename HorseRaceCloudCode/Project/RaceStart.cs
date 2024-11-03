using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class RaceStart
    {
        private readonly IGameApiClient gameApiClient;
        private readonly IPushClient pushClient;
        private readonly ILogger<RaceStart> _logger;

        public RaceStart(IGameApiClient _gameApiClient, IPushClient _pushClient, ILogger<RaceStart> logger)
        {
            this.gameApiClient = _gameApiClient;
            this.pushClient = _pushClient;
            this._logger = logger;
        }

        [CloudCodeFunction("StartRace")]
        public async Task<StartRaceResponse> StartRace(IExecutionContext context, StartRaceRequest startRaceRequest)
        {
            StartRaceResponse response = new StartRaceResponse();

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                response.Message = "Invalid Player ID";
                return response;
            }

            if (startRaceRequest == null)
            {
                response.Message = "Invalid Request";
                return response;
            }

            if (CheckLobbyPlayers(startRaceRequest.RaceLobbyParticipants.Count, out string lobbyPlayersErrorMessage) == false)
            {
                response.Message = lobbyPlayersErrorMessage;
                return response;
            }

            //Start Race
            await SetLobbyPlayers(context, startRaceRequest.RaceLobbyParticipants);
            SetUnQualifiedPlayers(context, startRaceRequest.UnQualifiedPlayerIDs);
            response.IsRaceStart = true;
            return response;
        }

        public bool CheckLobbyPlayers(int lobbyPlayersCount, out string lobbyPlayersErrorMessage)
        {
            lobbyPlayersErrorMessage = "";
            if (lobbyPlayersCount > HostConfig.maxPlayersInLobby)
            {
                lobbyPlayersErrorMessage = "Exceeded Maximum Players in Lobby";
                return false;
            }
            if (lobbyPlayersCount < HostConfig.minPlayersInLobby)
            {
                lobbyPlayersErrorMessage = $"Minimum {HostConfig.minPlayersInLobby} Players Required to Start a Race";
                return false;
            }
            return true;
        }

        private async Task SetLobbyPlayers(IExecutionContext context, List<RaceLobbyParticipant> lobbyPlayers)
        {
            for (int i = 0; i < lobbyPlayers.Count; i++)
            {
                await pushClient.SendPlayerMessageAsync(context, $"{lobbyPlayers[i].HorseNumber}", "RaceStart", lobbyPlayers[i].PlayerID);
            }
            //Set Race Lobby players in Cloud and reset RaceResults
            await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
               new SetItemBatchBody(new List<SetItemBody>()
                  {
                           new SetItemBody("RaceLobby", JsonConvert.SerializeObject(lobbyPlayers)),
                           new (StringUtils.RACERESULTSKEY,"")
                  }));
        }

        private async void SetUnQualifiedPlayers(IExecutionContext context, List<string> notQualifiedPlayersList)
        {
            //For unqualified players send zero as horse number
            foreach (var unQualifiedPlayerID in notQualifiedPlayersList)
            {
                await pushClient.SendPlayerMessageAsync(context, $"{0}", "RaceStart", unQualifiedPlayerID);
            }
        }


        [CloudCodeFunction("RaceResults")]
        public async Task SendRaceResultToPlayers(IExecutionContext context, RaceResult raceResultData)
        {
            if (raceResultData != null)
            {
                for (int i = 0; i < raceResultData.playerRaceResults.Count; i++)
                {
                    //Send Message to the players for Race Results
                    await pushClient.SendPlayerMessageAsync(context, $"{JsonConvert.SerializeObject(raceResultData.playerRaceResults[i])}", "RaceResult", raceResultData.playerRaceResults[i].PlayerID);
                }
            }

            //Update RaceResults, Clear Lobby Data and Current Race Checkins
            await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
                new SetItemBatchBody(new List<SetItemBody>()
                   {
                           new ("RaceLobby", ""),
                           new ("RaceCheckIn", ""),
                           new SetItemBody("RaceResults", JsonConvert.SerializeObject(raceResultData))
                   }));
        }
    }
}
