using TMPro;
using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private TextMeshProUGUI horseNumberTxt;

    public void SetData(string horseNumber,string playerName)
    {
        horseNumberTxt.text = horseNumber;
        playerNameTxt.text = playerName;
    }
}
