using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.UI
{
    public class RacePositionsUIBoard : MonoBehaviour
    {
        [SerializeField] private RacePositionUI[] racePositionsUI;

        public void ShowRacePositions(Dictionary<int, int> _racePositions)
        {
            //Activate Gameobjects
            for (int i = 1; i <= _racePositions.Count; i++)
            {
                if (!racePositionsUI[i - 1].gameObject.activeSelf)
                {
                    racePositionsUI[i - 1].gameObject.SetActive(true);
                }
                racePositionsUI[i - 1].SetUI(_racePositions[i], i);
            }
        }

        public void DisableRacePositions()
        {
            for (int i = 0; i < racePositionsUI.Length; i++)
            {
                racePositionsUI[i].gameObject.SetActive(false);
            }
        }
    }
}
