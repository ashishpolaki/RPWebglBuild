using CharacterCustomisation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        public override async void Open(ScreenTabType screenTabType)
        {
            Character character = CharacterCustomisationManager.Instance.InstantiateCharacter();
            await LoadCharacterData(character);
            base.Open(screenTabType);
        }

        public async Task LoadCharacterData(Character character)
        {
            CharacterCustomisationEconomy characterCustomisation = new CharacterCustomisationEconomy();
            Func<Task<List<PlayersInventoryItem>>> method = async () => await UGSManager.Instance.Economy.GetInventoryItem(StringUtils.INVENTORYITEMID_CHARACTER, StringUtils.PLAYERINVENTORYITEMID_CHARACTER);
            List<PlayersInventoryItem> playersInventoryItems = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            string deserializeData = playersInventoryItems[0].InstanceData.GetAsString();

            bool isCharacterDataExist = playersInventoryItems.Count > 0;
            if (isCharacterDataExist)
            {
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