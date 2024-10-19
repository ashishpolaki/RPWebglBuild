using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HorsePositionUI : MonoBehaviour
{
    public TextMeshProUGUI horsePositionsText;

    public void SetUI(string[] horses)
    {
        horsePositionsText.text = string.Empty;
        horsePositionsText.text += "Horse Positions";
        for (int i = 0; i < horses.Length; i++)
        {
            horsePositionsText.text += horses[i];
        }
    }
}
