using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace
{
    public class FinishLine : MonoBehaviour
    {
        private List<int> horseNumbers = new List<int>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HorseLeg"))
            {
                HorseLeg horseLeg = other.GetComponent<HorseLeg>();
                if (horseLeg != null)
                {
                    int horseNumer = horseLeg.GetHorseNumber();
                    if (!horseNumbers.Contains(horseNumer))
                    {
                        horseNumbers.Add(horseNumer);
                        EventManager.Instance.CrossFinishLine(horseNumer);
                    }
                }
            }
        }
    }
}
