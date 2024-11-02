using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class WelcomeTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private Button registerTabBtn;
        [SerializeField] private Button loginTabBtn;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            //Button Click Events
            registerTabBtn.onClick.AddListener(() => OpenRegisterTab());
            loginTabBtn.onClick.AddListener(() => OpenLoginTab());
        }
        private void OnDisable()
        {
            //Button Click Events
            registerTabBtn.onClick.RemoveAllListeners();
            loginTabBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        private void OpenLoginTab()
        {
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open, ScreenTabType.LoginPlayer);
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close, ScreenTabType.Welcome);
        }
        private void OpenRegisterTab()
        {
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open, ScreenTabType.RegisterPlayer);
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Close, ScreenTabType.Welcome);
        }
        #endregion
    }
}


