using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGS
{
    public class RaceLobbyHandler : IDisposable
    {
        #region Private Variables
        private int maxLobbyPlayers = 12;
        private Dictionary<string, (string, int)> qualifiedPlayers;
        private List<string> unQualifiedPlayersList;
        private List<CurrentRacePlayerCheckIn> checkInPlayersList;
        private List<int> horsesInRaceOrderList;
        #endregion

        //Constructor
        public RaceLobbyHandler(List<CurrentRacePlayerCheckIn> _checkInPlayersList)
        {
            //Set Default Values
            qualifiedPlayers = new Dictionary<string, (string, int)>();
            unQualifiedPlayersList = new List<string>();

            checkInPlayersList = _checkInPlayersList;
            horsesInRaceOrderList = new List<int>(GameManager.Instance.LoadHorsesInRaceOrder());
        }

        public void Dispose()
        {
            if (qualifiedPlayers != null)
            {
                qualifiedPlayers.Clear();
                qualifiedPlayers = null;
            }
            if (checkInPlayersList != null)
            {
                checkInPlayersList.Clear();
                checkInPlayersList = null;
            }
            if (unQualifiedPlayersList != null)
            {
                unQualifiedPlayersList.Clear();
                unQualifiedPlayersList = null;
            }
            if (horsesInRaceOrderList != null)
            {
                horsesInRaceOrderList.Clear();
                horsesInRaceOrderList = null;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the Qualified Players in the race.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, (string, int)>> GetQualifiedPlayers()
        {
            await SetRaceWinner();
            await ChooseRemainingLobbyPlayers();

            return qualifiedPlayers;
        }

        /// <summary>
        /// Get the Un Qualified Players in the race.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetUnQualifiedPlayers()
        {
            await Task.Run(() =>
            {
                foreach (var player in checkInPlayersList)
                {
                    unQualifiedPlayersList.Add(player.PlayerID);
                }
            });
            return unQualifiedPlayersList;
        }

        /// <summary>
        /// Set Race Winner with cumulative odd entries.
        /// </summary>
        /// <returns></returns>
        private async Task SetRaceWinner()
        {
            System.Random random = new System.Random();
            double cumulative = 0.0;
            long totalCheckIns = await Task.Run(() => checkInPlayersList.Sum(player => (long)player.CurrentDayCheckIns));

            // Generate a random number between 0 and totalCheckIns
            double randomNumber = random.NextDouble() * totalCheckIns;

            // Select the winner based on the weighted random number
            for (int i = 0; i < checkInPlayersList.Count; i++)
            {
                cumulative += checkInPlayersList[i].CurrentDayCheckIns;
                if (randomNumber < cumulative)
                {
                    AddPlayerToLobby(i);
                }
            }
        }

        /// <summary>
        /// Choose Remaining Players for Host Lobby.
        /// </summary>
        private async Task ChooseRemainingLobbyPlayers()
        {
            //Adjust maxLobbyPlayersCount to match checkInPlayersCount, if maxLobbyPlayersCount is less than checkInPlayersCount.
            int lobbyPlayersCount = maxLobbyPlayers;
            if (checkInPlayersList.Count < lobbyPlayersCount)
            {
                lobbyPlayersCount = checkInPlayersList.Count;
            }

            //Add Remaining Players
            await Task.Run(() =>
            {
                for (int i = 0; i < lobbyPlayersCount; i++)
                {
                    System.Random random = new System.Random();
                    int randomPlayerIndex = random.Next(checkInPlayersList.Count);
                    AddPlayerToLobby(randomPlayerIndex);
                }
            });
        }

        /// <summary>
        /// Add the player to the Host Lobby.
        /// </summary>
        /// <param name="index"></param>
        private void AddPlayerToLobby(int index)
        {
            //After Assign Horse Number to qualified members. Remove the checkinplayers,horseNumbers to avoid conflicts.
            qualifiedPlayers[checkInPlayersList[index].PlayerID] = (checkInPlayersList[index].PlayerName, horsesInRaceOrderList[0]);
            checkInPlayersList.RemoveAt(index);
            horsesInRaceOrderList.RemoveAt(0);
        }
    }
}
