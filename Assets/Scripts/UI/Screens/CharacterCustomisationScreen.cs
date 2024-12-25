using CharacterCustomisation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using UnityEngine;
using UGS;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        private bool isCharacterDataExist = false;
        public override async void Open(ScreenTabType screenTabType)
        {
            Character character = CharacterCustomisationManager.Instance.InstantiateCharacter();
            await LoadCharacterData(character);
            if (isCharacterDataExist)
            {
                UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomisation, UIScreenEvent.Close);
            }
            else
            {
                base.Open(screenTabType);
            }
        }

        public async Task LoadCharacterData(Character character)
        {
            CharacterCustomisationEconomy characterCustomisation = new CharacterCustomisationEconomy();
            Func<Task<List<PlayersInventoryItem>>> method = async () => await UGSManager.Instance.Economy.GetInventoryItem(StringUtils.INVENTORYITEMID_CHARACTER, StringUtils.PLAYERINVENTORYITEMID_CHARACTER);
            List<PlayersInventoryItem> playersInventoryItems = await LoadingScreen.Instance.PerformAsyncWithLoading(method);

             isCharacterDataExist = playersInventoryItems.Count > 0;
            if (isCharacterDataExist)
            {
                string deserializeData = playersInventoryItems[0].InstanceData.GetAsString();
                characterCustomisation = JsonUtility.FromJson<CharacterCustomisationEconomy>(deserializeData);
            }
            using (PlayerData playerData = new PlayerData())
            {
                playerData.character = character;
                playerData.character.Load(characterCustomisation);
                UGSManager.Instance.SetPlayerData(playerData);
            }
        }

    }
}