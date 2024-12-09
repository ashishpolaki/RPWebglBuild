using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGS
{
    public class RaceLobbyHandler : IDisposable
    {
        #region Private Variables
        private const int maxLobbyPlayers = 12;
        private List<RaceLobbyParticipant> qualifiedPlayers;
        private List<CurrentRacePlayerCheckIn> unQualifiedPlayersList;
        private List<CurrentRacePlayerCheckIn> checkInPlayersList;
        private List<int> horsesInRaceOrderList;
        #endregion

        //Constructor
        public RaceLobbyHandler(List<CurrentRacePlayerCheckIn> _checkInPlayersList)
        {
            //Set Default Values
            qualifiedPlayers = new List<RaceLobbyParticipant>();
            unQualifiedPlayersList = new List<CurrentRacePlayerCheckIn>();
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
        public async Task<List<RaceLobbyParticipant>> GetQualifiedPlayers()
        {
            await SetRaceWinner();
            await ChooseRemainingLobbyPlayers();

            return qualifiedPlayers;
        }

        /// <summary>
        /// Get the Un Qualified Players in the race.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CurrentRacePlayerCheckIn>> GetUnQualifiedPlayers()
        {
            await Task.Run(() =>
            {
                foreach (var player in checkInPlayersList)
                {
                    unQualifiedPlayersList.Add(player);
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
                    int index = i;
                    AddPlayerToLobby(checkInPlayersList[index], index);
                    break;
                }
            }
        }

        /// <summary>
        /// Choose Remaining Players for Host Lobby.
        /// </summary>
        private async Task ChooseRemainingLobbyPlayers()
        {
            //Adjust maxLobbyPlayersCount to match checkInPlayersCount, if maxLobbyPlayersCount is less than checkInPlayersCount.
            int lobbyPlayersCount = maxLobbyPlayers - 1;
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
                    AddPlayerToLobby(checkInPlayersList[randomPlayerIndex], randomPlayerIndex);
                }
            });
        }

        /// <summary>
        /// Add the player to the Host Lobby.
        /// </summary>
        /// <param name="index"></param>
        private void AddPlayerToLobby(CurrentRacePlayerCheckIn currentRacePlayerCheckIn, int index)
        {
            RaceLobbyParticipant raceLobbyParticipant = new RaceLobbyParticipant();
            raceLobbyParticipant.PlayerID = currentRacePlayerCheckIn.PlayerID;
            raceLobbyParticipant.PlayerName = currentRacePlayerCheckIn.PlayerName;
            raceLobbyParticipant.HorseNumber = horsesInRaceOrderList[0];
            //After Assign Horse Number to qualified members. Remove the checkinplayers,horseNumbers to avoid conflicts.
            qualifiedPlayers.Add(raceLobbyParticipant);
            checkInPlayersList.RemoveAt(index);
            horsesInRaceOrderList.RemoveAt(0);
        }
    }
}
