using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceScheduleTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TMP_InputField raceTimingsInput;
        [SerializeField] private TMP_InputField raceIntervalInput;
        [SerializeField] private Button setScheduleBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;

        [SerializeField] private TimeAdjustmentSettings startSchedule;
        [SerializeField] private TimeAdjustmentSettings endSchedule;
        #endregion

        #region Private Variables
        private int raceInterval;
        private int raceTimings;
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
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.HostSetting);
        }
        #endregion

        #region Private Methods
        private async void SetSchedule()
        {
            errorMessageTxt.text = string.Empty;
            if (!IsRaceIntervalValid() || !IsRaceTimingsValid())
            {
                return;
            }

            Func<Task> method = () => UGSManager.Instance.CloudCode.ScheduleRaceTime(startSchedule.ReturnTime(), endSchedule.ReturnTime(), raceTimings, raceInterval);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }

        private bool IsRaceIntervalValid()
        {
            if (StringUtils.IsStringEmpty(raceIntervalInput.text))
            {
                errorMessageTxt.text = StringUtils.ENTER_RACEINTERVAL;

                return false;
            }
            raceInterval = int.Parse(raceIntervalInput.text);
            if (raceInterval <= 0)
            {
                errorMessageTxt.text = StringUtils.RACEINTERVAL_GREATERTHANZERO;
                return false;
            }
            return true;
        }
       
        private bool IsRaceTimingsValid()
        {
            if (StringUtils.IsStringEmpty(raceTimingsInput.text))
            {
                errorMessageTxt.text = StringUtils.ENTER_RACETIMINGS;
                return false;
            }

            raceTimings = int.Parse(raceTimingsInput.text);
            if (raceTimings <= 0)
            {
                errorMessageTxt.text = StringUtils.RACE_TIMINGS_GREATERTHANZERO;
                return false;
            }

            if (raceInterval >= raceTimings)
            {
                errorMessageTxt.text = StringUtils.RACEINTERVAL_LESSTHAN_RACETIMINGS;
                return false;
            }

            return true;
        }
        #endregion
    }
}
