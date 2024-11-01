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
            if (UGSManager.Instance.RaceData.horseNumber == 0)
            {
                horseNumberTxt.text = $"“Your ticket was not selected. The longer you spend at the venue and check-in, the greater the odds.”";
            }
            else
            {
                horseNumberTxt.text = $"Horse Number : {UGSManager.Instance.RaceData.horseNumber}";
            }
        }
        #endregion
    }
}
