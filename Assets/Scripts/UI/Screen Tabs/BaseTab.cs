using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class BaseTab : MonoBehaviour, IScreenTab
    {
        #region Inspector Variables
        [SerializeField] private ScreenTabType screenTabType;
        [SerializeField] private Button backButton;
        #endregion

        #region Properties
        public ScreenTabType ScreenTabType => screenTabType;
        public bool IsOpen { get => gameObject.activeSelf; }
        #endregion

        #region Unity methods
        protected virtual void Start()
        {
        }
        #endregion

        #region Public Methods
        public virtual void Close()
        {
            gameObject.SetActive(false);
            if (backButton != null)
                backButton.onClick.RemoveAllListeners();
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
            if (backButton != null)
                backButton.onClick.AddListener(() => OnTabBack());
        }
        protected virtual void OnTabBack()
        {

        }
        #endregion
    }
    public interface IScreenTab
    {
        public ScreenTabType ScreenTabType { get; }
        public bool IsOpen { get; }
        public void Open();
        public void Close();
    }
}

