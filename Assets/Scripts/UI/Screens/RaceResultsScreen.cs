using HorseRace.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class RaceResultsScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button gotoHomeButton;
        [SerializeField] private List<RaceWinnerUIBoard> raceWinnerUIBoardList;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            gotoHomeButton.onClick.AddListener(Home);
        }
        private void OnDisable()
        {
            gotoHomeButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region Inherited Methods
        public override void Open(ScreenTabType screenTabType)
        {
            base.Open(screenTabType);
            int count = Mathf.Min(GameManager.Instance.HorsesInRaceOrderList.Count, 3);
            for (int i = 0; i < count; i++)
            {
                int horseNumber = GameManager.Instance.HorsesInRaceOrderList[i];
                raceWinnerUIBoardList[i].SetRaceWinner(horseNumber, UGSManager.Instance.HostRaceData.currentRaceAvatars[horseNumber]);
            }
            UGSManager.Instance.HostRaceData.Dispose();
        }
        #endregion

        #region Button Listener Methods
        private void Home()
        {
            Action action = () =>
            {
                UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
            };
            UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
            //Change the screen orientation to Portrait, after uploading the results in cloud.
            LoadingScreen.Instance.LoadSceneAdditiveAsync((int)Scene.Menu, action);
        }
        #endregion
    }
}