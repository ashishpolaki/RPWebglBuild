using System;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceScheduleTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField timeGap_Input;
        [SerializeField] private InputField preRaceWaitTime_Input;
        [SerializeField] private Button setScheduleBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;

        [SerializeField] private TimeAdjustmentSettings startSchedule;
        [SerializeField] private TimeAdjustmentSettings endSchedule;
        #endregion

        #region Private Variables
        private int raceInterval;
        private int lobbyWaitTime;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            setScheduleBtn.onClick.AddListener(SetSchedule);
            if(UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnRaceScheduleFailEvent += RaceScheduleFailedHandle;
                UGSManager.Instance.CloudCode.OnRaceScheduleSuccessEvent += RaceScheduleSuccessHandle;
            }
        }
        private void OnDisable()
        {
            setScheduleBtn.onClick.RemoveListener(SetSchedule);
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnRaceScheduleFailEvent -= RaceScheduleFailedHandle;
                UGSManager.Instance.CloudCode.OnRaceScheduleSuccessEvent -= RaceScheduleSuccessHandle;
            }
        }
        #endregion

        #region Subscribed Events
        private void RaceScheduleFailedHandle(string error)
        {
            errorMessageTxt.text = error;
        }

        private void RaceScheduleSuccessHandle()
        {
            Close();
        }
        #endregion

        #region Private Methods
        private async void SetSchedule()
        {
            if (!IsRaceIntervalValid() || !IsLobbyWaitTimeValid())
            {
                return;
            }

            Func<Task> method = () => UGSManager.Instance.CloudCode.ScheduleRaceTime(startSchedule.ReturnTime(), endSchedule.ReturnTime(), raceInterval, lobbyWaitTime);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }

        private bool IsRaceIntervalValid()
        {
            if (StringUtils.IsStringEmpty(timeGap_Input.text))
            {
                errorMessageTxt.text = StringUtils.ENTER_RACEINTERVAL;
                return false;
            }
            raceInterval = int.Parse(timeGap_Input.text);
            if (raceInterval <= 0)
            {
                errorMessageTxt.text = StringUtils.RACE_INTERVAL_GREATERTHANZERO;
                return false;
            }
            return true;
        }
       
        private bool IsLobbyWaitTimeValid()
        {
            if (StringUtils.IsStringEmpty(preRaceWaitTime_Input.text))
            {
                errorMessageTxt.text = StringUtils.ENTER_LOBBYWAITTIME;
                return false;
            }

            lobbyWaitTime = int.Parse(preRaceWaitTime_Input.text);
            if (lobbyWaitTime <= 0)
            {

                errorMessageTxt.text = StringUtils.LOBBY_WAITTIME_GREATERTHANZERO;
                return false;
            }

            if (lobbyWaitTime >= raceInterval)
            {
                errorMessageTxt.text = StringUtils.LOBBYWAITTIME_LESSTHAN_RACEINTERVAL;
                return false;
            }

            return true;
        }
        #endregion
    }
}
