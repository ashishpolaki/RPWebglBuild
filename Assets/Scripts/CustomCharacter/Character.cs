using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private List<CharacterData> characterDataList = new List<CharacterData>();
    [SerializeField] private CharacterSO characterSO;

    private static readonly int HairColorPropertyID = Shader.PropertyToID("_Color_Hair");
    private static readonly int SkinColorPropertyID = Shader.PropertyToID("_Color_Skin");
    private static readonly int MaskColorPropertyID = Shader.PropertyToID("_Color_Leather_Secondary");

    public void ChangeCharacterPart(CharacterPartType characterPartType, Mesh mesh)
    {
        foreach (CharacterData item in characterDataList)
        {
            if (item.characterPartType == characterPartType)
            {
                item.meshRenderer.sharedMesh = mesh;
            }
        }
    }

    public void SetPartColor(CharacterPartUIType characterPartType, Color color)
    {
        switch (characterPartType)
        {
            case CharacterPartUIType.FACE:
                SetMeshColor(CharacterPartType.Head, SkinColorPropertyID, color);
                break;
            case CharacterPartUIType.HAIR:
                SetMeshColor(CharacterPartType.Hair, HairColorPropertyID, color);
                break;
            case CharacterPartUIType.EYEBROWS:
                SetMeshColor(CharacterPartType.Eyebrows, HairColorPropertyID, color);
                break;
            case CharacterPartUIType.BEARD:
                SetMeshColor(CharacterPartType.FacialHair, HairColorPropertyID, color);
                break;
            case CharacterPartUIType.HAT:
                break;
            case CharacterPartUIType.MASK:
                SetMeshColor(CharacterPartType.Mask, MaskColorPropertyID, color);
                break;
            case CharacterPartUIType.TORSO:
                SetMeshColor(CharacterPartType.Torso, SkinColorPropertyID, color);
                break;
            case CharacterPartUIType.ARMS:
                SetMeshColor(CharacterPartType.LeftLowerArm, SkinColorPropertyID, color);
                SetMeshColor(CharacterPartType.RightLowerArm, SkinColorPropertyID, color);
                SetMeshColor(CharacterPartType.LeftUpperArm, SkinColorPropertyID, color);
                SetMeshColor(CharacterPartType.RightUpperArm, SkinColorPropertyID, color);

                break;
            case CharacterPartUIType.HANDS:
                SetMeshColor(CharacterPartType.LeftHand, SkinColorPropertyID, color);
                SetMeshColor(CharacterPartType.RightHand, SkinColorPropertyID, color);
                break;
            case CharacterPartUIType.LEGS:
                SetMeshColor(CharacterPartType.Hips, SkinColorPropertyID, color);
                SetMeshColor(CharacterPartType.LeftLeg, SkinColorPropertyID, color);
                SetMeshColor(CharacterPartType.RightLeg, SkinColorPropertyID, color);
                break;
            default:
                break;
        }
    }

    private void SetMeshColor(CharacterPartType characterPartType, int nameID, Color color)
    {
        foreach (CharacterData item in characterDataList)
        {
            if (item.characterPartType == characterPartType)
            {
                item.meshRenderer.material.SetColor(nameID, color);
            }
        }
    }

    public void TurnOffCharacterPart(CharacterPartType characterPartType)
    {
        foreach (CharacterData item in characterDataList)
        {
            if (item.characterPartType == characterPartType)
            {
                item.meshRenderer.sharedMesh = null;
            }
        }
    }

}

[System.Serializable]
public struct CharacterData
{
    public CharacterPartType characterPartType;
    public SkinnedMeshRenderer meshRenderer;
}