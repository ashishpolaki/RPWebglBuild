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
            resultText.text = $"Your are place \n #{UGSManager.Instance.RaceData.racePosition} in the race";
        }
        #endregion

        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.VenueCheckIn);
        }
    }
}