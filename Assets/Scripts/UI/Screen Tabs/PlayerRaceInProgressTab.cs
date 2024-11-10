using TMPro;
using UnityEngine;

namespace UI.Screen.Tab
{
    public class PlayerRaceInProgressTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI horseNumberTxt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            horseNumberTxt.text = $"Horse Number : {UGSManager.Instance.RaceData.horseNumber}";
        }
        #endregion

        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.VenueCheckIn);
        }
    }
}
