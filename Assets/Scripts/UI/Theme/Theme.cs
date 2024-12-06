using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class Theme : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textGroup;
    [SerializeField] private TextMeshProUGUI[] buttonTextGroup;
    [SerializeField] private TextMeshProUGUI[] errorTextGroup;
    [SerializeField] private TextMeshProUGUI[] inputFieldTextGroup;
    [SerializeField] private Image[] bodyBGImageGroup;
    [SerializeField] private Image[] bodyImageGroup;
    [SerializeField] private Image[] inputFieldBGImageGroup;
    [SerializeField] private Image[] inputFieldImageGroup;
    [SerializeField] private Image[] cloudCommentImageGroup;
    [SerializeField] private Image[] characterImageGroup;
    [SerializeField] private Image[] toggleImageGroup;

    private void Awake()
    {
        ThemeDataSO themeData = UIController.Instance.CurrentTheme;
        SetThemeData(themeData);
    }

    private void SetThemeData(ThemeDataSO themeData)
    {
        SetSprites(characterImageGroup, themeData.character);
        SetTextColors(textGroup, themeData.textColor, themeData.textOutlineColor);
        SetTextColors(buttonTextGroup, themeData.buttonTextColor, themeData.buttonTextOutlineColor);
        SetTextColors(errorTextGroup, themeData.errorTextColor, themeData.errorTextColor);
        SetTextColors(inputFieldTextGroup, themeData.inputFieldTextColor, themeData.inputFieldTextColor);
        SetImageColors(cloudCommentImageGroup, themeData.cloudColor, themeData.cloudOutlineColor);
        SetImageColors(bodyBGImageGroup, themeData.bodyBGColor, themeData.bodyBGOutlineColor);
        SetImageColors(bodyImageGroup, themeData.bodyColor, themeData.bodyOutlineColor);
        SetImageColors(inputFieldBGImageGroup, themeData.inputFieldBGColor, themeData.inputFieldBGOutlineColor);
        SetImageColors(inputFieldImageGroup, themeData.inputFieldColor);
        SetImageColors(toggleImageGroup, themeData.toggleColor, themeData.toggleOutlineColor);
    }
    private void SetSprites(Image[] imageGroup, Sprite sprite)
    {
        if (imageGroup == null) return;
        foreach (var item in imageGroup)
        {
            if (item != null)
            {
                item.sprite = sprite;
            }
        }
    }
    private void SetImageColors(Image[] imageGroup, Color color, Color? outlineColor = null)
    {
        if (imageGroup == null) return;
        foreach (var item in imageGroup)
        {
            if (item != null)
            {
                item.color = color;
                var outline = item.GetComponent<Outline>();
                if (outline != null && outlineColor.HasValue)
                {
                    outline.effectColor = outlineColor.Value;
                }
            }
        }
    }
    private void SetTextColors(TextMeshProUGUI[] textGroup, Color color, Color outlineColor)
    {
        if (textGroup == null) return;
        foreach (var text in textGroup)
        {
            if (text != null)
            {
                text.color = color;
                text.outlineColor = outlineColor;
            }
        }
    }

#if UNITY_EDITOR
    //Create a method for inspector to signout
    [ContextMenu("Set Theme")]
    public void SetTheme()
    {
        ThemeDataSO themeData = UIController.Instance.CurrentTheme;
        SetThemeData(themeData);
    }
#endif

}
