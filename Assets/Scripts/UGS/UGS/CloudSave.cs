using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;

namespace UGS
{
    public class CloudSave
    {
        public async Task<string> GetHostID(string customID, float latitude, float longitude)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(customID);
            foreach (var customItem in customItemData)
            {
                string customItemValue = customItem.Value.Value.GetAsString();
                if (!string.IsNullOrEmpty(customItemValue) && !string.IsNullOrWhiteSpace(customItemValue))
                {
                    VenueRegistrationRequest venueRegistrationRequest = JsonConvert.DeserializeObject<VenueRegistrationRequest>(customItemValue);
                    float distance = DistanceCalculator.CalculateHaversineDistance(venueRegistrationRequest.Latitude, venueRegistrationRequest.Longitude, latitude, longitude);
                    if (distance <= venueRegistrationRequest.Radius)
                    {
                        return customItem.Key;
                    }
                }
            }
            return string.Empty;
        }

        public async Task<string> GetHostVenueName(string customID, float latitude, float longitude)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(customID);
            foreach (var customItem in customItemData)
            {
                string customItemValue = customItem.Value.Value.GetAsString();
                if (!string.IsNullOrEmpty(customItemValue) && !string.IsNullOrWhiteSpace(customItemValue))
                {
                    VenueRegistrationRequest venueRegistrationRequest = JsonConvert.DeserializeObject<VenueRegistrationRequest>(customItemValue);
                    float distance = DistanceCalculator.CalculateHaversineDistance(venueRegistrationRequest.Latitude, venueRegistrationRequest.Longitude, latitude, longitude);
                    if (distance <= venueRegistrationRequest.Radius)
                    {
                        return venueRegistrationRequest.Name;
                    }
                }
            }
            return string.Empty;
        }

        public async Task<bool> IsPlayerAlreadyCheckIn(string hostID, string playerID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<CurrentRacePlayerCheckIn> raceLobbyData = JsonConvert.DeserializeObject<List<CurrentRacePlayerCheckIn>>(raceItemValue);
                    foreach (var raceLobby in raceLobbyData)
                    {
                        if (raceLobby.PlayerID == playerID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<List<CurrentRacePlayerCheckIn>> GetRaceCheckinParticipants(string hostID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<CurrentRacePlayerCheckIn> checkinPlayers = JsonConvert.DeserializeObject<List<CurrentRacePlayerCheckIn>>(raceItemValue);
                    return checkinPlayers;
                }
            }
            return null;
        }

        public async Task<List<RaceLobbyParticipant>> TryGetRaceLobby(string hostID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceItemValue = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceItemValue) && !string.IsNullOrWhiteSpace(raceItemValue))
                {
                    List<RaceLobbyParticipant> raceLobbyData = JsonConvert.DeserializeObject<List<RaceLobbyParticipant>>(raceItemValue);
                    return raceLobbyData;
                }
            }
            return null;
        }

        public async Task<PlayerRaceResult> TryGetPlayerRaceResult(string hostID, string playerID, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(hostID, new HashSet<string> { key });

            if (customItemData.TryGetValue(key, out var item))
            {
                string raceResult = item.Value.GetAs<string>();
                if (!string.IsNullOrEmpty(raceResult) && !string.IsNullOrWhiteSpace(raceResult))
                {
                    RaceResult raceResults = JsonConvert.DeserializeObject<RaceResult>(raceResult);
                    foreach (var raceResultParticipant in raceResults.playerRaceResults)
                    {
                        if (raceResultParticipant.PlayerID == playerID)
                        {
                            return raceResultParticipant;
                        }
                    }
                }
            }
            return null;
        }

        public async void SetUserHostAsync(bool val)
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>() { { "Host", val } });
        }

        public async Task<bool> IsUserHostAsync()
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "Host" });
            if (result.TryGetValue("Host", out var val))
            {
                return val.Value.GetAs<bool>();
            }
            return false;
        }

        public async Task SetCharacterDataAsync(string key, SaveCharacterData _storeCharacterData)
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>
            {
                { key, _storeCharacterData }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData, new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions()));
        }

        public async Task<SaveCharacterData> GetCharacterDataAsync(string key)
        {
            SaveCharacterData characterData = null;
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key }, new LoadOptions(new PublicReadAccessClassOptions()));
            if (result.TryGetValue(key, out var val))
            {
                characterData = val.Value.GetAs<SaveCharacterData>();
            }
            return characterData;
        }

        public async Task<VenueRegistrationRequest> GetVenueRegistrationDataAsync(string Id, string key)
        {
            var customItemData = await CloudSaveService.Instance.Data.Custom.LoadAsync(Id, new HashSet<string> { key });
            if (customItemData.TryGetValue(key, out var item))
            {
                var result = item.Value.GetAs<VenueRegistrationRequest>();
                return result;
            }
            return null;
        }
    }
}
