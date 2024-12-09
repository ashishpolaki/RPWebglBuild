using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HorseRace.UI
{
    public class RaceWinnerUIBoard : MonoBehaviour
    {
        [SerializeField] private GameObject bgPanel;
        [SerializeField] private TextMeshProUGUI horseNumberText;
        [SerializeField] private RawImage jockeyAvatar;

        public void SetRaceWinner(int _horseNumber, RenderTexture renderTexture)
        {
            bgPanel.SetActive(true);
            horseNumberText.text = $"Horse #{_horseNumber}";
            jockeyAvatar.texture = renderTexture;
        }
    }
}
