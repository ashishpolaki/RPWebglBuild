using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HorseRace.UI
{
    public class RacePositionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI horseNumberText;
        [SerializeField] private TextMeshProUGUI positionNumberText;
        [SerializeField] private RawImage avatarRawImage;

        public void SetUI(int _horseNumber, int _positionNumber,RenderTexture renderTexture)
        {
            positionNumberText.text = $"{_positionNumber}";
            horseNumberText.text = $"Horse #{_horseNumber}";
            avatarRawImage.texture = renderTexture;
        }

        public void SetUI(int _positionNumber)
        {
            positionNumberText.text = $"{_positionNumber}";
        }

        public void FinishLineCross(int _positionNumber)
        {
            gameObject.SetActive(true);
            positionNumberText.text = $"{_positionNumber}";
            transform.SetSiblingIndex(_positionNumber - 1);
        }
    }
}