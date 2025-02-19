using System.Collections.Generic;
using UnityEngine;
using UI.Screen.Tab;
using UnityEngine.UI;

namespace UI.Screen
{
    public class BaseScreen : MonoBehaviour, IScreen
    {
        #region Inspector Variables
        [SerializeField] private ScreenType screenType;
        [SerializeField] private ScreenTabType defaultOpenTab;
        [SerializeField] private List<BaseTab> tabs;
        [SerializeField] private Image backGroundImage;
        #endregion

        #region Private Variables
        private ScreenTabType currentOpenTab;
        #endregion

        #region Properties
        public ScreenType ScreenType => screenType;
        public List<BaseTab> Tabs { get => tabs; }
        public ScreenTabType DefaultOpenTab { get => defaultOpenTab; }
        public ScreenTabType CurrentOpenTab { get => currentOpenTab; }

        public bool IsScreenOpen { get => gameObject.activeSelf; }
        public bool IsAnyTabOpened { get => tabs.Exists(tab => tab.IsOpen); }
        #endregion

        #region Public Methods
        public virtual void Open(ScreenTabType screenTabType)
        {
            if (backGroundImage != null)
            {
                backGroundImage.sprite = UIController.Instance.CurrentTheme.backGround;
                backGroundImage.color = UIController.Instance.CurrentTheme.backGroundTintColor;
            }
            gameObject.SetActive(true);
            //If screenTabType is not None then open the tab
            if (screenTabType != ScreenTabType.None)
            {
                OpenTab(screenTabType);
            }
            else
            {
                //else if defaultOpenTab is not None then open the defaulttab
                if (defaultOpenTab != ScreenTabType.None)
                {
                    OpenTab(defaultOpenTab);
                }
            }
        }
        public virtual void Close()
        {
            CloseAllTabs();
            gameObject.SetActive(false);
        }
        public virtual void Show(ScreenTabType screenTabType)
        {
            gameObject.SetActive(true);
            if (screenTabType != ScreenTabType.None)
            {
                OpenTab(screenTabType);
            }
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }

        public virtual void OnScreenBack()
        {
            //Close the tab that is open and then return.
            if (currentOpenTab != ScreenTabType.None)
            {
                CloseTab(currentOpenTab);
                return;
            }
        }

        #region Tab Methods
        public void OpenTab(ScreenTabType screenTabType)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].ScreenTabType == screenTabType)
                {
                    currentOpenTab = screenTabType;
                    Tabs[i].Open();
                    break;
                }
            }
        }
        public void ChangeTab(ScreenTabType screenTabType)
        {
            CloseTab(currentOpenTab);
            OpenTab(screenTabType);
        }
        public void CloseTab(ScreenTabType screenTabType)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].ScreenTabType == screenTabType)
                {
                    currentOpenTab = ScreenTabType.None;
                    Tabs[i].Close();
                    break;
                }
            }
        }
        public void CloseAllTabs()
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].IsOpen)
                {
                    Tabs[i].Close();
                }
            }
            currentOpenTab = ScreenTabType.None;
        }
        #endregion

        #endregion

    }
    public interface IScreen
    {
        public List<BaseTab> Tabs { get; }
        public ScreenType ScreenType { get; }
        public ScreenTabType DefaultOpenTab { get; }
        public ScreenTabType CurrentOpenTab { get; }
        public bool IsScreenOpen { get; }
        public void OnScreenBack();
        public void Open(ScreenTabType screenTabType);
        public void Close();
        public void Show(ScreenTabType screenTabType);
        public void Hide();
    }
}

