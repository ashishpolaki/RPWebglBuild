using System;
using System.Collections.Generic;

namespace HorseRaceCloudCode
{
    public interface IRaceController
    {
        public void AddRaceCheckIn(CurrentRacePlayerCheckIn currentRacePlayerCheckIn, string venueName);
        public void RemoveVenueName(string venueName);
        public int CheckInsCount(string venueName);
    }

    public class RaceController : IRaceController
    {
        private Dictionary<string, List<CurrentRacePlayerCheckIn>> raceCheckIns = new Dictionary<string, List<CurrentRacePlayerCheckIn>>();

        public RaceController()
        {
            raceCheckIns = new Dictionary<string, List<CurrentRacePlayerCheckIn>>();
        }

        public void AddRaceCheckIn(CurrentRacePlayerCheckIn currentRacePlayerCheckIn, string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName) == false)
            {
                raceCheckIns.Add(venueName, new List<CurrentRacePlayerCheckIn>());
            }
            raceCheckIns[venueName].Add(currentRacePlayerCheckIn);
        }

        public void RemoveVenueName(string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName))
            {
                raceCheckIns[venueName].Clear();
                raceCheckIns[venueName] = null;
                raceCheckIns.Remove(venueName);
            }
        }

        public int CheckInsCount(string venueName)
        {
            if (raceCheckIns.ContainsKey(venueName))
            {
                return raceCheckIns[venueName].Count;
            }
            return 0;
        }
    }
}
