using System;
using System.Threading.Tasks;
using TMPro;
using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class SetVenueNameTab : BaseTab
    {
        [SerializeField] private Button logOutButton;
        [SerializeField] private Button setVenueNameButton;
        [SerializeField] private TMP_InputField venueInputField;
        [SerializeField] private TextMeshProUGUI errorMessageText;

        private void OnEnable()
        {
            ResetFields();

            if (logOutButton != null)
                logOutButton.onClick.AddListener(() => OnLogOut());
            setVenueNameButton.onClick.AddListener(() => OnSetVenueName());
        }
        private void OnDisable()
        {
            if (logOutButton != null)
                logOutButton.onClick.RemoveAllListeners();

            setVenueNameButton.onClick.RemoveAllListeners();
        }

        private void OnLogOut()
        {
            UGSManager.Instance.Authentication.Signout();
        }

        private void ResetFields()
        {
            venueInputField.text = string.Empty;
            errorMessageText.text = string.Empty;
        }

        private async void OnSetVenueName()
        {
            if (StringUtils.IsStringEmpty(venueInputField.text))
            {
                errorMessageText.text = StringUtils.VENUE_NAME_EMPTY_ERROR;
                return;
            }

            if (venueInputField.text.Length > 15)
            {
                errorMessageText.text = StringUtils.VENUE_NAME_LENGTH_ERROR;
                return;
            }

            VenueRegistrationRequest venueRegistrationRequest = new VenueRegistrationRequest();
            venueRegistrationRequest.Name = venueInputField.text;

            Func<Task<SetVenueNameResponse>> method = () => UGSManager.Instance.CloudCode.SetVenueName(venueRegistrationRequest);
            SetVenueNameResponse setVenueNameResponse = await LoadingScreen.Instance.PerformAsyncWithLoading(method);

            if (setVenueNameResponse.IsVenueNameSet)
            {
                //Change Tab
                UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.RegisterVenue);

                VenueRegistrationData venueRegistrationData = new VenueRegistrationData();
                venueRegistrationData.Name = venueInputField.text;
                UGSManager.Instance.SetVenueRegistrationData(venueRegistrationData);
            }
            else
            {
                errorMessageText.text = setVenueNameResponse.Message;
            }

            if(setVenueNameResponse != null)
            {
                setVenueNameResponse.Dispose();
            }
        }

    }
}