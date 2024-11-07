using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPart",menuName = "ScriptableObjects/Character/CharacterPart")]
public class CharacterPartSO : ScriptableObject
{
    public CharacterGenderType characterGender;
    public CharacterPartType partType;
    public Mesh mesh;
}
