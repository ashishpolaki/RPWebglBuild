using System.Collections;
using System.Collections.Generic;
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