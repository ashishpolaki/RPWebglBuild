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
        [SerializeField] private InputField locationLatitude;
        [SerializeField] private InputField locationLongitude;
        [SerializeField] private InputField radiusInput;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button fetchCurrentLocationBtn;
        [SerializeField] private TextMeshProUGUI messageTxt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
            fetchCurrentLocationBtn.onClick.AddListener(() => FetchCurrentLocation());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnVenueRegistrationFailEvent += (message) => messageTxt.text = message;
                UGSManager.Instance.CloudCode.OnVenueRegistrationSuccessEvent += () => Close();
            }
        }
        private void OnDisable()
        {
            registerVenueBtn.onClick.RemoveAllListeners();
            fetchCurrentLocationBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnVenueRegistrationFailEvent -= (message) => messageTxt.text = message;
                UGSManager.Instance.CloudCode.OnVenueRegistrationSuccessEvent -= () => Close();
            }
        }
        #endregion

        #region Private Methods
        private async void FetchCurrentLocation()
        {
            Func<Task<bool>> method = () => GPS.TryGetLocationAsync();
            bool islocationFound = await LoadingScreen.Instance.PerformAsyncWithLoading<bool>(method);
            if (islocationFound)
            {
                locationLatitude.text = GPS.CurrentLocationLatitude.ToString();
                locationLongitude.text = GPS.CurrentLocationLongitude.ToString();
            }
        }
        private async void RegisterVenue()
        {
            messageTxt.text = string.Empty;

            //Check if Latitude and Longitude are empty
            if (StringUtils.IsStringEmpty(locationLatitude.text) || StringUtils.IsStringEmpty(locationLongitude.text))
            {
                messageTxt.text = StringUtils.GPSLOCATIONFETCHERROR;
                return;
            }

            //Check if Radius is empty
            if (StringUtils.IsStringEmpty(radiusInput.text))
            {
                messageTxt.text = StringUtils.GPSRADIUSEMPTY;
                return;
            }

            //Register Host in Venue Cloud List.
            double latitude = double.Parse(locationLatitude.text);
            double longitude = double.Parse(locationLongitude.text);
            float radius = float.Parse(radiusInput.text);
            Func<Task> method = () => UGSManager.Instance.CloudCode.RegisterVenue(latitude, longitude, radius);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion
    }
}
