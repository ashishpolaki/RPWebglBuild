using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        #region Inspector Variables
        [SerializeField] private RaceWinnerUIBoard raceWinnerUIBoard;
        [SerializeField] private RacePositionsUIBoard racePositionsBoard;
        [SerializeField] private float racePositionsEnableTimer = 3f;
        #endregion

        #region Property
        private bool CanUpdateUI
        {
            get
            {
                return canUpdateUI;
            }
            set
            {
                canUpdateUI = value;
                if (!canUpdateUI)
                {
                    racePositionsBoard.DisableRacePositions();
                }
            }
        }
        #endregion

        #region Private variables
        private bool canUpdateUI = false;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            EventManager.Instance.OnRaceStartEvent += StartRace;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnRaceStartEvent -= StartRace;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Show Winner Race Ui board at the race end 
        /// </summary>
        /// <param name="_horseNumber"></param>
        /// <param name="jockeyName"></param>
        public void ShowWinnerRaceBoard(int _horseNumber, string jockeyName)
        {
            raceWinnerUIBoard.SetRaceWinner(_horseNumber, jockeyName);
            raceWinnerUIBoard.ShowRaceWinnerBoard();
        }
        /// <summary>
        /// Update Race Positions of the Horses in UI
        /// </summary>
        /// <param name="_racePositions"></param>
        public void UpdateRacePositions(Dictionary<int, int> _racePositions)
        {
            if (CanUpdateUI)
            {
                racePositionsBoard.ShowRacePositions(_racePositions);
            }
        }
        public void EnableRacePositions(bool _value)
        {
            CanUpdateUI = _value;
        }
        #endregion

        #region Private Methods
        private void StartRace()
        {
            StartCoroutine(IEEnableRacePositions());
        }

        /// <summary>
        /// Enable the Race Positions UI after some time from the start of the race.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEEnableRacePositions()
        {
            yield return new WaitForSeconds(racePositionsEnableTimer);
            racePositionsBoard.gameObject.SetActive(true);
            EnableRacePositions(true);
        }
        #endregion

    }
}
