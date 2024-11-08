using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class LoginAuthenticationScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Image backGroundImage;
        #endregion

        #region Unity Methods
        private void Start()
        {
            backGroundImage.sprite = UIController.Instance.CurrentTheme.backGround;
            backGroundImage.color = UIController.Instance.CurrentTheme.backGroundTintColor;
        }
        #endregion
    }
}