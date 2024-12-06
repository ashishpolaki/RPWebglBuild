using UnityEngine;
using UI.Screen;
using HorseRace.UI;
using System.Collections.Generic;

namespace UI
{
    public class RaceScreen : BaseScreen
    {
        [SerializeField] private HorseRace.UI.RaceWinnerUIBoard raceWinnerUIBoard;
        [SerializeField] private RacePositionUI racePositionsUIPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private List<RacePositionUI> racePositionsUIList;
        [Tooltip("Key : HorseNumber, Value : RacePositionUI")]
        private Dictionary<int, RacePositionUI> horsesInRaceOrder = new Dictionary<int, RacePositionUI>();
        private void OnEnable()
        {

        }
        private void OnDisable()
        {

        }
        private void InitializeUI()
        {
            int horsesCount = GameManager.Instance.HorsesToSpawnList.Count;
            for (int i = 0; i < horsesCount; i++)
            {
                int horseNumber = GameManager.Instance.HorsesToSpawnList[i];
                RacePositionUI racePositionUI = Instantiate(racePositionsUIPrefab, content);
                racePositionUI.SetUI(horseNumber, 0);
                horsesInRaceOrder.Add(horseNumber, racePositionUI);
            }
        }
        private void ShowRacePositions()
        {

        }
        private void ShowWinnersWithMedals()
        {

        }
    }
}
