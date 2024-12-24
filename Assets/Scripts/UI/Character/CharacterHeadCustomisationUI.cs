using CharacterCustomisation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHeadCustomisationUI : MonoBehaviour
{
    [SerializeField] private PartStyleUI partStyleUIPrefab;
    [SerializeField] private PartColorUI partColorUIPrefab;
    [SerializeField] private Button partStyleButton;
    [SerializeField] private Button partColorButton;
    [SerializeField] private TextMeshProUGUI partNameText;
    [SerializeField] private Transform scrollParent;
    [SerializeField] private Sprite selectedTabSprite;
    [SerializeField] private Sprite unSelectedTabSprite;

    [Space(10), Header("Body Layout")]
    [SerializeField] private RectTransform bodyLayoutRect;
    [SerializeField] private float bodyLayOutNormalHeight;
    [SerializeField] private float bodyLayOutExtendedHeight;
    [SerializeField] private float bodyLayOutNormalYPos;
    [SerializeField] private float bodyLayOutExtendedYPos;

    [Space(10), Header("layout Group")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Vector2 colorCellSize;
    [SerializeField] private Vector2 styleCellSize;

    [Space(10), Header("Live Render Texture")]
    [SerializeField] private Vector3 position;
    [SerializeField] private float scale = 1f;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float orthoGraphicSize;
    [SerializeField] private RenderTextureSettings renderTextureSettings;
    private bool isCharacterLoaded;
    private RenderTexture renderTexture;
    private Character character;

    [Space(10), Header("Capture Render Texture")]
    [SerializeField] private Vector3 headCaptureCharacterPosition;
    [SerializeField] private CaptureTextureSettings[] captureTextureSettings;
    private Character headCaptureCharacterInstance;
    private bool isHeadCaptureCharacterLoaded;

    private CharacterPartSO currentPartSO;
    private Dictionary<BlendPartType, List<PartStyleUI>> storedPartStyles = new Dictionary<BlendPartType, List<PartStyleUI>>();
    private Dictionary<BlendPartType, List<PartColorUI>> storedPartColors = new Dictionary<BlendPartType, List<PartColorUI>>();
    public static event Action<Texture, Transform> OnCharacterAssign;

    private void OnEnable()
    {
        partStyleButton.onClick.AddListener(OnStyleTabClick);
        partColorButton.onClick.AddListener(OnColorTabClick);
    }
    private void OnDisable()
    {
        partStyleButton.onClick.RemoveAllListeners();
        partColorButton.onClick.RemoveAllListeners();
    }
    private void OnDestroy()
    {
        if (headCaptureCharacterInstance != null)
        {
            Destroy(headCaptureCharacterInstance.gameObject);
        }
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        if (storedPartColors != null)
        {
            foreach (var partColorUIList in storedPartColors.Values)
            {
                foreach (var partColorUI in partColorUIList)
                {
                    partColorUI.OnColorSelected -= OnColorSelected;
                }
            }
        }
    }
    public void OnPartSelected(CharacterPartSO characterPartSO)
    {
        DisableStoredStyles();
        DisableStoredColors();
        LoadCharacter();
        currentPartSO = characterPartSO;
        OnArrangeTabs(characterPartSO);
    }

    private void OnArrangeTabs(CharacterPartSO characterPartSO)
    {
        bool arePartStylesExist = characterPartSO.parts.Length > 0;
        bool arePartColorsExist = characterPartSO.colorPreset != null;

        if (arePartStylesExist && arePartColorsExist)
        {
            partStyleButton.gameObject.SetActive(true);
            partColorButton.gameObject.SetActive(true);
            bodyLayoutRect.sizeDelta = new Vector2(bodyLayoutRect.sizeDelta.x, bodyLayOutNormalHeight);
            bodyLayoutRect.anchoredPosition = new Vector2(bodyLayoutRect.anchoredPosition.x, bodyLayOutNormalYPos);
            OnStyleTabClick();
        }
        else
        {
            //deactive the two buttons and extend the scrollParent height
            partStyleButton.gameObject.SetActive(false);
            partColorButton.gameObject.SetActive(false);
            bodyLayoutRect.sizeDelta = new Vector2(bodyLayoutRect.sizeDelta.x, bodyLayOutExtendedHeight);
            bodyLayoutRect.anchoredPosition = new Vector2(bodyLayoutRect.anchoredPosition.x, bodyLayOutExtendedYPos);

            if (arePartStylesExist)
            {
                partNameText.text = $"{currentPartSO.partName} Style";
                SpawnStyles();
            }
            else if (arePartColorsExist)
            {
                partNameText.text = $"{currentPartSO.partName} Color";
                SpawnColors();
            }
        }
    }

    private CaptureTextureSettings GetCaptureTextureSettings(BlendPartType blendPartType)
    {
        foreach (var part in captureTextureSettings)
        {
            if (part.blendPartType == blendPartType)
            {
                return part;
            }
        }
        return new CaptureTextureSettings();
    }

    #region Style
    private void OnStyleTabClick()
    {
        //Change sprites for selected style and unselected color
        partStyleButton.transform.SetAsLastSibling(); //Make it appear infront of scrollParent
        partColorButton.transform.SetAsFirstSibling(); // Make it behind of scrollParent
        partStyleButton.image.sprite = selectedTabSprite;
        partColorButton.image.sprite = unSelectedTabSprite;
        partNameText.text = $"{currentPartSO.partName} Style";
        DisableStoredColors();
        SpawnStyles();
    }
    private void SpawnStyles()
    {
        gridLayoutGroup.cellSize = styleCellSize;

        if (!storedPartStyles.ContainsKey(currentPartSO.partType))
        {
            StartCoroutine(IESpawnStyles());
        }

        if (storedPartStyles.ContainsKey(currentPartSO.partType))
        {
            foreach (var partStyleUI in storedPartStyles[currentPartSO.partType])
            {
                partStyleUI.gameObject.SetActive(true);
            }
        }
    }
    IEnumerator IESpawnStyles()
    {
        List<PartStyleUI> partStyleUIList = new List<PartStyleUI>();
        CaptureTextureSettings captureTextureSettings = GetCaptureTextureSettings(currentPartSO.partType);
        switch (currentPartSO.partType)
        {
            case BlendPartType.Nose:
                headCaptureCharacterInstance.EnableNose();
                break;
            case BlendPartType.Eyebrows:
                headCaptureCharacterInstance.EnableEyebrows();
                break;
            case BlendPartType.Hair:
                headCaptureCharacterInstance.EnableHair();
                break;
            case BlendPartType.FacialHair:
                headCaptureCharacterInstance.EnableFacialHair();
                break;
            case BlendPartType.Ears:
                headCaptureCharacterInstance.EnableEars();
                break;
        }

        //Capture Styles in UI with the character parts
        foreach (var partNumber in currentPartSO.parts)
        {
            var part = Instantiate(partStyleUIPrefab, scrollParent);

            if (partNumber != -1)
            {
                headCaptureCharacterInstance.ChangePartInHead(currentPartSO.partType, partNumber);
                yield return null;
                RenderTexture renderTexture = GameManager.Instance.CaptureObject.CaptureWithCustom(headCaptureCharacterInstance.gameObject, captureTextureSettings.Offset, captureTextureSettings.FieldOfView, captureTextureSettings.RenderTextureSize);
                part.SetData(this, renderTexture, partNumber, currentPartSO.partType);
            }
            else
            {
                part.SetStyleNone(this, partNumber, currentPartSO.partType);
            }
            partStyleUIList.Add(part);
        }
        storedPartStyles.Add(currentPartSO.partType, partStyleUIList);
    }
    private void DisableStoredStyles()
    {
        if (currentPartSO == null)
        {
            return;
        }

        if (storedPartStyles.ContainsKey(currentPartSO.partType))
        {
            foreach (var partStyleUI in storedPartStyles[currentPartSO.partType])
            {
                partStyleUI.UnSelect();
                partStyleUI.gameObject.SetActive(false);
            }
        }
    }
    public void SetPartStyle(int partIndex, BlendPartType blendPartType)
    {
        //DeSelect all the other styles
        foreach (var partStyleUI in storedPartStyles[blendPartType])
        {
            partStyleUI.UnSelect();
        }
        character.ChangePartInHead(blendPartType, partIndex);
    }
    #endregion

    #region Color
    private void SpawnColors()
    {
        gridLayoutGroup.cellSize = colorCellSize;
        if (!storedPartColors.ContainsKey(currentPartSO.partType))
        {
            List<PartColorUI> partColorsUIList = new List<PartColorUI>();

            foreach (var color in currentPartSO.colorPreset.colors)
            {
                PartColorUI partColor = Instantiate(partColorUIPrefab, scrollParent);
                partColor.SetColor(color);
                partColor.OnColorSelected += OnColorSelected;
                partColorsUIList.Add(partColor);
            }
            storedPartColors.Add(currentPartSO.partType, partColorsUIList);
        }

        if (storedPartColors.ContainsKey(currentPartSO.partType))
        {
            foreach (var partColorUI in storedPartColors[currentPartSO.partType])
            {
                partColorUI.gameObject.SetActive(true);
            }
        }
    }
    private void DisableStoredColors()
    {
        if (currentPartSO == null)
        {
            return;
        }

        if (storedPartColors.ContainsKey(currentPartSO.partType))
        {
            foreach (var partColorUI in storedPartColors[currentPartSO.partType])
            {
                partColorUI.UnSelect();
                partColorUI.gameObject.SetActive(false);
            }
        }
    }
    public void OnColorSelected(Color color)
    {
        //Unselect all the other colors
        foreach (var partColorUI in storedPartColors[currentPartSO.partType])
        {
            partColorUI.UnSelect();
        }
        character.ChangePartColorInHead(Utils.ToHex(color), currentPartSO.partType);
    }

    private void OnColorTabClick()
    {
        //Change sprites for selected color and unselected style
        partColorButton.transform.SetAsLastSibling(); //Make it appear infront of scrollParent
        partStyleButton.transform.SetAsFirstSibling(); // Make it behind of scrollParent
        partColorButton.image.sprite = selectedTabSprite;
        partStyleButton.image.sprite = unSelectedTabSprite;
        partNameText.text = $"{currentPartSO.partName} Color";
        DisableStoredStyles();
        SpawnColors();
    }
    #endregion

    #region Character
    private void LoadCharacter()
    {
        if (!isCharacterLoaded)
        {
            isCharacterLoaded = true;
            character = UGSManager.Instance.PlayerData.character;
        }
        //Capture Styles with the character parts
        if (!isHeadCaptureCharacterLoaded)
        {
            isHeadCaptureCharacterLoaded = true;
            headCaptureCharacterInstance = CharacterCustomisationManager.Instance.InstantiateCharacter();
            headCaptureCharacterInstance.GetComponent<Animator>().enabled = false;
            headCaptureCharacterInstance.transform.position = headCaptureCharacterPosition;
            headCaptureCharacterInstance.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        SetTransform();
    }

    private void SetTransform()
    {
        character.EnableFace();
        character.transform.position = position;
        character.transform.eulerAngles = new Vector3(0, 180, 0);
        character.transform.localScale = Vector3.one * scale;

        //Create a new Render Texture and assign it to camera.
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(renderTextureSettings.renderTextureSize.x, renderTextureSettings.renderTextureSize.y, renderTextureSettings.colorFormat, renderTextureSettings.depthStencilFormat);
        }

        //Set Camera Position
        characterCamera.transform.position = character.transform.position + cameraOffset;
        characterCamera.targetTexture = renderTexture;
        characterCamera.orthographicSize = orthoGraphicSize;
        characterCamera.gameObject.SetActive(true);

        OnCharacterAssign?.Invoke(renderTexture, character.transform);
    }
    #endregion

}
