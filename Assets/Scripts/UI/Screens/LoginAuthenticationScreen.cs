using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class LoginAuthenticationScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private ToggleUI hostToggle;
        [SerializeField] private Image backGroundImage;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            hostToggle.OnValueChanged += OnHostToggleHandle;
        }
        private void OnDisable()
        {
            hostToggle.OnValueChanged -= OnHostToggleHandle;
        }
        private void Start()
        {
            hostToggle.SetThemeColor();
            backGroundImage.sprite = UIController.Instance.CurrentTheme.backGround;
            backGroundImage.color = UIController.Instance.CurrentTheme.backGroundTintColor;
        }
        #endregion

        #region Subscribed Methods
        private void OnHostToggleHandle(bool val)
        {
            UGSManager.Instance.SetHost(val);
        }
        #endregion

    }
}