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
            StopCoroutine(IEStartTimer());
        }
        private void StartTimer()
        {
            raceStartTimeLeft = raceTime - DateTime.UtcNow;
            waitForSeconds = new WaitForSecondsRealtime(1);
            StartCoroutine(IEStartTimer());
        }
        IEnumerator IEStartTimer()
        {
            StringBuilder sb = new StringBuilder();

            while (raceStartTimeLeft.TotalSeconds >= 0)
            {
                sb.Clear();
                sb.Append("Next Race check-In : \n ")
                  .Append(raceStartTimeLeft.Hours.ToString("D2")).Append(" Hours ")
                  .Append(raceStartTimeLeft.Minutes.ToString("D2")).Append(" Minutes ")
                  .Append(raceStartTimeLeft.Seconds.ToString("D2")).Append(" Seconds ");
                messageText.text = sb.ToString();
                yield return waitForSeconds;
                raceStartTimeLeft = raceStartTimeLeft.Add(TimeSpan.FromSeconds(raceStartTimeDecrement));
            }
            messageText.text = "Race Will Start Soon";
            yield return null;
        }

        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.RaceCheckIn);
        }
    }
}