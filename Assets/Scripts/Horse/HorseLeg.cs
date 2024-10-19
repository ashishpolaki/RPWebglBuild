using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseLeg : MonoBehaviour
{
    private int horseNumber;
    public void SetHorseNumber(int _horseNumber)
    {
        horseNumber = _horseNumber;
    }
    public int GetHorseNumber()
    {
        return horseNumber;
    }
}
