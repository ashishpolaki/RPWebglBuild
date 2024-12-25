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

        [CloudCodeFunction("EnterRaceRequest")]
        public async Task<EnterRaceResponse> EnterRaceRequest(IExecutionContext context, ICheatCode cheatCode, IRaceController raceController, string venueName)
        {
            EnterRaceResponse enterRaceResponse = new EnterRaceResponse();
            DateTime currentDateTime = DateTime.UtcNow;

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                enterRaceResponse.Message = "Invalid Player ID";
                return enterRaceResponse;
            }

            currentDateTime = cheatCode.IsCheatCodeActive(context.PlayerId) ? cheatCode.CurrentDateTime(context.PlayerId) : currentDateTime;

            if (StringUtils.IsEmpty(venueName))
            {
                enterRaceResponse.Message = "Invalid Venue Name";
                return enterRaceResponse;
            }

            //Get host Race Data from the cloud
            var hostRaceScheduleData = await Utils.GetCustomDataWithKey<RaceScheduleRequest>(context, gameApiClient, venueName, "RaceSchedule");

            //Check if the player has updated the Race Schedule Time
            if (StringUtils.IsEmpty(hostRaceScheduleData.ScheduleStart))
            {
                enterRaceResponse.Message = "Host Not updated Race Schedule Time";
                return enterRaceResponse;
            }

            if (DateTimeUtils.IsValidDateTime(hostRaceScheduleData.ScheduleStart) == false || DateTimeUtils.IsValidDateTime(hostRaceScheduleData.ScheduleEnd) == false)
            {
                enterRaceResponse.Message = "Host has Invalid Race Schedule Format";
                return enterRaceResponse;
            }

            AdjustEndTimeIfEarlierThanStartTime(hostRaceScheduleData.ScheduleStart, hostRaceScheduleData.ScheduleEnd, out DateTime raceStartTime, out DateTime raceEndTime);
            List<DateTime> raceTimings = GenerateRaceTimingsFromSchedule(raceStartTime, raceEndTime, TimeSpan.FromMinutes(hostRaceScheduleData.RaceTimings));

            //If upcoming race is not found, then the player can join race tomorrow
            bool isUpcomingRaceFound = IsFindUpcomingTodayRace(raceTimings, currentDateTime, out DateTime upcomingRaceTime);
            if (isUpcomingRaceFound == false)
            {
                enterRaceResponse.Message = "Join Race Tomorrow";
                return enterRaceResponse;
            }

            enterRaceResponse.IsConfirmRaceCheckIn = raceController.IsPlayerCheckedIn(context.PlayerId, venueName);
            enterRaceResponse.UpcomingRaceTime = upcomingRaceTime.ToString();
            enterRaceResponse.IsFoundUpcomingRace = isUpcomingRaceFound;
            enterRaceResponse.RaceInterval = hostRaceScheduleData.RaceInterval;
            return enterRaceResponse;
        }

        [CloudCodeFunction("RaceCheckInRequest")]
        public async Task<RaceCheckInResponse> RaceCheckInRequest(IExecutionContext context, ICheatCode cheatCode, IRaceController iController, string venueName, string playerName)
        {
            RaceCheckInResponse raceCheckInResponse = new RaceCheckInResponse();

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                raceCheckInResponse.Message = "Invalid Player ID";
                return raceCheckInResponse;
            }

            if (StringUtils.IsEmpty(venueName))
            {
                raceCheckInResponse.Message = "Invalid Venue Name";
                return raceCheckInResponse;
            }

            //Check if player is cheating by changing time in his device.
            DateTime currentDateTime = DateTime.UtcNow;

#if !CheatCode
            currentDateTime = cheatCode.IsCheatCodeActive(context.PlayerId) ? cheatCode.CurrentDateTime(context.PlayerId) : currentDateTime;
