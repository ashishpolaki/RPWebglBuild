using UnityEngine;

[CreateAssetMenu(fileName = "Outfit", menuName = "ScriptableObjects/Character/LowerOutfit")]
public class LowerOutfitSO : ScriptableObject
{
    public int index;

    public int hipsIndex;
    public int rightLegIndex;
    public int leftLegIndex;
    public int rightFootIndex;
    public int leftFootIndex;

    public CharacterPartTextureColors[] hipsColors;
    public CharacterPartTextureColors[] legColors;
    public CharacterPartTextureColors[] footColors;
}
