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
    public class VenueCheckIn
    {
        private readonly IGameApiClient gameApiClient;
        private readonly ILogger<VenueCheckIn> _logger;

        public VenueCheckIn(IGameApiClient _gameApiClient, ILogger<VenueCheckIn> logger)
        {
            this.gameApiClient = _gameApiClient;
            this._logger = logger;
        }

        [CloudCodeFunction("CheckedInVenue")]
        public async Task<VenueRegistrationResponse> CheckInVenue(IExecutionContext context, string hostId, string dateTimeString)
        {
            VenueRegistrationResponse venueRegistrationResponse = new VenueRegistrationResponse();
            DateTime dateTime = DateTimeUtils.ConvertStringToUTCTime(dateTimeString);

            if (context.PlayerId == null || StringUtils.IsEmpty(context.PlayerId))
            {
                venueRegistrationResponse.Message = "Invalid Player ID";
                return venueRegistrationResponse;
            }

            if (StringUtils.IsEmpty(hostId))
            {
                venueRegistrationResponse.Message = "Invalid Host ID";
                return venueRegistrationResponse;
            }

            if(DateTimeUtils.IsValidDateTime(dateTimeString) == false)
            {
                venueRegistrationResponse.Message = "Invalid DateTime Format";
                return venueRegistrationResponse;
            };

            //Get the player checkin records from the cloud
            string playerCheckinsKey = $"{hostId}{dateTime.ToString(StringUtils.YEAR_MONTH_FORMAT)}";
            List<PlayerVenueCheckIn>? currentVenueCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerVenueCheckIn>>(context, gameApiClient, context.PlayerId, playerCheckinsKey);

            if (IsAlreadyCheckedInToday(currentVenueCheckInsList, dateTime, out int index))
            {
                if (IsAlreadyCheckedInCurrentInterval(currentVenueCheckInsList[index].LastCheckInTime, dateTime))
                {
                    // Tell the player when they can check in next.
                    TimeSpan timeSpan = GetNextCheckInTime(dateTime, HostConfig.venueCheckInInterval);
                    venueRegistrationResponse.Message = $"Next check-in is after {timeSpan.Hours} hours and {timeSpan.Minutes} minutes and {timeSpan.Seconds} seconds.";
                    return venueRegistrationResponse;
                }

                //Update today's venue checkin record.
                UpdatePlayerCheckIn(currentVenueCheckInsList[index], dateTime);
            }
            else
            {
                //Player has not checked in today, so add the player's today first checkin.
                AddPlayerCheckInToList(currentVenueCheckInsList, dateTime);
            }

            await gameApiClient.CloudSaveData.SetProtectedItemAsync(context, context.ServiceToken, context.ProjectId,
                               context.PlayerId, new SetItemBody(playerCheckinsKey, JsonConvert.SerializeObject(currentVenueCheckInsList)));
            venueRegistrationResponse.Message = "Successfully Checked In";
            return venueRegistrationResponse;
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

        public TimeSpan GetNextCheckInTime(DateTime dateTime,int checkInInterval)
        {
            // Calculate the next check-in time based on the interval within the hour
            int minutesPastHour = dateTime.Minute % checkInInterval;
            int minutesToAdd = checkInInterval - minutesPastHour;
            DateTime nextCheckInTime = dateTime.AddMinutes(minutesToAdd).AddSeconds(-dateTime.Second);

            // Calculate the time until the next check-in
            TimeSpan timeUntilNextCheckIn = nextCheckInTime - dateTime;
            return timeUntilNextCheckIn;
        }

        public bool IsAlreadyCheckedInToday(List<PlayerVenueCheckIn> venueCheckInsList, DateTime currentDateTime, out int index)
        {
            index = -1;
            if (venueCheckInsList.Count > 0)
            {
                for (int i = 0; i < venueCheckInsList.Count; i++)
                {
                    if (venueCheckInsList[i].Date == currentDateTime.Date.ToString(StringUtils.DAY_FORMAT))
                    {
                        index = i;
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