#endif

            var hostRaceScheduleData = await Utils.GetCustomDataWithKey<RaceScheduleRequest>(context, gameApiClient, venueName, "RaceSchedule");
            bool isConfirmRaceCHeckInValid = IsConfirmRaceCheckInValid(hostRaceScheduleData, currentDateTime);
            if (!isConfirmRaceCHeckInValid)
            {
                raceCheckInResponse.Message = "Improper Actions Detected.";
                return raceCheckInResponse;
            }

            //Calculate Player's venue checkin data
            string key = $"{venueName}{currentDateTime.ToString(StringUtils.YEAR_MONTH_FORMAT)}";
            int currentDayVenueCheckIns = 0;
            List<PlayerVenueCheckIn>? currentVenueCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerVenueCheckIn>>(context, gameApiClient, context.PlayerId, key);
            if (IsAlreadyVenueCheckedInToday(currentVenueCheckInsList, currentDateTime, out int index))
            {
                currentDayVenueCheckIns = currentVenueCheckInsList[index].Count;
            }
            CurrentRacePlayerCheckIn currentRacePlayerCheckIn = new CurrentRacePlayerCheckIn() { PlayerID = context.PlayerId, PlayerName = playerName, CurrentDayCheckIns = currentDayVenueCheckIns };

            //Add the player to the confirm raceCheckin list
            iController.AddRaceCheckIn(currentRacePlayerCheckIn, venueName);

            raceCheckInResponse.IsSuccess = true;
            return raceCheckInResponse;
        }

        [CloudCodeFunction("TryGetRaceLobbyPlayer")]
        public async Task<RaceLobbyParticipant> TryGetRaceLobbyPlayer(IExecutionContext context, IRaceController controller, string venueName)
        {
            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                return new RaceLobbyParticipant();
            }

            if (StringUtils.IsEmpty(venueName))
            {
                return new RaceLobbyParticipant();
            }

            await Task.Run(() =>
            {
                return controller.GetRaceLobbyParticipant(context.PlayerId, venueName);
            });
            return new RaceLobbyParticipant();
        }

        [CloudCodeFunction("PreviousRaceResult")]
        public async Task<PlayerRaceResult> GetPlayerRaceResult(IExecutionContext context, IRaceController controller, string venueName)
        {
            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                return new PlayerRaceResult();
            }

            if (StringUtils.IsEmpty(venueName))
            {
                return new PlayerRaceResult();
            }
            PlayerRaceResult playerRaceResult = await Task.Run(() =>
            {
                PlayerRaceResult result = controller.GetPlayerRaceResult(context.PlayerId, venueName);
                return result;
            });

            return playerRaceResult;
        }


        #region Private Methods
        private bool IsConfirmRaceCheckInValid(RaceScheduleRequest hostRaceScheduleData, DateTime currentDateTime)
        {
            AdjustEndTimeIfEarlierThanStartTime(hostRaceScheduleData.ScheduleStart, hostRaceScheduleData.ScheduleEnd, out DateTime raceStartTime, out DateTime raceEndTime);
            List<DateTime> raceTimings = GenerateRaceTimingsFromSchedule(raceStartTime, raceEndTime, TimeSpan.FromMinutes(hostRaceScheduleData.RaceTimings));
            bool isUpcomingRaceFound = IsFindUpcomingTodayRace(raceTimings, currentDateTime, out DateTime upcomingRaceTime);
            if (!isUpcomingRaceFound)
            {
                return false;
            }
            bool canConfirmRaceCheckIn = CanPlayerRaceCheckIn(upcomingRaceTime, currentDateTime, hostRaceScheduleData.RaceInterval);
            if (!canConfirmRaceCheckIn)
            {
                return false;
            }
            return true;
        }

        public bool CanPlayerRaceCheckIn(DateTime raceTime, DateTime currentTime, int lobbyWaitTime)
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
        public bool IsFindUpcomingTodayRace(List<DateTime> raceTimings, DateTime currentTime, out DateTime todayUpcomingRaceTime)
        {
            todayUpcomingRaceTime = DateTime.MinValue;
            foreach (var raceTime in raceTimings)
            {
                if (raceTime > currentTime)
                {
                    todayUpcomingRaceTime = raceTime;
                    return true;
                }
            }
            return false;
        }
        public List<DateTime> GenerateRaceTimingsFromSchedule(DateTime startTime, DateTime endTime, TimeSpan timeInterval)
        {
            List<DateTime> timings = new List<DateTime>();
            DateTime currentTime = startTime;

            while (currentTime <= endTime)
            {
                timings.Add(currentTime);
                currentTime = currentTime.Add(timeInterval);
            }
            return timings;
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
