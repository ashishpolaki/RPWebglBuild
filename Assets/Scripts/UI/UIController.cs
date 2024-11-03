using UI.Screen;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        #region Inspector Variables
        [SerializeField] private UIScreenManager screenManager;
        [SerializeField] private ThemeConfigSO themeConfig;
        #endregion

        #region Properties
        public ThemeDataSO CurrentTheme { get; private set; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            screenManager.Initialize();

            //Randomly Set Theme
            CurrentTheme = themeConfig.GenerateRandomTheme();
        }
        #endregion

        #region Public Methods
        public void ScreenEvent(ScreenType screenType, UIScreenEvent uIScreenEvent, ScreenTabType screenTabType = ScreenTabType.None)
        {
            screenManager.ScreenEvent(screenType, uIScreenEvent, screenTabType);
        }
        public void ChangeCurrentScreenTab(ScreenTabType screenTabType)
        {
            screenManager.ChangeCurrentScreenTab(screenTabType);
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