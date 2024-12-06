using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "ScriptableObjects/Theme/ThemeData")]
public class ThemeDataSO : ScriptableObject
{
    public Sprite backGround;
    public Sprite character;

    public Color backGroundTintColor;

    public Color bodyBGColor;
    public Color bodyBGOutlineColor;
    public Color bodyColor;
    public Color bodyOutlineColor;

    public Color buttonTextColor;
    public Color buttonTextOutlineColor;

    public Color textColor;
    public Color textOutlineColor;

    public Color cloudColor;
    public Color cloudOutlineColor;

    public Color toggleColor;
    public Color toggleOutlineColor;

    public Color inputFieldBGColor;
    public Color inputFieldBGOutlineColor;
    public Color inputFieldColor;

    public Color inputFieldTextColor;
    public Color errorTextColor;

}
