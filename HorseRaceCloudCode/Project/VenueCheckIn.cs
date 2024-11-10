using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class VenueCheckIn
    {
        private readonly IGameApiClient gameApiClient;
        private readonly ILogger<VenueCheckIn> _logger;

        public VenueCheckIn(IGameApiClient _gameApiClient, ILogger<VenueCheckIn> logger)
        {
            this.gameApiClient = _gameApiClient;
            this._logger = logger;
        }
        #region CheatCode Cloud Methods
        [CloudCodeFunction("CheatCodeVenueCheckIn")]
        public async Task<VenueCheckInResponse> CheatCodeCheckInVenue(IExecutionContext context, string venueName, string dateTimeString)
        {
            VenueCheckInResponse response = new VenueCheckInResponse();
            DateTime currentDateTime = DateTimeUtils.ConvertStringToUTCTime(dateTimeString);

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                response.Message = "Invalid Player ID";
                return response;
            }

            if (StringUtils.IsEmpty(venueName))
            {
                response.Message = "Invalid Host Venue";
                return response;
            }

            //Get the player checkin records from the cloud
            string playerCheckinsKey = $"{venueName}{currentDateTime.ToString(StringUtils.YEAR_MONTH_FORMAT)}";
            List<PlayerVenueCheckIn>? currentVenueCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerVenueCheckIn>>(context, gameApiClient, context.PlayerId, playerCheckinsKey);

            //Check if Player has already checked in today
            if (IsAlreadyCheckedInToday(currentVenueCheckInsList, currentDateTime, out int lastCheckInIndex))
            {
                //Get Todays Checkin Count
                response.CheckInCount = currentVenueCheckInsList[lastCheckInIndex].Count;

                //If the player has already checked in the current interval, then return the next check-in time.
                if (IsAlreadyCheckedInCurrentInterval(currentVenueCheckInsList[lastCheckInIndex].LastCheckInTime, currentDateTime))
                {
                    // Tell the player when they can check in next.
                    DateTime nextCheckInTime = GetNextCheckInTime(currentDateTime, HostConfig.venueCheckInInterval);
                    response.NextCheckInTime = nextCheckInTime.ToString();
                    return response;
                }
            }

            response.CanCheckIn = true;
            response.Message = "Click to Check-In";
            return response;
        }

        #endregion

        [CloudCodeFunction("SetVenueCheckIn")]
        public async Task<VenueCheckInResponse> SetVenueCheckIn(IExecutionContext context, string venueName)
        {
            VenueCheckInResponse response = new VenueCheckInResponse();
            DateTime currentDateTime = DateTime.UtcNow;

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                response.Message = "Invalid Player ID";
                return response;
            }

            if (StringUtils.IsEmpty(venueName))
            {
                response.Message = "Invalid Host Venue";
                return response;
            }

            //Get the player checkin records from the cloud
            string playerCheckinsKey = $"{venueName}{currentDateTime.ToString(StringUtils.YEAR_MONTH_FORMAT)}";
            List<PlayerVenueCheckIn>? currentVenueCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerVenueCheckIn>>(context, gameApiClient, context.PlayerId, playerCheckinsKey);

            //Check if Player has already checked in today
            if (IsAlreadyCheckedInToday(currentVenueCheckInsList, currentDateTime, out int dayIndex))
            {
                //Update today's venue checkin record.
                UpdatePlayerCheckIn(currentVenueCheckInsList[dayIndex], currentDateTime);
            }
            else
            {
                //Player has not checked in today, so add the player's today first checkin.
                AddPlayerCheckInToList(currentVenueCheckInsList, currentDateTime);
            }

            response.CheckInCount = currentVenueCheckInsList[dayIndex].Count;
            response.NextCheckInTime = GetNextCheckInTime(currentDateTime, HostConfig.venueCheckInInterval).ToString();
            response.IsSuccess = true;

            await gameApiClient.CloudSaveData.SetProtectedItemAsync(context, context.ServiceToken, context.ProjectId,
                          context.PlayerId, new SetItemBody(playerCheckinsKey, JsonConvert.SerializeObject(currentVenueCheckInsList)));

            return response;
        }


        [CloudCodeFunction("CheckedInVenue")]
        public async Task<VenueCheckInResponse> CheckInVenue(IExecutionContext context, string venueName)
        {
            VenueCheckInResponse response = new VenueCheckInResponse();
            DateTime currentDateTime = DateTime.UtcNow;

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                response.Message = "Invalid Player ID";
                return response;
            }

            if (StringUtils.IsEmpty(venueName))
            {
                response.Message = "Invalid Host Venue";
                return response;
            }

            //Get the player checkin records from the cloud
            string playerCheckinsKey = $"{venueName}{currentDateTime.ToString(StringUtils.YEAR_MONTH_FORMAT)}";
            List<PlayerVenueCheckIn>? currentVenueCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerVenueCheckIn>>(context, gameApiClient, context.PlayerId, playerCheckinsKey);

            //Check if Player has already checked in today
            if (IsAlreadyCheckedInToday(currentVenueCheckInsList, currentDateTime, out int lastCheckInIndex))
            {
                //Get Todays Checkin Count
                response.CheckInCount = currentVenueCheckInsList[lastCheckInIndex].Count;

                //If the player has already checked in the current interval, then return the next check-in time.
                if (IsAlreadyCheckedInCurrentInterval(currentVenueCheckInsList[lastCheckInIndex].LastCheckInTime, currentDateTime))
                {
                    // Tell the player when they can check in next.
                    DateTime nextCheckInTime = GetNextCheckInTime(currentDateTime, HostConfig.venueCheckInInterval);
                    response.NextCheckInTime = nextCheckInTime.ToString();
                    return response;
                }
            }

            response.CanCheckIn = true;
            response.Message = "Click to Check-In";
            return response;
        }

        public void UpdatePlayerCheckIn(PlayerVenueCheckIn playerCheckIn, DateTime dateTime)
        {
            playerCheckIn.Count++;
            playerCheckIn.LastCheckInTime = dateTime.ToString(StringUtils.HOUR_MINUTE_FORMAT);
        }

        public void AddPlayerCheckInToList(List<PlayerVenueCheckIn> venueCheckInsList, DateTime dateTime)
        {
            PlayerVenueCheckIn checkInAttendance = new PlayerVenueCheckIn
            {
                Date = dateTime.ToString(StringUtils.DAY_FORMAT),
                LastCheckInTime = dateTime.ToString(StringUtils.HOUR_MINUTE_FORMAT),
                Count = 1
            };
            venueCheckInsList.Add(checkInAttendance);
        }

        public DateTime GetNextCheckInTime(DateTime dateTime, int checkInInterval)
        {
            // Calculate the next check-in time based on the interval within the hour
            int minutesPastHour = dateTime.Minute % checkInInterval;
            int minutesToAdd = checkInInterval - minutesPastHour;
            DateTime nextCheckInTime = dateTime.AddMinutes(minutesToAdd).AddSeconds(-dateTime.Second);

            // Calculate the time until the next check-in
            // TimeSpan timeUntilNextCheckIn = nextCheckInTime - currentDateTime;
            return nextCheckInTime;
        }

        public bool IsAlreadyCheckedInToday(List<PlayerVenueCheckIn> venueCheckInsList, DateTime currentDateTime, out int dayIndex)
        {
            dayIndex = 0;
            if (venueCheckInsList.Count > 0)
            {
                for (int i = 0; i < venueCheckInsList.Count; i++)
                {
                    if (venueCheckInsList[i].Date == currentDateTime.Date.ToString(StringUtils.DAY_FORMAT))
                    {
                        dayIndex = i;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  Check if the player has checked in the current venue checkin interval already.
        /// </summary>
        /// <param name="playerCheckIn"></param>
        /// <param name="currentDateTime"></param>
        /// <returns></returns>
        public bool IsAlreadyCheckedInCurrentInterval(string lastCheckInTime, DateTime currentDateTime)
        {
            //Parse the last checkin time.
            DateTime lastCheckInDateTime = DateTime.ParseExact(lastCheckInTime, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
            int lastCheckInInterval = (lastCheckInDateTime.Hour * 60 + lastCheckInDateTime.Minute) / HostConfig.venueCheckInInterval;
            int currentInterval = (currentDateTime.Hour * 60 + currentDateTime.Minute) / HostConfig.venueCheckInInterval;

            //If the current interval is the same as the last checkin interval
            return currentInterval == lastCheckInInterval;
        }
    }
}
