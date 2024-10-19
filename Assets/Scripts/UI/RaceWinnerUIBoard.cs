using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace HorseRace.UI
{
    public class RaceWinnerUIBoard : MonoBehaviour
    {
        [SerializeField] private GameObject bgPanel;
        [SerializeField] private TextMeshProUGUI horseNumberText;
        [SerializeField] private TextMeshProUGUI jockeyNameTxt;
        [SerializeField] private Image jockeyColor;

        public void SetRaceWinner(int _horseNumber, string jockeyName)
        {
            horseNumberText.text = _horseNumber.ToString();
            jockeyNameTxt.text = "Jockey " + _horseNumber.ToString();
          //  jockeyNameTxt.text = jockeyName.ToString();
        }

        public void ShowRaceWinnerBoard()
        {
            bgPanel.SetActive(true);
        }
    }
}
