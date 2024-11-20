using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPart",menuName = "ScriptableObjects/Character/CharacterPart")]
public class CharacterPartSO : ScriptableObject
{
    public BlendPartType partType;
    public string partName;
    public ColorPresetSO colorPreset;
    public int[] parts;
    public BlendShapePart blendPartData;
}
