using HorseRace;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UGS;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HorseColorTab : BaseTab
    {
        [SerializeField] private HorseJockeyMaterials horseJockeyMaterials;
        [SerializeField] private HorseColorUI horseColorUIPrefab;
        [SerializeField] private Transform scrollContent;
        [SerializeField] private Horse horse;
        [SerializeField] private Button submitButton;

        private List<HorseColorUI> horseColorUIList = new List<HorseColorUI>();
        private int currentBodyColorIndex = 0;

        [Space(10), Header("Character Render Settings")]
        [SerializeField] private Vector3 position;
        [SerializeField] private float scale = 1f;
        [SerializeField] private Camera characterCamera;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private float orthoGraphicSize;
        [SerializeField] private RenderTextureSettings renderTextureSettings;

        public override async void Open()
        {
            base.Open();
            InstantiateHorseColorUI();
            await LoadHorseData();
        }

        private void OnEnable()
        {
            submitButton.onClick.AddListener(() => OnSubmitButton());
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


        private void InstantiateHorseColorUI()
        {
            int index = 0;
            foreach (var horseColor in horseJockeyMaterials.horseMaterials)
            {
                HorseColorUI horseColorUI = Instantiate(horseColorUIPrefab, scrollContent);
                horseColorUI.SetData(horseColor.mainTexture, index);
                horseColorUI.OnHorseColorSelectedAction += HandleHorseColorSelection;
                horseColorUIList.Add(horseColorUI);
                index++;
            }
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

