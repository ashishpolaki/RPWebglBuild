using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace HorseRaceCloudCode
{
    public interface ICheatCode
    {
        public void ActivateCheatCode(string playerId, string dateTime, bool isActive);

        public bool IsCheatCodeActive(string playerId);
        public DateTime CurrentDateTime(string playerId);
    }
    public class CheatCode : ICheatCode
    {
        private Dictionary<string, bool> playersCheatDictionary = new Dictionary<string, bool>();
        private Dictionary<string, DateTime> playersDateTimeDictionary = new Dictionary<string, DateTime>();

        public void ActivateCheatCode(string playerId, string dateTime, bool isActive)
        {
            if (playersCheatDictionary.ContainsKey(playerId))
            {
                playersCheatDictionary[playerId] = isActive;
            }
            else
            {
                playersCheatDictionary.Add(playerId, isActive);
            }

            if (playersDateTimeDictionary.ContainsKey(playerId))
            {
                playersDateTimeDictionary[playerId] = DateTime.Parse(dateTime);
            }
            else
            {
                playersDateTimeDictionary.Add(playerId, DateTime.Parse(dateTime));
            }

        }
        public DateTime CurrentDateTime(string playerId)
        {
            if (playersDateTimeDictionary.ContainsKey(playerId))
            {
                return playersDateTimeDictionary[playerId];
            }
            else
            {
                return DateTime.UtcNow;
            }

        }
        public bool IsCheatCodeActive(string playerId)
        {
            if (playersCheatDictionary.ContainsKey(playerId))
            {
                return playersCheatDictionary[playerId];
            }
            else
            {
                return false;
            }
        }
    }

    public class SetCheatCodeClass
    {
        private readonly IGameApiClient gameApiClient;

        public SetCheatCodeClass(IGameApiClient _gameApiClient)
        {
            this.gameApiClient = _gameApiClient;
        }

        [CloudCodeFunction("SetCheatCode")]
        public async Task SetCheatCode(IExecutionContext context, ICheatCode cheatCode, string dateTime, bool isActive)
        {
            cheatCode.ActivateCheatCode(context.PlayerId, dateTime, isActive);
       }
    }
}
