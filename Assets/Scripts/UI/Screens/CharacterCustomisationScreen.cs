using CharacterCustomisation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        private void Awake()
        {
            Character character = CharacterCustomisationManager.Instance.InstantiateCharacter();
            LoadCharacterData(character);
        }

        public async void LoadCharacterData(Character character)
        {
            CharacterCustomisationEconomy characterCustomisation = new CharacterCustomisationEconomy();
            Func<Task<List<PlayersInventoryItem>>> method = async () => await UGSManager.Instance.Economy.GetInventoryItem(StringUtils.INVENTORYITEMID_CHARACTER, StringUtils.PLAYERINVENTORYITEMID_CHARACTER);
            List<PlayersInventoryItem> playersInventoryItems = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            bool isCharacterDataExist = playersInventoryItems.Count > 0;
            if (isCharacterDataExist)
            {
                characterCustomisation = playersInventoryItems[0].InstanceData.GetAs<CharacterCustomisationEconomy>();
            }
            using (PlayerData playerData = new PlayerData())
            {
                playerData.character = character;
                playerData.character.Load(characterCustomisation);
                UGSManager.Instance.SetPlayerData(playerData);
            }
            OpenTab(ScreenTabType.CharacterBodyCustomize);
        }

    }
}