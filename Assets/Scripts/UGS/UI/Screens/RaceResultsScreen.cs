using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class RaceResultsScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI horseJockeyNameTxt;
        [SerializeField] private Button gotoHomeButton;
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
            int firstPlaceHorseNumber = GameManager.Instance.HorsesInRaceOrderList[0];
            foreach (var playerValue in UGSManager.Instance.RaceData.lobbyQualifiedPlayers.Values)
            {
                if(playerValue.Item2 == firstPlaceHorseNumber)
                {
                    horseJockeyNameTxt.text = $"Winner \n {playerValue.Item1} - Horse #{playerValue.Item2}";
                    break;
                }
            }
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