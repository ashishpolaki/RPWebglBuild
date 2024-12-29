using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class RaceTimerTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private int raceStartTimeDecrement = -1;

        private DateTime raceTime;
        private WaitForSecondsRealtime waitForSeconds;
        private TimeSpan raceStartTimeLeft = new TimeSpan();

        private void OnEnable()
        {
            StartTimer();
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }
        private void StartTimer()
        {
            raceTime = UGSManager.Instance.PlayerRaceData.upcomingRaceTime;
            waitForSeconds = new WaitForSecondsRealtime(1);
            StartCoroutine(IEStartTimer());
        }
        IEnumerator IEStartTimer()
        {
            StringBuilder sb = new StringBuilder();
            DateTime currentTime = DateTime.UtcNow;
            bool isCheatCodeActive = false;

#if CHEAT_CODE
            isCheatCodeActive = CheatCode.Instance.IsCheatEnabled;
            if (CheatCode.Instance.IsCheatEnabled)
            {
                currentTime = CheatCode.Instance.GetCheatDateTime();
            }
#endif
            raceStartTimeLeft = raceTime - currentTime;

            while (true)
            {
                if(isCheatCodeActive)
                {
                    raceStartTimeLeft = raceStartTimeLeft.Add(TimeSpan.FromSeconds(raceStartTimeDecrement));
                }
                else
                {
                    raceStartTimeLeft = raceTime - DateTime.UtcNow;
                }

                //Stop the timer if the countdown is over
                if (raceStartTimeLeft.TotalSeconds < 0)
                {
                    break;
                }

                sb.Clear();
                sb.Append("Race Starts in : \n ")
                  .Append(raceStartTimeLeft.Hours.ToString("D2")).Append(" Hours ")
                  .Append(raceStartTimeLeft.Minutes.ToString("D2")).Append(" Minutes ")
                  .Append(raceStartTimeLeft.Seconds.ToString("D2")).Append(" Seconds ");
                messageText.text = sb.ToString();
                yield return waitForSeconds;
            }
            messageText.text = "Race Will Start Soon";
            yield return null;
        }

        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.VenueCheckIn);
        }
    }
}