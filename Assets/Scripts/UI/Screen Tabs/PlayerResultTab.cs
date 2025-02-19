using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class PlayerResultTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI resultText;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            resultText.text = $"Your are placed \n #{UGSManager.Instance.PlayerRaceData.racePosition} in the race";
        }
        #endregion

        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.VenueCheckIn);
        }
    }
}