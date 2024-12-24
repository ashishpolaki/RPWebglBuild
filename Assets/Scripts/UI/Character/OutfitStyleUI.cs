using UnityEngine;
using UnityEngine.UI;

public class OutfitStyleUI : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Image selectImage;

    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color selectedColor;
    private int partIndex;
    private OutfitType outfitType;
    private OutfitCustomisationUI outfitCustomisationUI;

    private void OnEnable()
    {
        Button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(OnButtonClick);
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
        outfitCustomisationUI.SetOutfit(partIndex, outfitType);
        Select();
    }

    public void SetData(OutfitCustomisationUI outfitCustomisationUI, RenderTexture renderTexture, int partIndex, OutfitType outfitType)
    {
        rawImage.texture = renderTexture;
        this.outfitCustomisationUI = outfitCustomisationUI;
        this.partIndex = partIndex;
        this.outfitType = outfitType;
    }
}
