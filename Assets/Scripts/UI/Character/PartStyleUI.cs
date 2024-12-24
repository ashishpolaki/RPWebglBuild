using UnityEngine;
using UnityEngine.UI;

public class PartStyleUI : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Image selectImage;
    [SerializeField] private GameObject noneImage;

    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color selectedColor;
    private int partIndex;
    private BlendPartType blendPartType;
    private CharacterHeadCustomisationUI characterHeadCustomisationUI;

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
        characterHeadCustomisationUI.SetPartStyle(partIndex, blendPartType);
        Select();
    }

    public void SetData(CharacterHeadCustomisationUI characterHeadCustomisationUI, RenderTexture renderTexture, int partIndex, BlendPartType blendPartType)
    {
        rawImage.texture = renderTexture;
        this.characterHeadCustomisationUI = characterHeadCustomisationUI;
        this.partIndex = partIndex;
        this.blendPartType = blendPartType;
    }

    public void SetStyleNone(CharacterHeadCustomisationUI characterHeadCustomisationUI, int partIndex, BlendPartType blendPartType)
    {
        // rawImage.texture = noneSprite.texture;
        rawImage.gameObject.SetActive(false);
        noneImage.SetActive(true);
        this.characterHeadCustomisationUI = characterHeadCustomisationUI;
        this.partIndex = partIndex;
        this.blendPartType = blendPartType;
    }

}
