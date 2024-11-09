using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HostSettingTab : BaseTab
    {
        [SerializeField] private Button scheduleRaceButton;
        [SerializeField] private Button startRaceButton;
        [SerializeField] private Button logOutButton;

        private void OnEnable()
        {
            scheduleRaceButton.onClick.AddListener(() => OnScheduleRace());
            startRaceButton.onClick.AddListener(() => OnStartRace());
            logOutButton.onClick.AddListener(() => OnLogOut());
        }

        private void OnDisable()
        {
            scheduleRaceButton.onClick.RemoveAllListeners();
            startRaceButton.onClick.RemoveAllListeners();
            logOutButton.onClick.RemoveAllListeners();
        }

        #region Private Methods

        private void OnScheduleRace()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.RaceSchedule);
        }

        private void OnStartRace()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.Lobby);
        }

        private void OnLogOut()
        {
            UGSManager.Instance.Authentication.Signout();
        }

        #endregion

    }
}