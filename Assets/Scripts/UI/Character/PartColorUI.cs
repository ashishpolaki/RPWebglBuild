using UnityEngine;
using UnityEngine.UI;

public class PartColorUI : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    [SerializeField] private Image selectImage;
    [SerializeField] private Button button;
    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color selectedColor;
    private Color color;

    public Color Color => color;

    public event System.Action<Color> OnColorSelected;

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        OnColorSelected?.Invoke(color);
        Select();
    }
    public void Select()
    {
        selectImage.color = selectedColor;
    }
    public void UnSelect()
    {
        selectImage.color = unSelectedColor;
    }
    public void SetColor(Color _color)
    {
        color = _color;
        colorImage.color = color;
    }
}
