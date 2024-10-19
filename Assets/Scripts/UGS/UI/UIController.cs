using UI.Screen;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        #region Inspector Variables
        [SerializeField] private UIScreenManager screenManager;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            screenManager.Initialize();
        }
        #endregion
        
        #region Public Methods
        public void ScreenEvent(ScreenType screenType, UIScreenEvent uIScreenEvent, ScreenTabType screenTabType = ScreenTabType.None)
        {
            screenManager.ScreenEvent(screenType, uIScreenEvent,screenTabType);
        }
        #endregion
    }
}
public enum UIScreenEvent
{
    Open,
    Close,
    Show,
    Hide
}