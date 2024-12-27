using UGS;

namespace UI.Screen
{
    public class HostScreen : BaseScreen
    {
        private async void OnEnable()
        {
            VenueRegistrationRequest venueRegistrationResponse = await UGSManager.Instance.CloudSave.GetVenueRegistrationDataAsync("HostVenue", UGSManager.Instance.PlayerData.playerID);
            if (venueRegistrationResponse != null)
            {
                if(StringUtils.IsStringEmpty(venueRegistrationResponse.Name))
                {
                    OpenTab(ScreenTabType.SetVenueName);
                }
                else
                {
                    VenueRegistrationData venueRegistrationData = new VenueRegistrationData();
                    venueRegistrationData.Name = venueRegistrationResponse.Name;
                    venueRegistrationData.Latitude = venueRegistrationResponse.Latitude;
                    venueRegistrationData.Longitude = venueRegistrationResponse.Longitude;
                    UGSManager.Instance.SetVenueRegistrationData(venueRegistrationData);

                    ScreenTabType screenTabType = venueRegistrationResponse.Latitude == 0 ? ScreenTabType.RegisterVenue : ScreenTabType.HostSetting;
                    OpenTab(screenTabType);
                }
            }
            else
            {
                OpenTab(ScreenTabType.SetVenueName);
            }

            UGSManager.Instance.Authentication.OnSignedOut += LogOutHandle;
        }

        private void OnDisable()
        {
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedOut -= LogOutHandle;
            }
        }

        #region Private Methods
        private void LogOutHandle()
        {
            UGSManager.Instance.ResetData();
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Destroy);    
        }
        #endregion

        public override void OnScreenBack()
        {
            if (CurrentOpenTab == ScreenTabType.None)
            {
                Close();
            }
            base.OnScreenBack();
        }
    }
}
