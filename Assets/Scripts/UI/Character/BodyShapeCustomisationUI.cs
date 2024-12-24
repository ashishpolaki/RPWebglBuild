using System;
using UnityEngine;
using UnityEngine.UI;

public class BodyShapeCustomisationUI : MonoBehaviour
{
    [SerializeField] private Slider bodyTypeSlider;
    [SerializeField] private Slider bodySizeSlider;
    [SerializeField] private Slider musculatureSlider;
    [SerializeField] private Vector3 position;
    [SerializeField] private float scale = 1f;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float orthoGraphicSize;
    [SerializeField] private RenderTextureSettings renderTextureSettings;

    private Character character;
    private bool isCharacterLoaded;
    private RenderTexture renderTexture;

    public static event Action<Texture, Transform> OnCharacterAssign;

    private void OnEnable()
    {
        bodyTypeSlider.onValueChanged.AddListener((value) => OnBodyTypeValueChanged(value));
        bodySizeSlider.onValueChanged.AddListener((value) => OnBodySizeValueChanged(value));
        musculatureSlider.onValueChanged.AddListener((value) => OnMusculatureValueChanged(value));
        LoadCharacter();
    }
    private void OnDisable()
    {
        bodyTypeSlider.onValueChanged.RemoveAllListeners();
        bodySizeSlider.onValueChanged.RemoveAllListeners();
        musculatureSlider.onValueChanged.RemoveAllListeners();
    }

    #region Private Methods
    private void LoadCharacter()
    {
        if (!isCharacterLoaded)
        {
            isCharacterLoaded = true;
            character = UGSManager.Instance.PlayerData.character;
        }
        SetTransform();
    }
    private void SetTransform()
    {
        character.EnableFullBody();
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

    #region Button Listener Methods
    private void OnBodyTypeValueChanged(float value)
    {
        character.BodyGenderBlendShape(value);
    }
    private void OnBodySizeValueChanged(float value)
    {
        character.BodySizeBlendShape(value);
    }
    private void OnMusculatureValueChanged(float value)
    {
        character.MusculatureBlendShape(value);
    }
    #endregion

}
