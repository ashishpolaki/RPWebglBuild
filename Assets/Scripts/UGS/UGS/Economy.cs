using UnityEngine;
using Unity.Services.Economy;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UGS
{
    public class Economy
    {
        public Economy()
        {

        }

        public async void InitializeEconomy()
        {
            await EconomyService.Instance.Configuration.SyncConfigurationAsync();
        }

        public async Task GetInventoryItem(string itemID)
        {
            GetInventoryOptions options = new GetInventoryOptions
            {
                InventoryItemIds = new List<string>() { itemID }
            };

            GetInventoryResult upperBodyParts = await EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
        }

        public async Task AddInventoryItem(string itemID, EconomyCustom economy, string instanceId = "")
        {
            AddInventoryItemOptions options = new AddInventoryItemOptions
            {
                InstanceData = economy,
                PlayersInventoryItemId = instanceId
            };

            PlayersInventoryItem createdInventoryItem = await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(itemID, options);
        }

        public async Task UpdateInventoryItem(string itemId, EconomyCustom instanceData)
        {
            PlayersInventoryItem updatedInventoryItem = await EconomyService.Instance.PlayerInventory.UpdatePlayersInventoryItemAsync(itemId, instanceData);
        }

    }
}
