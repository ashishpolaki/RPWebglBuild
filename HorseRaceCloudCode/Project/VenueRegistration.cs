using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class VenueRegistration
    {
        private readonly IGameApiClient gameApiClient;

        public VenueRegistration(IGameApiClient _gameApiClient)
        {
            this.gameApiClient = _gameApiClient;
        }

        [CloudCodeFunction("RegisterVenue")]
        public async Task<VenueRegistrationResponse> SetVenue(IExecutionContext context, VenueRegistrationRequest venueData)
        {
            VenueRegistrationResponse response = new VenueRegistrationResponse();
            response.IsRegistered = false;

            if (venueData == null)
            {
                response.Message = "Invalid Venue Data";
                return response;
            }

            if (context.PlayerId == null)
            {
                response.Message = "Invalid Player ID";
                return response;
            }

            if (Utils.IsValidGpsLocation(venueData.Latitude, venueData.Longitude) == false)
            {
                response.Message = "Invalid GPS Location";
                return response;
            }

            if (venueData.Radius < HostConfig.radiusMin || venueData.Radius > HostConfig.radiusMax)
            {
                response.Message = "Radius is out of range";
                return response;
            }

            //Register host data in VenuesList
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                  StringUtils.HOSTVENUEKEY, new SetItemBody(context.PlayerId, venueData));
            response.Message = "Venue Registered";
            response.IsRegistered = true;

            //If any data is not present, set default data.
            var playerResponse = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId);
            if (playerResponse.Data.Results.Count <= 0)
            {
                //Venue GameData
                await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
                new SetItemBatchBody(new List<SetItemBody>()
                   {
                           new (StringUtils.RACELOBBYKEY, ""),
                           new (StringUtils.RACESCHEDULEKEY,""),
                           new (StringUtils.RACECHECKINKEY, ""),
                           new (StringUtils.RACERESULTSKEY,"")
                   }));
            }
            return response;
        }

        [CloudCodeFunction("RaceScheduleTimings")]
        public async Task<RaceScheduleResponse> ScheduleRaceTimings(IExecutionContext context, RaceScheduleRequest raceScheduleRequest)
        {
            RaceScheduleResponse response = new RaceScheduleResponse();
            response.IsScheduled = false;

            if (raceScheduleRequest == null)
            {
                response.Message = "Invalid Schedule Data";
                return response;
            }
            if (context.PlayerId == null)
            {
                response.Message = "Invalid Player ID";
                return response;
            }
            if (IsStartRaceTimeValid(raceScheduleRequest.ScheduleStart, out string startmessage) == false)
            {
                response.Message = startmessage;
                return response;
            }
            if (IsEndRaceTimeValid(raceScheduleRequest.ScheduleStart, out string endmessage) == false)
            {
                response.Message = endmessage;
                return response;
            }
            if (CheckIfRaceScheduleTimesAreEqual(raceScheduleRequest.ScheduleStart, raceScheduleRequest.ScheduleEnd, out string _dateTimeEqualMessage))
            {
                response.Message = _dateTimeEqualMessage;
                return response;
            }
            if (IsRaceIntervalValid(raceScheduleRequest.RaceInterval, raceScheduleRequest.ScheduleStart, raceScheduleRequest.ScheduleEnd, out string raceIntervalMessage) == false)
            {
                response.Message = raceIntervalMessage;
                return response;
            }
            if (IsLobbyWaitTimeValid(raceScheduleRequest.LobbyWaitTime, raceScheduleRequest.RaceInterval, out string lobbyWaitMessage) == false)
            {
                response.Message = lobbyWaitMessage;
                return response;
            }

            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                       context.PlayerId, new SetItemBody(StringUtils.RACESCHEDULEKEY, raceScheduleRequest));
            response.Message = "Race Scheduled Successfully.";
            response.IsScheduled = true;
            return response;
        }

        private bool CheckIfRaceScheduleTimesAreEqual(string startTime, string endTime, out string message)
        {
            DateTime startScheduleTime = DateTime.Parse(startTime);
            DateTime endScheduleTime = DateTime.Parse(endTime);
            if (DateTimeUtils.AreDateTimesEqual(startScheduleTime, endScheduleTime))
            {
                message = "Start Race Time and End Race Time are same";
                return true;
            }
            message = string.Empty;
            return false;
        }

        private bool IsRaceIntervalValid(int _raceInterval, string startTime, string endTime, out string message)
        {
            if (_raceInterval <= 0)
            {
                message = "Race Interval should be greater than zero.";
                return false;
            }

            DateTime startScheduleTime = DateTime.Parse(startTime);
            DateTime endScheduleTime = DateTime.Parse(endTime);
            //If end time is less than start time, add a day to end time
            if (startScheduleTime > endScheduleTime)
            {
                endScheduleTime = endScheduleTime.AddDays(1);
            }
            //Check if raceInterval is greater than raceTime
            TimeSpan raceTimeSpan = endScheduleTime - startScheduleTime;
            TimeSpan raceIntervalSpan = TimeSpan.FromMinutes(_raceInterval);
            if (raceTimeSpan < raceIntervalSpan)
            {
                message = "The Race Interval should be less than the race schedule.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private bool IsStartRaceTimeValid(string _raceTime, out string message)
        {
            if (StringUtils.IsEmpty(_raceTime))
            {
                message = "Race Schedule Start Time is Empty";
                return false;
            }
            if (DateTimeUtils.IsValidDateTimeFormat(_raceTime,StringUtils.HOUR_MINUTE_FORMAT) == false)
            {
                message = "Invalid Schedule Start Time";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private bool IsEndRaceTimeValid(string _raceTime, out string message)
        {
            if (StringUtils.IsEmpty(_raceTime))
            {
                message = "Race Schedule End Time is Empty";
                return false;
            }
            if (DateTimeUtils.IsValidDateTimeFormat(_raceTime,StringUtils.HOUR_MINUTE_FORMAT) == false)
            {
                message = "Invalid Schedule End Time";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private bool IsLobbyWaitTimeValid(int _lobbyWaitTime, int _raceInterval, out string message)
        {
            if (_lobbyWaitTime <= 0)
            {
                message = "Lobby Wait Time should be greater than zero.";
                return false;
            }
            if (_lobbyWaitTime >= _raceInterval)
            {
                message = "Lobby Wait Time should be less than Race Interval";
                return false;
            }
            message = string.Empty;
            return true;
        }
    }
}
