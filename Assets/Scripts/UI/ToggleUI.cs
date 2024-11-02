using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ToggleUI : MonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables
        [SerializeField] private bool isOn = false;

        [Header("Toggle UI Elements")]
        [SerializeField] private Image knob; // Knob that moves between On/Off positions
        [SerializeField] private Image bg;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI onText;
        [SerializeField] private TextMeshProUGUI offText;
        #endregion

        #region Properties
        public bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;
                UpdateToggleVisual();
                OnValueChanged?.Invoke(isOn);
            }
        }
        #endregion

        public UnityAction<bool> OnValueChanged;

        #region Unity Methods
        private void Start()
        {
            IsOn = isOn;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }
        #endregion

        #region Public Methods
        public void SetThemeColor()
        {
            ThemeDataSO themeData = UIController.Instance.CurrentTheme;

            //Set Text Color
            text.color = themeData.textColor;
            text.outlineColor = themeData.textOutlineColor;
            onText.color = themeData.textColor;
            onText.outlineColor = themeData.textOutlineColor;
            offText.color = themeData.textColor;
            offText.outlineColor = themeData.textOutlineColor;

            //Set Image Color
            bg.color = themeData.toggleColor;
            bg.GetComponent<Outline>().effectColor = themeData.toggleOutlineColor;
            knob.color = themeData.toggleColor;
            knob.GetComponent<Outline>().effectColor = themeData.toggleOutlineColor;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Toggles the state of the toggle
        /// </summary>
        private void Toggle()
        {
            IsOn = !IsOn; // Flip the state
        }
        /// <summary>
        /// Updates the visual state of the toggle
        /// </summary>
        private void UpdateToggleVisual()
        {
            // Update knob position
            if (knob != null)
            {
                float togglePosX = Mathf.Abs(knob.rectTransform.anchoredPosition.x);
                togglePosX = IsOn ? togglePosX : -togglePosX;
                knob.rectTransform.anchoredPosition = new Vector2(togglePosX, knob.rectTransform.anchoredPosition.y);
            }

            // Update toggle text
            onText.gameObject.SetActive(isOn);
            offText.gameObject.SetActive(!isOn);
        }
        #endregion
    }
}
