using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class HostScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button startRaceBtn;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button scheduleRaceBtn;
        [SerializeField] private Button backButton;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            startRaceBtn.onClick.AddListener(() => StartRace());
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
            scheduleRaceBtn.onClick.AddListener(() => ScheduleRace());
            backButton.onClick.AddListener(() => OnScreenBack());
        }
        private void OnDisable()
        {
            startRaceBtn.onClick.RemoveAllListeners();
            registerVenueBtn.onClick.RemoveAllListeners();
            scheduleRaceBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        private void RegisterVenue()
        {
            OpenTab(ScreenTabType.RegisterVenue);
        }
        private async void StartRace()
        {
            await UGSManager.Instance.SetLobbyPlayers();
            OpenTab(ScreenTabType.Lobby);
        }
        private void ScheduleRace()
        {
            OpenTab(ScreenTabType.RaceSchedule);
        }
        #endregion

        public override void OnScreenBack()
        {
            if (CurrentOpenTab == ScreenTabType.None)
            {
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
                Close();
            }
            base.OnScreenBack();
        }
    }
}
