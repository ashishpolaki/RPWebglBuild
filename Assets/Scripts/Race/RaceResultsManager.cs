using UnityEngine;
namespace HorseRace
{
    public class RaceResultsManager : MonoBehaviour
    {
        public string raceNumber;
        void Start()
        {
           RaceAnalyticsProgram.Main(raceNumber); 
        }
    }
}