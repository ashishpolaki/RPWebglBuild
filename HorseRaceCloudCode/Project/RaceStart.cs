using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        #region Start Race
        [CloudCodeFunction("StartRace")]
        public async Task<StartRaceResponse> StartRace(IExecutionContext context, IRaceController raceController, StartRaceRequest startRaceRequest)
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

            VenueRegistrationRequest venueRegistrationRequest = await Utils.GetCustomDataWithKey<VenueRegistrationRequest>(context, gameApiClient, StringUtils.HOSTVENUEKEY, context.PlayerId);

            //Get Player Outfits
            Dictionary<int, CharacterCustomisationEconomy> playerOutfits = new Dictionary<int, CharacterCustomisationEconomy>();
            foreach (var player in startRaceRequest.RaceLobbyParticipants)
            {
                var playerInventory = await gameApiClient.EconomyInventory.GetPlayerInventoryAsync(context, context.ServiceToken, context.ProjectId, player.PlayerID);
                CharacterCustomisationEconomy characterCustomisationEconomy = JsonConvert.DeserializeObject<CharacterCustomisationEconomy>(playerInventory.Data.Results[0].InstanceData.ToString());
                playerOutfits.Add(player.HorseNumber, characterCustomisationEconomy);
            }
            response.playerOutfits = playerOutfits;

            //Start Race
            await SetLobbyPlayers(context, raceController, venueRegistrationRequest.Name, startRaceRequest.RaceLobbyParticipants);
            SetUnQualifiedPlayers(context, startRaceRequest.UnQualifiedPlayerIDs);
            raceController.DeleteRaceCheckIns(venueRegistrationRequest.Name);
            raceController.DeleteRaceResult(venueRegistrationRequest.Name);
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

        private async Task SetLobbyPlayers(IExecutionContext context, IRaceController raceController, string venueName, List<RaceLobbyParticipant> lobbyPlayers)
        {
            for (int i = 0; i < lobbyPlayers.Count; i++)
            {
                await pushClient.SendPlayerMessageAsync(context, $"{lobbyPlayers[i].HorseNumber}", "RaceStart", lobbyPlayers[i].PlayerID);
                raceController.AddRaceLobbyParticipant(lobbyPlayers[i], venueName);
            }
        }

        private async void SetUnQualifiedPlayers(IExecutionContext context, List<CurrentRacePlayerCheckIn> notQualifiedPlayersList)
        {
            //For unqualified players send zero as horse number
            foreach (var unQualifiedPlayer in notQualifiedPlayersList)
            {
                await pushClient.SendPlayerMessageAsync(context, $"{-1}", "RaceStart", unQualifiedPlayer.PlayerID);
            }
        }
        #endregion

        [CloudCodeFunction("GetVenueRaceCheckIns")]
        public async Task<List<CurrentRacePlayerCheckIn>> GetVenueRaceCheckIns(IExecutionContext context, IRaceController controller)
        {
            VenueRegistrationRequest venueRegistrationRequest = await Utils.GetCustomDataWithKey<VenueRegistrationRequest>(context, gameApiClient, StringUtils.HOSTVENUEKEY, context.PlayerId);

            if (venueRegistrationRequest != null)
            {
                return controller.GetRaceCheckInPlayers(venueRegistrationRequest.Name);
            }

            return new List<CurrentRacePlayerCheckIn>();
        }

        [CloudCodeFunction("RaceResults")]
        public async Task SendRaceResultToPlayers(IExecutionContext context, IRaceController controller, RaceResult raceResultData)
        {
            VenueRegistrationRequest venueRegistrationRequest = await Utils.GetCustomDataWithKey<VenueRegistrationRequest>(context, gameApiClient, StringUtils.HOSTVENUEKEY, context.PlayerId);

            for (int i = 0; i < raceResultData.playerRaceResults.Count; i++)
            {
                //Send Message to the players for Race Results
                await pushClient.SendPlayerMessageAsync(context, $"{JsonConvert.SerializeObject(raceResultData.playerRaceResults[i])}", "RaceResult", raceResultData.playerRaceResults[i].PlayerID);
                controller.AddRaceResult(raceResultData.playerRaceResults[i], venueRegistrationRequest.Name);

                //Set Race Wins Count
                if (raceResultData.playerRaceResults[i].RacePosition == 1)
                {
                    int raceWinsCount = 0;
                    var response = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(context, context.ServiceToken, context.ProjectId, raceResultData.playerRaceResults[i].PlayerID, new List<string> { "TotalRaceWins" });
                    if (response != null && response.Data != null && response.Data.Results != null && response.Data.Results.Count > 0)
                    {
                        raceWinsCount = JsonConvert.DeserializeObject<int>(response.Data.Results[0].Value.ToString());
                    }
                    raceWinsCount++;
                    await gameApiClient.CloudSaveData.SetProtectedItemAsync(context, context.ServiceToken, context.ProjectId, raceResultData.playerRaceResults[i].PlayerID, new SetItemBody("TotalRaceWins", raceWinsCount));
                }
            }
            controller.DeleteRaceLobby(venueRegistrationRequest.Name);
        }
    }
}
