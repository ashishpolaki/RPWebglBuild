using System;
using UnityEngine;
using UnityEngine.UI;

public class HorseColorUI : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Image selectImage;

    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color selectedColor;

    private int partIndex;
    public int PartIndex => partIndex;

    public Action<int> OnHorseColorSelectedAction;

    private void OnEnable()
    {
        Button.onClick.AddListener(OnButtonClick);
    }
    private void OnDisable()
    {
        Button.onClick.RemoveListener(OnButtonClick);
    }
    private void OnDestroy()
    {
        OnHorseColorSelectedAction = null;
    }
    public void Select()
    {
        selectImage.color = selectedColor;
    }
    public void UnSelect()
    {
        selectImage.color = unSelectedColor;
    }
    private void OnButtonClick()
    {
        OnHorseColorSelectedAction?.Invoke(partIndex);
        Select();
    }

    public void SetData(Texture renderTexture, int partIndex)
    {
        rawImage.texture = renderTexture;
        this.partIndex = partIndex;
    }
}
