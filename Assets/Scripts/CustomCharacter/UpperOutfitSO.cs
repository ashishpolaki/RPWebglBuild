using UnityEngine;

[CreateAssetMenu(fileName = "Outfit", menuName = "ScriptableObjects/Character/UpperOutfit")]
public class UpperOutfitSO : ScriptableObject
{
    public int index;

    public int torsoIndex;
    public int upperRightArmIndex;
    public int lowerRightArmIndex;
    public int upperLeftArmIndex;
    public int lowerLeftArmIndex;

    public CharacterPartTextureColors[] torsoColors;
    public CharacterPartTextureColors[] upperArmColors;
}
