using TMPro;
using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;

    public void SetData(string playerName)
    {
        playerNameTxt.text = playerName;
    }
}
