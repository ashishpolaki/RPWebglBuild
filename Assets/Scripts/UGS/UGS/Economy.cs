using Unity.Services.Economy;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using System.Collections.Generic;

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

        public async Task<List<PlayersInventoryItem>> GetInventoryItem(string itemID, string playersInventoryItemId)
        {
            GetInventoryOptions options = new GetInventoryOptions
            {
                InventoryItemIds = new List<string>() { itemID, },
                PlayersInventoryItemIds = new List<string>() { playersInventoryItemId },
            };

            GetInventoryResult inventoryResult = await EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
            return inventoryResult.PlayersInventoryItems;
        }
        public async Task AddInventoryItem(string itemID, EconomyCustom economy, string playersInventoryItemId = "")
        {
            AddInventoryItemOptions options = new AddInventoryItemOptions
            {
                InstanceData = economy,
                PlayersInventoryItemId = playersInventoryItemId
            };

            await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(itemID, options);
        }
        public async Task UpdateInventoryItem(string itemId, EconomyCustom instanceData)
        {
            await EconomyService.Instance.PlayerInventory.UpdatePlayersInventoryItemAsync(itemId, instanceData);
        }
    }
}
