using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinColorCustomisationUI : MonoBehaviour
{
    [SerializeField] private Transform colorsParent;
    [SerializeField] private PartColorUI partColorPrefab;

    private Character character;
    private bool isCharacterLoaded;
    private bool isColorDataLoaded;
    private List<PartColorUI> partColorsList = new List<PartColorUI>();

    [Space(10), Header("Render")]
    [SerializeField] private Camera characterCamera;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float orthoGraphicSize;
    [SerializeField] private RenderTextureSettings renderTextureSettings;

    private RenderTexture renderTexture;
    public static event Action<Texture, Transform> OnCharacterAssign;

    private void OnEnable()
    {
        LoadCharacter();
        LoadColorFromCharacter();
    }

    private void LoadCharacter()
    {
        if (!isCharacterLoaded)
        {
            isCharacterLoaded = true;
            character = UGSManager.Instance.PlayerData.character;
        }

        //Create a new Render Texture and assign it to camera.
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(renderTextureSettings.renderTextureSize.x, renderTextureSettings.renderTextureSize.y, renderTextureSettings.colorFormat, renderTextureSettings.depthStencilFormat);
        }

        character.EnableFullBody();
        //Set Camera Position
        characterCamera.transform.position = character.transform.position + cameraOffset;
        characterCamera.targetTexture = renderTexture;
        characterCamera.orthographicSize = orthoGraphicSize;
        characterCamera.gameObject.SetActive(true);

        OnCharacterAssign?.Invoke(renderTexture, character.transform);
    }

    private void LoadColorFromCharacter()
    {
        foreach (var item in partColorsList)
        {
            if (character.CustomisationData.skinToneColor == (Utils.ToHex(item.Color)))
            {
                item.Select();
                break;
            }
        }
    }

    public void SetData(ColorPresetSO colorPresetSO)
    {
        if (!isColorDataLoaded)
        {
            foreach (var color in colorPresetSO.colors)
            {
                PartColorUI partColor = Instantiate(partColorPrefab, colorsParent);
                partColor.SetColor(color);
                partColor.OnColorSelected += OnColorSelected;
                partColorsList.Add(partColor);
            }
            isColorDataLoaded = true;
        }
    }

    private void OnDestroy()
    {
        if (partColorsList != null)
        {
            foreach (var partColor in partColorsList)
            {
                partColor.OnColorSelected -= OnColorSelected;
            }
            partColorsList = null;
        }
    }

    private void OnColorSelected(Color color)
    {
        //Unselect all other colors
        foreach (var partColor in partColorsList)
        {
            partColor.UnSelect();
        }
        character.ChangeSkinToneColor(color);
    }
}
