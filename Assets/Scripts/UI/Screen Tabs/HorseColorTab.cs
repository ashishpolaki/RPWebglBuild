using HorseRace;
using UGS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI.Screen.Tab
{
    public class HorseColorTab : BaseTab
    {
        [SerializeField] private HorseJockeyMaterials horseJockeyMaterials;
        [SerializeField] private HorseColorUI horseColorUIPrefab;
        [SerializeField] private Transform scrollContent;
        [SerializeField] private Horse horse;
        [SerializeField] private Horse horsePreview;
        [SerializeField] private Button submitButton;

        private List<HorseColorUI> horseColorUIList = new List<HorseColorUI>();
        private int currentBodyColorIndex = 0;
        private RenderTexture renderTexture;

        [Space(10), Header("Horse Render Settings")]
        [SerializeField] private TextureAndRotateHandler textureAndRotateHandler;
        [SerializeField] private Vector3 position;
        [SerializeField] private Camera horseCamera;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private float orthoGraphicSize;
        [SerializeField] private RenderTextureSettings renderTextureSettings;
        [SerializeField] private Vector3 horsePreviewPosition;
        [SerializeField] private Vector3 horsePreviewCameraOffset;
        [SerializeField] private float horsePreviewOrthoGraphicSize;
        [SerializeField] private Vector2Int horsePreviewRenderTextureSize;

        public override async void Open()
        {
            base.Open();
            LoadHorses();
            StartCoroutine(IESpawnHorsePreviews());
            //  InstantiateHorseColorUI();
            // await LoadHorseData();
        }

        private void OnEnable()
        {
            submitButton.onClick.AddListener(() => OnSubmitButton());
        }

        IEnumerator IESpawnHorsePreviews()
        {
            int index = 0;
            foreach (var horseColor in horseJockeyMaterials.horseMaterials)
            {
                HorseColorUI horseColorUI = Instantiate(horseColorUIPrefab, scrollContent);
                horsePreview.SetHorseMaterial(horseColor);
                yield return null;
                RenderTexture renderTexture = GameManager.Instance.CaptureObject.CaptureWithCustom(horsePreview.gameObject, horsePreviewCameraOffset, horsePreviewOrthoGraphicSize, horsePreviewRenderTextureSize);
                horseColorUI.SetData(renderTexture, index);
                horseColorUI.OnHorseColorSelectedAction += HandleHorseColorSelection;
                horseColorUIList.Add(horseColorUI);
                index++;
            }
        }

        private void OnDisable()
        {
            submitButton.onClick.RemoveAllListeners();
        }

        private async void OnSubmitButton()
        {
            await Save();
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
            UIController.Instance.ScreenEvent(ScreenType.HorseCustomisation, UIScreenEvent.Close);
        }

        private void LoadHorses()
        {
            horse.transform.position = position;
            horsePreview.transform.position = horsePreviewPosition;

            //Create a new Render Texture and assign it to camera.
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(renderTextureSettings.renderTextureSize.x, renderTextureSettings.renderTextureSize.y, renderTextureSettings.colorFormat, renderTextureSettings.depthStencilFormat);
            }

            //Set Horse Camera Position
            horseCamera.transform.position = horse.transform.position + cameraOffset;
            horseCamera.targetTexture = renderTexture;
            horseCamera.orthographicSize = orthoGraphicSize;
            horseCamera.gameObject.SetActive(true);
            textureAndRotateHandler.OnCharacterAssign(renderTexture, horse.transform);
        }

        public async Task Save()
        {
            HorseCustomisationEconomy horseCustomisationEconomy = new HorseCustomisationEconomy();
            horseCustomisationEconomy.bodyColorIndex = currentBodyColorIndex;

            Func<Task<List<PlayersInventoryItem>>> method = async () => await UGSManager.Instance.Economy.GetInventoryItem(StringUtils.INVENTORYITEMID_HORSE, StringUtils.PLAYERINVENTORYITEMID_HORSE);
            List<PlayersInventoryItem> playersInventoryItems = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            bool isHorseDataExist = playersInventoryItems.Count > 0;
            if (isHorseDataExist)
            {
                //Update 
                await UGSManager.Instance.Economy.UpdateInventoryItem(StringUtils.PLAYERINVENTORYITEMID_HORSE, horseCustomisationEconomy);
            }
            else
            {
                //Add
                await UGSManager.Instance.Economy.AddInventoryItem(StringUtils.INVENTORYITEMID_HORSE, horseCustomisationEconomy, StringUtils.PLAYERINVENTORYITEMID_HORSE);
            }
        }

        public async Task LoadHorseData()
        {
            HorseCustomisationEconomy horseCustomisationEconomy = new HorseCustomisationEconomy();
            Func<Task<List<PlayersInventoryItem>>> method = async () => await UGSManager.Instance.Economy.GetInventoryItem(StringUtils.INVENTORYITEMID_HORSE, StringUtils.PLAYERINVENTORYITEMID_HORSE);
            List<PlayersInventoryItem> playersInventoryItems = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            bool isHorseDataExist = playersInventoryItems.Count > 0;
            int partIndex = 0;
            if (isHorseDataExist)
            {
                string deserializeData = playersInventoryItems[0].InstanceData.GetAsString();
                horseCustomisationEconomy = JsonUtility.FromJson<HorseCustomisationEconomy>(deserializeData);
                partIndex = horseCustomisationEconomy.bodyColorIndex;
            }
            HandleHorseColorSelection(partIndex);
            horseColorUIList[partIndex].Select();
        }

        private void HandleHorseColorSelection(int partIndex)
        {
            foreach (var horseColorUI in horseColorUIList)
            {
                horseColorUI.UnSelect();
            }
            currentBodyColorIndex = partIndex;
            horse.SetHorseMaterial(horseJockeyMaterials.horseMaterials[partIndex]);
        }
    }
}

