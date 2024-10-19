using TMPro;
using UnityEngine;

namespace HorseRace.UI
{
    public class RacePositionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI horseNumberText;
        [SerializeField] private TextMeshProUGUI positionNumberText;

        public void SetUI(int _horseNumber, int _positionNumber)
        {
            positionNumberText.text = $"Pos {_positionNumber}";
            horseNumberText.text = $"Horse {_horseNumber}";
        }
    }
}