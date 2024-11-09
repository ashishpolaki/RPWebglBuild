using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RegisterVenueTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TMP_InputField radiusInput;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button fetchCurrentLocationBtn;
        [SerializeField] private TextMeshProUGUI messageTxt;
        #endregion

        private float latitude;
        private float longitude;

        #region Unity Methods
        private void OnEnable()
        {
            ResetFields();
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
            fetchCurrentLocationBtn.onClick.AddListener(() => FetchCurrentLocation());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnVenueRegistrationFailEvent += (message) => messageTxt.text = message;
                UGSManager.Instance.CloudCode.OnVenueRegistrationSuccessEvent += () => OnVenueRegistrationSuccessful();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GPS.OnLocationResult += HandleLocationResult;
            }
        }
        private void OnDisable()
        {
            registerVenueBtn.onClick.RemoveAllListeners();
            fetchCurrentLocationBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnVenueRegistrationFailEvent -= (message) => messageTxt.text = message;
                UGSManager.Instance.CloudCode.OnVenueRegistrationSuccessEvent -= () => OnVenueRegistrationSuccessful();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GPS.OnLocationResult -= HandleLocationResult;
            }
        }
        #endregion

        #region Subscribed Methods
        private void HandleLocationResult(string message, float latitude, float longitude)
        {
            messageTxt.text = message;
            bool isLocationValid = latitude != 0 && longitude != 0;
            this.latitude = latitude;
            this.longitude = longitude;
            registerVenueBtn.interactable = isLocationValid;

            //Set local values
            VenueRegistrationData venueRegistrationData = new VenueRegistrationData();
            venueRegistrationData.Latitude = latitude;
            venueRegistrationData.Longitude = longitude;
            UGSManager.Instance.SetVenueRegistrationData(venueRegistrationData);

            LoadingScreen.Instance.Hide();
        }
        private void OnVenueRegistrationSuccessful()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.HostSetting);
        }
        #endregion

        #region Private Methods
        private void ResetFields()
        {
            registerVenueBtn.interactable = false;
            radiusInput.text = string.Empty;
            latitude = 0;
            longitude = 0;
        }
        private void FetchCurrentLocation()
        {
            LoadingScreen.Instance.Show();
            GameManager.Instance.FetchLocation();
        }
        private async void RegisterVenue()
        {
            messageTxt.text = string.Empty;

            //Check if Radius is empty
            if (StringUtils.IsStringEmpty(radiusInput.text))
            {
                messageTxt.text = StringUtils.GPSRADIUSEMPTY;
                return;
            }

            //Register Host in Venue Cloud List.
            float radius = float.Parse(radiusInput.text);
            Func<Task> method = () => UGSManager.Instance.CloudCode.RegisterVenue(latitude, longitude, radius);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion
    }
}
