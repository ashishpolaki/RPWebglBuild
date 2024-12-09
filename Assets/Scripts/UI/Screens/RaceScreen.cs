using UnityEngine;
using UI.Screen;
using HorseRace.UI;
using System.Collections.Generic;
using HorseRace;
using System.Collections;

namespace UI
{
    public class RaceScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private RaceWinnerUIBoard raceWinnerUIBoard;
        [SerializeField] private RacePositionUI racePositionsUIPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private float racePositionsEnableTimer = 3f;
        #endregion

        #region Private Variables
        [Tooltip("Key : HorseNumber, Value : RacePositionUI")]
        private Dictionary<int, RacePositionUI> horseRacePositionsUI = new Dictionary<int, RacePositionUI>();
        private int finishLineCount = 0;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            InitializeUI();
        }
        private void OnEnable()
        {
            EventManager.Instance.OnRaceStartEvent += StartRace;
            EventManager.Instance.OnRacePositionsEvent += ShowRacePositions;
            EventManager.Instance.OnRaceUIActiveEvent += RacePostionsUIActive;
            EventManager.Instance.OnCrossFinishLineEvent += OnFinishLineCross;
            EventManager.Instance.OnRaceWinnerEvent += ShowWinnersWithMedals;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnRaceStartEvent -= StartRace;
            EventManager.Instance.OnRacePositionsEvent -= ShowRacePositions;
            EventManager.Instance.OnRaceUIActiveEvent -= RacePostionsUIActive;
            EventManager.Instance.OnCrossFinishLineEvent -= OnFinishLineCross;
            EventManager.Instance.OnRaceWinnerEvent -= ShowWinnersWithMedals;
        }
        #endregion

        #region Event Handlers
        private void OnFinishLineCross(int horseNumber)
        {
            ++finishLineCount;
            horseRacePositionsUI[horseNumber].FinishLineCross(finishLineCount);
        }
        private void StartRace()
        {
            StartCoroutine(IEEnableRacePositionsUI());
        }
        private void ShowRacePositions(Dictionary<int, int> racePositions)
        {
            foreach (var racePosition in racePositions)
            {
                horseRacePositionsUI[racePosition.Value].SetUI(racePosition.Key);
                horseRacePositionsUI[racePosition.Value].transform.SetSiblingIndex(racePosition.Key - 1);
            }
        }
        private void ShowWinnersWithMedals(int horseNumber)
        {
            raceWinnerUIBoard.SetRaceWinner(horseNumber, UGSManager.Instance.HostRaceData.currentRaceAvatars[horseNumber]);
        }
        #endregion

        #region Private Methods
        private IEnumerator IEEnableRacePositionsUI()
        {
            yield return new WaitForSeconds(racePositionsEnableTimer);
            RacePostionsUIActive(true);
        }
        private void RacePostionsUIActive(bool active)
        {
            foreach (var item in horseRacePositionsUI)
            {
                item.Value.gameObject.SetActive(active);
            }
        }
        private void InitializeUI()
        {
            for (int i = 0; i < UGSManager.Instance.HostRaceData.currentRaceAvatars.Count; i++)
            {
                int horseNumber = GameManager.Instance.HorsesToSpawnList[i];
                RacePositionUI racePositionUI = Instantiate(racePositionsUIPrefab, content);
                racePositionUI.SetUI(horseNumber, 0, UGSManager.Instance.HostRaceData.currentRaceAvatars[horseNumber]);
                horseRacePositionsUI.Add(horseNumber, racePositionUI);
            }
        }
        #endregion
    }
}
