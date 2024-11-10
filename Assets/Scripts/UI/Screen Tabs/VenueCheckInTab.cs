using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class VenueCheckInTab : BaseTab
    {
        #region VenueCheckIn Variables
        [Header("Venue Check-In")]
        [SerializeField] private Button venueCheckInButton;
        [SerializeField] private Button venueCheckInInfoButton;
        [SerializeField] private GameObject venueCheckInCloud;
        [SerializeField] private GameObject venueCheckInBlackPanel;
        [SerializeField] private TextMeshProUGUI venueCheckInMessageText;
        [SerializeField] private TextMeshProUGUI venueCheckInCountText;
        [SerializeField] private int venueCheckInTimeDecrement = 1;

        private TimeSpan venueTimeLeft = new TimeSpan();
        private DateTime nextVenueCheckInTime;
        private WaitForSecondsRealtime venueWaitForSeconds;
        #endregion

        #region EnterRace Variables
        [Space(10), Header("Enter Race")]
        [SerializeField] private Button enterRaceButton;
        [SerializeField] private Button racesWinInfoButton;
        [SerializeField] private Button previousRaceResultInfoButton;
        [SerializeField] private GameObject raceWinsCloud;
        [SerializeField] private GameObject previousRaceResultCloud;
        [SerializeField] private GameObject enterRaceBlackPanel;
        [SerializeField] private GameObject previousRaceResultInfoObject;
        [SerializeField] private TextMeshProUGUI enterRaceMessageText;
        [SerializeField] private TextMeshProUGUI racesWinCountText;
        [SerializeField] private TextMeshProUGUI previousRaceResultText;
        [SerializeField] private int raceCheckInTimeDecrement = 1;

        private DateTime upcomingRaceTime;
        private TimeSpan raceCheckInTimeLeft = new TimeSpan();
        private WaitForSecondsRealtime enterRaceWaitForSeconds;
        private int raceInterval;
        #endregion

        [Space(10)]
        [SerializeField] private Button logOutButton;
        [SerializeField] private Button fetchButton;

        #region Unity Methods
        private void OnEnable()
        {
            //InfoButtons
            venueCheckInInfoButton.onClick.AddListener(() => venueCheckInCloud.gameObject.SetActive(!venueCheckInCloud.activeSelf));
            racesWinInfoButton.onClick.AddListener(() => raceWinsCloud.gameObject.SetActive(!raceWinsCloud.activeSelf));
            previousRaceResultInfoButton.onClick.AddListener(() => previousRaceResultCloud.gameObject.SetActive(!previousRaceResultCloud.activeSelf));

            fetchButton.onClick.AddListener(() => Fetch());
            logOutButton.onClick.AddListener(() => UGSManager.Instance.Authentication.Signout());

            venueCheckInButton.onClick.AddListener(() => OnVenueCheckIn());
            enterRaceButton.onClick.AddListener(() => OnEnterRace());

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GPS.OnLocationResult += HandleLocationResult;
            }

            ResetFields();
            Fetch();
        }
        private void OnDisable()
        {
            //InfoButtons
            venueCheckInInfoButton.onClick.RemoveAllListeners();
            racesWinInfoButton.onClick.RemoveAllListeners();
            previousRaceResultInfoButton.onClick.RemoveAllListeners();

            fetchButton.onClick.RemoveAllListeners();
            logOutButton.onClick.RemoveAllListeners();

            venueCheckInButton.onClick.RemoveAllListeners();
            enterRaceButton.onClick.RemoveAllListeners();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GPS.OnLocationResult -= HandleLocationResult;
            }

            StopCoroutine(IEStartCountDown());
            StopCoroutine(IEStartEnterRaceTimer());
        }
        #endregion

        #region Subscribed Methods
        private async void HandleLocationResult(string message, float latitude, float longitude)
        {
            string hostVenueName = await UGSManager.Instance.GetHostVenueName(latitude, longitude);
            if (StringUtils.IsStringEmpty(hostVenueName))
            {
                venueCheckInBlackPanel.SetActive(true);
                enterRaceBlackPanel.SetActive(true);
                venueCheckInMessageText.text = "No Venue Found at this location";
                enterRaceMessageText.text = "No Venue Found at this location";
            }
            else
            {
                //   await FetchVenueCheckInAsync();
                //  await FetchRaceAsync();
                //Fetch Ongoing Race
                //Fetch Race Results
                RaceCheckInResponse raceCheckInResponse = await UGSManager.Instance.CloudCode.RaceCheckInRequest(UGSManager.Instance.PlayerData.hostVenueName);
                Debug.Log(raceCheckInResponse.Message);
                Debug.Log(raceCheckInResponse.IsSuccess);
            }
            LoadingScreen.Instance.Hide();
        }
        #endregion

        #region Venue CheckIn Methods
        private async Task FetchVenueCheckInAsync()
        {
            VenueCheckInResponse venueCheckInResponse = await UGSManager.Instance.CloudCode.VenueCheckIn(UGSManager.Instance.PlayerData.hostVenueName);
            if (venueCheckInResponse.CanCheckIn)
            {
                venueCheckInMessageText.text = venueCheckInResponse.Message;
            }

            if (!StringUtils.IsStringEmpty(venueCheckInResponse.NextCheckInTime))
            {
                nextVenueCheckInTime = DateTime.Parse(venueCheckInResponse.NextCheckInTime);
                StartCountDownTimer();
            }

            venueCheckInBlackPanel.SetActive(!venueCheckInResponse.CanCheckIn);
            venueCheckInCountText.text = venueCheckInResponse.CheckInCount.ToString();
        }
        private void StartCountDownTimer()
        {
            venueWaitForSeconds = new WaitForSecondsRealtime(1);
            StartCoroutine(IEStartCountDown());
        }
        IEnumerator IEStartCountDown()
        {
            StringBuilder sb = new StringBuilder();
            venueTimeLeft = nextVenueCheckInTime - DateTime.UtcNow;
            while (venueTimeLeft.TotalSeconds >= 0)
            {
                sb.Clear();
                sb.Append("Next Venue check-In : \n ")
                  .Append(venueTimeLeft.Hours.ToString("D2")).Append(" Hours ")
                  .Append(venueTimeLeft.Minutes.ToString("D2")).Append(" Minutes ")
                  .Append(venueTimeLeft.Seconds.ToString("D2")).Append(" Seconds ");
                venueCheckInMessageText.text = sb.ToString();
                yield return venueWaitForSeconds;
                venueTimeLeft = venueTimeLeft.Add(TimeSpan.FromSeconds(venueCheckInTimeDecrement));
            }

            venueCheckInMessageText.text = "Click to Check-In";
            venueCheckInBlackPanel.SetActive(false);
            yield return null;
        }
        private async void OnVenueCheckIn()
        {
            Func<Task<VenueCheckInResponse>> response = () => UGSManager.Instance.CloudCode.SetVenueCheckIn(UGSManager.Instance.PlayerData.hostVenueName);
            VenueCheckInResponse venueCheckInResponse = await LoadingScreen.Instance.PerformAsyncWithLoading(response);

            if (venueCheckInResponse.IsSuccess)
            {
                venueCheckInMessageText.text = venueCheckInResponse.Message;
                venueCheckInCountText.text = venueCheckInResponse.CheckInCount.ToString();
                venueCheckInBlackPanel.SetActive(true);
            }

            if (!StringUtils.IsStringEmpty(venueCheckInResponse.NextCheckInTime))
            {
                DateTime nextCheckInTime = DateTime.Parse(venueCheckInResponse.NextCheckInTime);
                venueTimeLeft = nextCheckInTime - DateTime.UtcNow;
                StartCountDownTimer();
            }
        }
        #endregion

        #region Private Methods
        private void ResetFields()
        {
            enterRaceBlackPanel.SetActive(true);
            venueCheckInBlackPanel.SetActive(true);
            previousRaceResultInfoObject.SetActive(false);
            venueCheckInMessageText.text = string.Empty;
            enterRaceMessageText.text = string.Empty;
            raceInterval = 0;
        }
        private void Fetch()
        {
            if (GPS.IsLocationPermissionGranted())
            {
                LoadingScreen.Instance.Show();
                GameManager.Instance.FetchLocation();
            }
            else
            {
                GPS.RequestPermission();
            }
        }
        #endregion

        #region Enter Race Methods
        private async Task FetchRaceAsync()
        {
            EnterRaceResponse enterRaceResponse = await UGSManager.Instance.CloudCode.EnterRaceRequest(UGSManager.Instance.PlayerData.hostVenueName);

            if (enterRaceResponse.IsFoundUpcomingRace)
            {
                if (!StringUtils.IsStringEmpty(enterRaceResponse.UpcomingRaceTime))
                {
                    upcomingRaceTime = DateTime.Parse(enterRaceResponse.UpcomingRaceTime);
                    raceInterval = enterRaceResponse.RaceInterval;
                    CheckRaceCheckIn();
                }
            }
            else
            {
                enterRaceMessageText.text = enterRaceResponse.Message;
            }
        }

        private void CheckRaceCheckIn()
        {
            TimeSpan timeUntilNextRace = upcomingRaceTime - DateTime.UtcNow;
            bool canCheckInRace = timeUntilNextRace.TotalSeconds <= raceInterval;
            if (canCheckInRace)
            {
                enterRaceMessageText.text = "Click to Enter";
                enterRaceBlackPanel.SetActive(false);
            }
            else
            {
                StartEnterRaceTimer();
            }
        }

        private void StartEnterRaceTimer()
        {
            enterRaceWaitForSeconds = new WaitForSecondsRealtime(1);
            StartCoroutine(IEStartEnterRaceTimer());
        }

        IEnumerator IEStartEnterRaceTimer()
        {
            StringBuilder sb = new StringBuilder();

            raceCheckInTimeLeft = (upcomingRaceTime - DateTime.UtcNow) + new TimeSpan(0, -raceInterval, 0);
            while (raceCheckInTimeLeft.TotalSeconds >= 0)
            {
                sb.Clear();
                sb.Append("Next Race check-In : \n ")
                  .Append(raceCheckInTimeLeft.Hours.ToString("D2")).Append(" Hours ")
                  .Append(raceCheckInTimeLeft.Minutes.ToString("D2")).Append(" Minutes ")
                  .Append(raceCheckInTimeLeft.Seconds.ToString("D2")).Append(" Seconds ");
                enterRaceMessageText.text = sb.ToString();
                yield return enterRaceWaitForSeconds;
                raceCheckInTimeLeft = raceCheckInTimeLeft.Add(TimeSpan.FromSeconds(raceCheckInTimeDecrement));
            }

            enterRaceMessageText.text = "Click to Enter";
            enterRaceBlackPanel.SetActive(false);
            yield return null;
        }

        private void OnEnterRace()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.RaceCheckIn);
        }
        #endregion
    }
}