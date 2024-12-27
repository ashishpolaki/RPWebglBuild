using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public List<RaceLobbyParticipant> GetQualifiedPlayers()
        {
            // Use UnityEngine.Random instead of System.Random
            double cumulative = 0.0;
            long totalCheckIns = checkInPlayersList.Sum(player => (long)player.CurrentDayCheckIns);

            // Generate a random number between 0 and totalCheckIns
            double randomNumber = Utils.GenerateRandomNumber(0f, (float)totalCheckIns);

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

            //Choose Remaining Players
            // Adjust maxLobbyPlayersCount to match checkInPlayersCount, if maxLobbyPlayersCount is less than checkInPlayersCount.
            int lobbyPlayersCount = maxLobbyPlayers - 1;
            if (checkInPlayersList.Count < lobbyPlayersCount)
            {
                lobbyPlayersCount = checkInPlayersList.Count;
            }

            // Add Remaining Players
            for (int i = 0; i < lobbyPlayersCount; i++)
            {
                int randomPlayerIndex = UnityEngine.Random.Range(0, checkInPlayersList.Count);
                AddPlayerToLobby(checkInPlayersList[randomPlayerIndex], randomPlayerIndex);
            }

            return qualifiedPlayers;
        }

        /// <summary>
        /// Get the Un Qualified Players in the race.
        /// </summary>
        /// <returns></returns>
        public List<CurrentRacePlayerCheckIn> GetUnQualifiedPlayers()
        {
            foreach (var player in checkInPlayersList)
            {
                unQualifiedPlayersList.Add(player);
            }
            return unQualifiedPlayersList;
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
