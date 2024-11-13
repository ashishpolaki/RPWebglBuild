using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorseRaceCloudCode
{
    public interface IRaceController
    {
        //Race CheckIn
        public void AddRaceCheckIn(CurrentRacePlayerCheckIn currentRacePlayerCheckIn, string venueName);
        public void DeleteRaceCheckIns(string venueName);
        public bool IsPlayerCheckedIn(string playerID, string venueName);
        public List<CurrentRacePlayerCheckIn> GetRaceCheckInPlayers(string venueName);

        //Race Lobby
        public void AddRaceLobbyParticipant(RaceLobbyParticipant raceLobbyParticipant, string venueName);
        public void DeleteRaceLobby(string venueName);
        public RaceLobbyParticipant GetRaceLobbyParticipant(string playerID, string venueName);

        //Race Result
        public void AddRaceResult(PlayerRaceResult playerRaceResult, string venueName);
        public void DeleteRaceResult(string venueName);
        public PlayerRaceResult GetPlayerRaceResult(string playerID, string venueName);
    }

    public class RaceController : IRaceController
    {
        private Dictionary<string, List<CurrentRacePlayerCheckIn>> raceCheckIns = new Dictionary<string, List<CurrentRacePlayerCheckIn>>();
        private Dictionary<string, List<RaceLobbyParticipant>> raceLobbies = new Dictionary<string, List<RaceLobbyParticipant>>();
        private Dictionary<string, List<PlayerRaceResult>> raceResults = new Dictionary<string, List<PlayerRaceResult>>();

        public RaceController()
        {
            raceCheckIns = new Dictionary<string, List<CurrentRacePlayerCheckIn>>();
        }

        #region Race CheckIn
        public void AddRaceCheckIn(CurrentRacePlayerCheckIn currentRacePlayerCheckIn, string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName) == false)
            {
                raceCheckIns.Add(venueName, new List<CurrentRacePlayerCheckIn>());
            }
            if (raceCheckIns[venueName].Contains(currentRacePlayerCheckIn) == false)
            {
                raceCheckIns[venueName].Add(currentRacePlayerCheckIn);
            }
        }
        public void DeleteRaceCheckIns(string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName))
            {
                raceCheckIns[venueName].Clear();
                raceCheckIns[venueName] = null;
                raceCheckIns.Remove(venueName);
            }
        }
        public bool IsPlayerCheckedIn(string playerID, string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName))
            {
                foreach (var currentRacePlayerCheckIn in raceCheckIns[venueName])
                {
                    if (currentRacePlayerCheckIn.PlayerID == playerID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<CurrentRacePlayerCheckIn> GetRaceCheckInPlayers(string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName))
            {
                return raceCheckIns[venueName];
            }
            return new List<CurrentRacePlayerCheckIn>();
        }
        #endregion

        #region Race Lobby
        public void DeleteRaceLobby(string venueName)
        {
            if (raceLobbies.ContainsKey(venueName))
            {
                raceLobbies[venueName].Clear();
                raceLobbies[venueName] = null;
                raceLobbies.Remove(venueName);
            }
        }
        public void AddRaceLobbyParticipant(RaceLobbyParticipant raceLobbyParticipant, string venueName)
        {
            if (raceLobbies.ContainsKey(venueName) == false)
            {
                raceLobbies.Add(venueName, new List<RaceLobbyParticipant>());
            }
            if (raceLobbies[venueName].Contains(raceLobbyParticipant) == false)
            {
                raceLobbies[venueName].Add(raceLobbyParticipant);
            }
        }
        public RaceLobbyParticipant GetRaceLobbyParticipant(string playerID, string venueName)
        {
            if (raceLobbies.ContainsKey(venueName))
            {
                foreach (var raceLobbyParticipant in raceLobbies[venueName])
                {
                    if (raceLobbyParticipant.PlayerID == playerID)
                    {
                        return raceLobbyParticipant;
                    }
                }
            }
            return new RaceLobbyParticipant();
        }
        #endregion

        #region Race Result
        public void AddRaceResult(PlayerRaceResult playerRaceResult, string venueName)
        {
            if (raceResults.ContainsKey(venueName))
            {
                raceResults.Add(venueName, new List<PlayerRaceResult>());
            }
            if (raceResults[venueName].Contains(playerRaceResult) == false)
            {
                raceResults[venueName].Add(playerRaceResult);
            }
        }
        public void DeleteRaceResult(string venueName)
        {
            if (raceResults.ContainsKey(venueName))
            {
                raceResults[venueName].Clear();
                raceResults[venueName] = null;
                raceResults.Remove(venueName);
            }
        }
        public PlayerRaceResult GetPlayerRaceResult(string playerID, string venueName)
        {
            if (raceResults.ContainsKey(venueName))
            {
                foreach (var playerRaceResult in raceResults[venueName])
                {
                    if (playerRaceResult.PlayerID == playerID)
                    {
                        return playerRaceResult;
                    }
                }
            }
            return new PlayerRaceResult();
        }
        #endregion
    }
}
