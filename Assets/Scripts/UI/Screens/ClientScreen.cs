using Newtonsoft.Json;

namespace UI.Screen
{
    public class ClientScreen : BaseScreen
    {
        #region Unity Methods
        private void OnEnable()
        {
            UGSManager.Instance.CloudCode.OnRaceStarted += OnRaceStart;
            UGSManager.Instance.CloudCode.OnRaceResult += OnRaceResult;
            UGSManager.Instance.Authentication.OnSignedOut += OnSignedOut;
        }
        private void OnDisable()
        {
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.CloudCode.OnRaceStarted -= OnRaceStart;
                UGSManager.Instance.CloudCode.OnRaceResult -= OnRaceResult;
                UGSManager.Instance.Authentication.OnSignedOut -= OnSignedOut;
            }
        }
        #endregion

        private void OnSignedOut()
        {
            UGSManager.Instance.ResetData();
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomisation, UIScreenEvent.Destroy);
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Close);
        }

        #region Inherited Methods
        public override void OnScreenBack()
        {
            //If no tab is opened and the back button is pressed, then close this screen.
            if (CurrentOpenTab == ScreenTabType.None)
            {
                //  UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
                Close();
                return;
            }
            base.OnScreenBack();
        }
        #endregion

        #region CloudCode Trigger Methods
        private void OnRaceStart(string message)
        {
            int horseNumber = int.Parse(message);
            using (PlayerRaceData raceData = new PlayerRaceData())
            {
                raceData.horseNumber = horseNumber;
                UGSManager.Instance.SetPlayerRaceData(raceData);
            }
            CloseAllTabs();
            ScreenTabType screenTabType = horseNumber > 0 ? ScreenTabType.RaceInProgress : ScreenTabType.NotInRace;
            OpenTab(screenTabType);
        }
        private void OnRaceResult(string _raceResult)
        {
            UGS.PlayerRaceResult raceResult = JsonConvert.DeserializeObject<UGS.PlayerRaceResult>(_raceResult);
            PlayerRaceData raceData = new PlayerRaceData();
            raceData.horseNumber = raceResult.HorseNumber;
            raceData.racePosition = raceResult.RacePosition;
            UGSManager.Instance.SetPlayerRaceData(raceData);
            CloseAllTabs();
            OpenTab(ScreenTabType.RaceResults);
        }
        #endregion
    }
}
