using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class BaseTab : MonoBehaviour, IScreenTab
    {
        #region Inspector Variables
        [SerializeField] private ScreenTabType screenTabType;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI[] textGroup;
        [SerializeField] private Image CloudCommentImage;
        [SerializeField] private Image characterImage;
        [SerializeField] private Image[] bodyBGImage;
        [SerializeField] private Image[] bodyImage;
        #endregion

        #region Properties
        public ScreenTabType ScreenTabType => screenTabType;
        public bool IsOpen { get => gameObject.activeSelf; }
        #endregion

        #region Unity methods
        private void Start()
        {
            SetTheme();
        }
        #endregion

        #region Public Methods
        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
        public void SetTheme()
        {
            ThemeDataSO themeData = UIController.Instance.CurrentTheme;

            //Set sprite
            characterImage.sprite = themeData.character;

            //Set color
            foreach (TextMeshProUGUI text in textGroup)
            {
                text.color = themeData.textColor;
                text.outlineColor = themeData.textOutlineColor;
            }
            CloudCommentImage.color = themeData.cloudColor;
            CloudCommentImage.GetComponent<Outline>().effectColor = themeData.cloudOutlineColor;

            foreach (var item in bodyBGImage)
            {
                item.color = themeData.bodyBGColor;
                item.GetComponent<Outline>().effectColor = themeData.bodyBGOutlineColor;
            }
            foreach (var item in bodyImage)
            {
                item.color = themeData.bodyColor;
                item.GetComponent<Outline>().effectColor = themeData.bodyOutlineColor;
            }
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

