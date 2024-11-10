using UGS;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceCheckInTab : BaseTab
    {
        [SerializeField] private Button raceCheckInBtn;

        private void OnEnable()
        {
            raceCheckInBtn.onClick.AddListener(() => ConfirmRaceCheckIn());
        }

        private void OnDisable()
        {
            raceCheckInBtn.onClick.RemoveAllListeners();
        }

        private async void ConfirmRaceCheckIn()
        {
            RaceCheckInResponse raceCheckInResponse = await UGSManager.Instance.CloudCode.RaceCheckInRequest(UGSManager.Instance.PlayerData.hostVenueName);
            if (raceCheckInResponse.IsSuccess)
            {
                UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.RaceTimer);
            }
        }

        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.VenueCheckIn);
        }
    }
}
