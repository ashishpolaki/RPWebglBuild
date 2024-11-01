using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ThemeData", menuName = "ScriptableObjects/Theme/ThemeData")]
public class ThemeDataSO : ScriptableObject
{
    public Sprite backGround;
    public Sprite character;

    public Color bodyBGColor;
    public Color bodyBGOutlineColor;
    public Color bodyColor;
    public Color bodyOutlineColor;

    public Color buttonTextColor;
    public Color buttonTextOutlineColor;

    public Color textColor;
    public Color textOutlineColor;
}
