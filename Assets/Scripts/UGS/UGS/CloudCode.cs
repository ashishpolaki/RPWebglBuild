using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode.Subscriptions;
using UnityEngine;

namespace UGS
{
    public class CloudCode
    {
        #region Private Variables
        private HorseRaceCloudCodeBindings module;
        #endregion

        #region Events
        public event Action<string> OnRaceStarted;
        public event Action<string> OnRaceResult;

        public event Action OnVenueRegistrationSuccessEvent;
        public event Action<string> OnVenueRegistrationFailEvent;

        public event Action<string> OnRaceScheduleFailEvent;
        public event Action OnRaceScheduleSuccessEvent;

        public event Action<string> OnRaceStartFailEvent;
        public event Action OnRaceStartSuccessEvent;
        #endregion

        public CloudCode()
        {
        }
        public async void InitializeBindings()
        {
            await Task.Delay(1000);
            module = new HorseRaceCloudCodeBindings(CloudCodeService.Instance);
        }

        #region Private Methods
        private bool IsGPSValid(double _latitude, double _longitude)
        {
            return GPS.IsValidGpsLocation(_latitude, _longitude);
        }
        private bool IsRadiusMinimumValid(float _radius)
        {
            return _radius >= HostConfig.radiusMin;
        }
        private bool IsRadiusMaximumValid(float _radius)
        {
            return _radius <= HostConfig.radiusMax;
        }
        private bool IsRaceScheduleValid(string scheduleStart, string scheduleEnd, int raceInterval)
        {
            if (!DateTimeUtils.IsValidDateTimeFormat(scheduleStart) || !DateTimeUtils.IsValidDateTimeFormat(scheduleEnd))
            {
                OnRaceScheduleFailEvent?.Invoke(StringUtils.INVALID_DATETIME_FORMAT);
                return false;
            }

            DateTime startSchedule = DateTimeUtils.ConvertToUTCTime(scheduleStart, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            DateTime endSchedule = DateTimeUtils.ConvertToUTCTime(scheduleEnd, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);

            // Check if start and end schedule strings are equal
            if (DateTimeUtils.AreDateTimesEqual(startSchedule, endSchedule))
            {
                OnRaceScheduleFailEvent?.Invoke(StringUtils.START_AND_END_RACESCHEDULE_EQUAL);
                return false;
            }

            //If end time is less than start time, add a day to end time
            if (startSchedule > endSchedule)
            {
                endSchedule = endSchedule.AddDays(1);
            }

            //Check if raceInterval is greater than raceTime
            TimeSpan raceTimeSpan = endSchedule - startSchedule;
            TimeSpan raceIntervalSpan = TimeSpan.FromMinutes(raceInterval);
            if (raceTimeSpan < raceIntervalSpan)
            {
                OnRaceScheduleFailEvent?.Invoke(StringUtils.INVALID_RACEINTERVAL_LIMIT);
                return false;
            }
            return true;
        }
        #endregion

        #region Public Methods
        public Task SubscribeToPlayerMessages()
        {
            var callbacks = new SubscriptionEventCallbacks();
            callbacks.MessageReceived += @event =>
            {
                switch (@event.MessageType)
                {
                    case "RaceStart":
                        OnRaceStarted?.Invoke(@event.Message);
                        break;
                    case "RaceResult":
                        OnRaceResult?.Invoke(@event.Message);
                        break;
                    default:
                        Debug.Log($"Got unsupported player Message:");
                        break;
                }
            };
            return CloudCodeService.Instance.SubscribeToPlayerMessagesAsync(callbacks);
        }

        //Methods
        public async Task RegisterVenue(double _latitude, double _longitude, float _radius)
        {
            // GPS Validation
            if (IsGPSValid(_latitude, _longitude) == false)
            {
                OnVenueRegistrationFailEvent?.Invoke(StringUtils.GPSVALIDLOCATIONERROR);
                return;
            }

            //Radius Validation
            if (IsRadiusMinimumValid(_radius) == false)
            {
                OnVenueRegistrationFailEvent?.Invoke(StringUtils.GPSRADIUSMINERROR);
                return;
            }

            if (IsRadiusMaximumValid(_radius) == false)
            {
                OnVenueRegistrationFailEvent?.Invoke(StringUtils.GPSRADIUSMAXERROR);
                return;
            }

            VenueRegistrationRequest venueRegistrationRequest = new VenueRegistrationRequest();
            try
            {
                venueRegistrationRequest.Latitude = _latitude;
                venueRegistrationRequest.Longitude = _longitude;
                venueRegistrationRequest.Radius = _radius;
                var response = await module.RegisterVenue(venueRegistrationRequest);
                if(response.IsRegistered)
                {
                    OnVenueRegistrationSuccessEvent?.Invoke();
                }
                else
                {
                    OnVenueRegistrationFailEvent?.Invoke(response.Message);
                }
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                venueRegistrationRequest.Dispose();
            }
        }


        public async Task ScheduleRaceTime(string scheduleStart, string scheduleEnd, int raceInterval, int preRaceWaitTime)
        {
            if (IsRaceScheduleValid(scheduleStart, scheduleEnd, raceInterval) == false)
            {
                return;
            }
            HostScheduleRace hostScheduleRace = new HostScheduleRace();
            try
            {
                hostScheduleRace.ScheduleStart = DateTimeUtils.ConvertToUTCTime(scheduleStart, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT).ToString(StringUtils.HOUR_MINUTE_TIME_FORMAT);
                hostScheduleRace.ScheduleEnd = DateTimeUtils.ConvertToUTCTime(scheduleEnd, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT).ToString(StringUtils.HOUR_MINUTE_TIME_FORMAT);
                hostScheduleRace.TimeGap = raceInterval;
                hostScheduleRace.PreRaceWaitTime = preRaceWaitTime;

                string jsonData = JsonConvert.SerializeObject(hostScheduleRace);
                await module.ScheduleRaceTimings(jsonData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (hostScheduleRace != null)
                {
                    hostScheduleRace.Dispose();
                }
                OnRaceScheduleSuccessEvent?.Invoke();
            }
        }

        public async Task<string> VenueCheckIn(string hostID, string dateTime)
        {
            try
            {
                var result = await module.VenueCheckIn(hostID, dateTime);
                return result.Message;
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return string.Empty;
        }

        public async Task<JoinRaceResponse> RequestRaceJoin(string hostID, string dateTime)
        {
            JoinRaceResponse joinRaceResponse = new JoinRaceResponse();
            try
            {
                joinRaceResponse = await module.RequestRaceJoin(hostID, dateTime);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return joinRaceResponse;
        }
        public async Task<bool> ConfirmRaceCheckIn(string hostID, string playerName)
        {
            try
            {
                return await module.ConfirmRaceCheckIn(hostID, playerName);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return false;
        }
        public async Task StartRace(Dictionary<string, (string, int)> lobbyQualifiedPlayers, List<string> notQualifiedPlayersList)
        {
            List<RaceLobbyParticipant> raceLobbyParticipants = new List<RaceLobbyParticipant>();
            foreach (var lobbyPlayer in lobbyQualifiedPlayers)
            {
                if (StringUtils.IsStringEmpty(lobbyPlayer.Key))
                {
                    OnRaceStartFailEvent?.Invoke(StringUtils.PLAYERID_EMPTY);
                    return;
                }
                if (StringUtils.IsStringEmpty(lobbyPlayer.Value.Item1))
                {
                    OnRaceStartFailEvent?.Invoke(StringUtils.PLAYERNAME_EMPTY);
                    return;
                }
                if (lobbyPlayer.Value.Item2 <= 0)
                {
                    OnRaceStartFailEvent?.Invoke(StringUtils.HORSENUMBER_INVALID);
                    return;
                }
                raceLobbyParticipants.Add(new RaceLobbyParticipant
                {
                    PlayerID = lobbyPlayer.Key,
                    PlayerName = lobbyPlayer.Value.Item1,
                    HorseNumber = lobbyPlayer.Value.Item2
                });
            }

            if (raceLobbyParticipants.Count > 12)
            {
                OnRaceStartFailEvent?.Invoke(StringUtils.MAXIMUM_LOBBYPLAYERS_EXCEEDED);
                return;
            }

            StartRaceRequest startRaceRequest = new StartRaceRequest();
            startRaceRequest.RaceLobbyParticipants = raceLobbyParticipants;
            startRaceRequest.UnQualifiedPlayerIDs = notQualifiedPlayersList;
            try
            {
                StartRaceResponse startRaceResponse = await module.StartRace(startRaceRequest);
                if (startRaceResponse.IsRaceStart)
                {
                    OnRaceStartSuccessEvent?.Invoke();
                }
                else
                {
                    Debug.LogError(startRaceResponse.Message);
                }
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                raceLobbyParticipants.Clear();
                startRaceRequest.Dispose();
            }
        }
        public async Task SendRaceResults(RaceResult raceResultsData)
        {
            try
            {
                await module.SendRaceResults(raceResultsData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
        #endregion
    }

}
