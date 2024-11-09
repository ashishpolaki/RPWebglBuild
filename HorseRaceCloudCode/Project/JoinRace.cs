using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class JoinRace
    {
        private readonly IGameApiClient gameApiClient;
        private readonly ILogger<JoinRace> _logger;

        public JoinRace(IGameApiClient _gameApiClient, ILogger<JoinRace> logger)
        {
            this.gameApiClient = _gameApiClient;
            this._logger = logger;
        }
        #region Race Join Request
        [CloudCodeFunction("RaceJoinRequest")]
        public async Task<JoinRaceResponse> RaceJoinRequest(IExecutionContext context, string hostID, string dateTimeString)
        {
            JoinRaceResponse? joinRaceResponse = new JoinRaceResponse();
            DateTime currentDateTime = Utils.ParseDateTime(dateTimeString);

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                joinRaceResponse.Message = "Invalid Player ID";
                return joinRaceResponse;
            }
            if (StringUtils.IsEmpty(hostID))
            {
                joinRaceResponse.Message = "Invalid Host ID";
                return joinRaceResponse;
            }
            if (DateTimeUtils.IsValidDateTime(dateTimeString) == false)
            {
                joinRaceResponse.Message = "Invalid DateTime Format";
                return joinRaceResponse;
            };

            //Get host Race Data ffrom the cloud
            var hostRaceScheduleData = await Utils.GetCustomDataWithKey<RaceScheduleRequest>(context, gameApiClient, hostID, "RaceSchedule");

            //Check if the player has updated the Race Schedule Time
            if (StringUtils.IsEmpty(hostRaceScheduleData.ScheduleStart))
            {
                joinRaceResponse.Message = "Host Not updated Race Schedule Time";
                return joinRaceResponse;
            }

            if (DateTimeUtils.IsValidDateTime(hostRaceScheduleData.ScheduleStart) == false || DateTimeUtils.IsValidDateTime(hostRaceScheduleData.ScheduleEnd) == false)
            {
                joinRaceResponse.Message = "Host has Invalid Race Schedule Format";
                return joinRaceResponse;
            }

            AdjustEndTimeIfEarlierThanStartTime(hostRaceScheduleData.ScheduleStart, hostRaceScheduleData.ScheduleEnd, out DateTime raceStartTime, out DateTime raceEndTime);
            List<DateTime> raceTimings = GenerateRaceTimingsFromSchedule(raceStartTime, raceEndTime, TimeSpan.FromMinutes(hostRaceScheduleData.RaceInterval));
            DateTime? getRaceDateTime = FindNextRaceToday(raceTimings, currentDateTime);

            //Check if getRaceDateTime is null
            if (getRaceDateTime == null)
            {
                joinRaceResponse.Message = "Join Race Tomorrow";
                return joinRaceResponse;
            }

            //Check if the player can wait in the lobby
            bool canWaitInLobby = CanPlayerWaitInLobby(getRaceDateTime.Value, currentDateTime, hostRaceScheduleData.RaceInterval);

            if (canWaitInLobby)
            {
                joinRaceResponse.RaceTime = getRaceDateTime.Value;
                joinRaceResponse.CanWaitInLobby = true;
            }
            else
            {
                //Show the time the player can join the lobby
                TimeSpan timeUntilLobbyOpen = (getRaceDateTime.Value - currentDateTime) + new TimeSpan(0, -hostRaceScheduleData.RaceInterval, 0);
                joinRaceResponse.Message = $"Player can join the lobby after {timeUntilLobbyOpen.Hours.ToString("D2")}:{timeUntilLobbyOpen.Minutes.ToString("D2")}:{timeUntilLobbyOpen.Seconds.ToString("D2")}";
            }

            return joinRaceResponse;
        }
        public bool CanPlayerWaitInLobby(DateTime raceTime, DateTime currentTime, int lobbyWaitTime)
        {
            TimeSpan timeUntilNextRace = raceTime - currentTime;
            return timeUntilNextRace.TotalSeconds <= (lobbyWaitTime * 60);
        }
        public void AdjustEndTimeIfEarlierThanStartTime(string startTime, string endTime, out DateTime raceStartTime, out DateTime raceEndTime)
        {
            raceStartTime = DateTime.ParseExact(startTime, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture);
            raceEndTime = DateTime.ParseExact(endTime, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture);

            // If the end time is earlier in the day than the start time, add a day to the end time
            if (raceEndTime < raceStartTime)
            {
                raceEndTime = raceEndTime.AddDays(1);
            }
        }
        public DateTime? FindNextRaceToday(List<DateTime> raceTimings, DateTime currentTime)
        {
            foreach (var raceTime in raceTimings)
            {
                if (raceTime > currentTime)
                {
                    return raceTime;
                }
            }
            return null;
        }
        public List<DateTime> GenerateRaceTimingsFromSchedule(DateTime startTime, DateTime endTime, TimeSpan interval)
        {
            List<DateTime> timings = new List<DateTime>();
            DateTime currentTime = startTime;

            while (currentTime <= endTime)
            {
                timings.Add(currentTime);
                currentTime = currentTime.Add(interval);
            }
            return timings;
        }
        #endregion


        #region Race CheckIn
        [CloudCodeFunction("ConfirmRaceCheckIn")]
        public async Task<bool> ConfirmRaceCheckIn(IExecutionContext context, string hostID, string playerName)
        {
            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                return false;
            }

            if (StringUtils.IsEmpty(hostID))
            {
                return false;
            }

            //key for the player Venue checkin records
            DateTime currentDateTime = DateTime.UtcNow;
            string key = $"{hostID}{currentDateTime.ToString(StringUtils.YEAR_MONTH_FORMAT)}";

            //Get the player checkin records from the cloud
            List<PlayerVenueCheckIn>? currentVenueCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerVenueCheckIn>>(context, gameApiClient, context.PlayerId, key);

            //Check if the player has already checked in today
            int currentDayVenueCheckIns = 0;
            if (IsAlreadyVenueCheckedInToday(currentVenueCheckInsList, currentDateTime, out int index))
            {
                currentDayVenueCheckIns = currentVenueCheckInsList[index].Count;
            }

            //Add the player to the list
            List<CurrentRacePlayerCheckIn>? raceCheckInPlayers = await Utils.GetCustomDataWithKey<List<CurrentRacePlayerCheckIn>>(context, gameApiClient, hostID, "RaceCheckIn");
            raceCheckInPlayers.Add(new CurrentRacePlayerCheckIn() { PlayerID = context.PlayerId, PlayerName = playerName, CurrentDayCheckIns = currentDayVenueCheckIns });

            //Save the updated list
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                         hostID, new SetItemBody("RaceCheckIn", JsonConvert.SerializeObject(raceCheckInPlayers)));

            return true;
        }
        private bool IsAlreadyVenueCheckedInToday(List<PlayerVenueCheckIn> playerCheckInsList, DateTime currentDateTime, out int index)
        {
            index = -1;
            if (playerCheckInsList.Count > 0)
            {
                for (int i = 0; i < playerCheckInsList.Count; i++)
                {
                    if (playerCheckInsList[i].Date == currentDateTime.Date.ToString(StringUtils.DAY_FORMAT))
                    {
                        index = i;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
