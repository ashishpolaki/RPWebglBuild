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
            resultText.text = $"Your race position is {UGSManager.Instance.RaceData.racePosition}";
        }
        #endregion
    }
}