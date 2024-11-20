using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Synty.SidekickCharacters.SkinnedMesh;
using Synty.SidekickCharacters.Utils;
using System.Linq;

public class Character : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private List<CharacterData> characterDataList = new List<CharacterData>();
    [SerializeField] private GameObject rootBone;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Collider collider;
    #endregion

    #region Texture UV's
    private static readonly Vector2Int TorsoUV = new Vector2Int(0, 5);
    private static readonly Vector2Int NoseUV = new Vector2Int(1, 1);
    private static readonly Vector2Int EarLeftUV = new Vector2Int(1, 5);
    private static readonly Vector2Int EarRightUV = new Vector2Int(2, 5);
    private static readonly Vector2Int EyeLidRightUV = new Vector2Int(2, 4);
    private static readonly Vector2Int EyeLidLeftUV = new Vector2Int(1, 4);
    private static readonly Vector2Int EyeRightUV = new Vector2Int(5, 5);
    private static readonly Vector2Int EyeLeftUV = new Vector2Int(4, 5);
    #endregion

#if !UNITY_EDITOR
    public bool isValidate;
    public SkinnedMeshRenderer meshRenderer;
    public SkinnedMeshRenderer changeMeshRenderer;
    public Color color;
    public int u;
    public int v;
    private void OnValidate()
    {
        if (isValidate)
        {
            UpdateTexture(texture, color, u, v);
        }
    }
    private void InitializeSyntyCharacterEnum()
    {
        if (isValidate && characterDataList.Count == 0)
        {
            foreach (var item in Enum.GetValues(typeof(SyntyCharacterPartType)))
            {
                characterDataList.Add(new CharacterData
                {
                    CharacterPartType = (SyntyCharacterPartType)item,
                    meshRenderer = null,
                });
            }
        }
    }

#endif
    #region Color
    public void ChangeSkinToneColor(Color color)
    {
        UpdateTexture(texture, color, TorsoUV.x, TorsoUV.y);
        UpdateTexture(texture, color, NoseUV.x, NoseUV.y);
        UpdateTexture(texture, color, EarLeftUV.x, EarLeftUV.y);
        UpdateTexture(texture, color, EarRightUV.x, EarRightUV.y);
        UpdateTexture(texture, color, EyeLidRightUV.x, EyeLidRightUV.y);
        UpdateTexture(texture, color, EyeLidLeftUV.x, EyeLidLeftUV.y);
    }

    private void UpdateTexture(Texture2D texture, Color newColor, int u, int v)
    {
        int scaledU = u * 2;
        int scaledV = v * 2;
        texture.SetPixel(scaledU, scaledV, newColor);
        texture.SetPixel(scaledU + 1, scaledV, newColor);
        texture.SetPixel(scaledU, scaledV + 1, newColor);
        texture.SetPixel(scaledU + 1, scaledV + 1, newColor);
        texture.Apply();
    }

    public void ChangePartColor(Color color, Vector2Int uv)
    {
        UpdateTexture(texture, color, uv.x, uv.y);
    }
    #endregion

    #region Blend Shape
    public void SetBlendShape(float value, string blendShapeName)
    {
        foreach (var characterData in characterDataList)
        {
            for (int i = 0; i < characterData.meshRenderer.sharedMesh.blendShapeCount; i++)
            {
                if (characterData.meshRenderer.sharedMesh.GetBlendShapeName(i).Contains(blendShapeName))
                {
                    characterData.meshRenderer.SetBlendShapeWeight(i, value);
                }
            }
        }
    }

    public void SetBlendShape(SyntyCharacterPartType syntyCharacterPartType, float value, string blendShapeName)
    {
        int count = characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.sharedMesh.blendShapeCount;

        for (int i = 0; i < count; i++)
        {
            if (characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.sharedMesh.GetBlendShapeName(i).Contains(blendShapeName))
            {
                characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.SetBlendShapeWeight(i, value);
            }
        }
    }
    #endregion

    #region Outfit Customisation
    public void ChangeUpperPart(UpperOutfitEconomy upperOutfit)
    {
        SkinnedMeshRenderer Torso = Resources.Load<GameObject>($"CharacterParts/Torso/Torso_{upperOutfit.torso}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer LeftUpperArm = Resources.Load<GameObject>($"CharacterParts/LeftUpperArm/LeftUpperArm_{upperOutfit.leftUpperArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer LeftLowerArm = Resources.Load<GameObject>($"CharacterParts/LeftLowerArm/LeftLowerArm_{upperOutfit.leftLowerArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer LeftHand = Resources.Load<GameObject>($"CharacterParts/LeftHand/LeftHand_{upperOutfit.leftHand}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer RightUpperArm = Resources.Load<GameObject>($"CharacterParts/RightUpperArm/RightUpperArm_{upperOutfit.rightUpperArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer RightLowerArm = Resources.Load<GameObject>($"CharacterParts/RightLowerArm/RightLowerArm_{upperOutfit.rightLowerArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer RightHand = Resources.Load<GameObject>($"CharacterParts/RightHand/RightHand_{upperOutfit.rightHand}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        ChangeMesh(SyntyCharacterPartType.Torso, Torso);
        ChangeMesh(SyntyCharacterPartType.ArmUpperLeft, LeftUpperArm);
        ChangeMesh(SyntyCharacterPartType.ArmLowerLeft, LeftLowerArm);
        ChangeMesh(SyntyCharacterPartType.HandLeft, LeftHand);
        ChangeMesh(SyntyCharacterPartType.ArmUpperRight, RightUpperArm);
        ChangeMesh(SyntyCharacterPartType.ArmLowerRight, RightLowerArm);
        ChangeMesh(SyntyCharacterPartType.HandRight, RightHand);
    }

    public void ChangeLowerPart(LowerOutfitEconomy lowerOutfit)
    {
        SkinnedMeshRenderer Hips = Resources.Load<GameObject>($"CharacterParts/Hips/Hips_{lowerOutfit.hips}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer RightLeg = Resources.Load<GameObject>($"CharacterParts/RightLeg/RightLeg_{lowerOutfit.rightLeg}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer RightFoot = Resources.Load<GameObject>($"CharacterParts/RightFoot/RightFoot_{lowerOutfit.rightFoot}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer LeftLeg = Resources.Load<GameObject>($"CharacterParts/LeftLeg/LeftLeg_{lowerOutfit.leftLeg}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer LeftFoot = Resources.Load<GameObject>($"CharacterParts/LeftFoot/LeftFoot_{lowerOutfit.leftFoot}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        ChangeMesh(SyntyCharacterPartType.Hips, Hips);
        ChangeMesh(SyntyCharacterPartType.LegRight, RightLeg);
        ChangeMesh(SyntyCharacterPartType.FootRight, RightFoot);
        ChangeMesh(SyntyCharacterPartType.LegLeft, LeftLeg);
        ChangeMesh(SyntyCharacterPartType.FootLeft, LeftFoot);

        SetBlendShape(SyntyCharacterPartType.Hips, 100, "Legs");
        SetBlendShape(SyntyCharacterPartType.Hips, 100, "Feet");
    }
    #endregion

    #region Change Mesh
    private void ChangeMesh(SyntyCharacterPartType syntyCharacterPartType, SkinnedMeshRenderer changeSkinMesh)
    {
        if (characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.enabled == false)
        {
            characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.enabled = true;
        }
        ChangeMesh(characterDataList[(int)syntyCharacterPartType - 1].meshRenderer, changeSkinMesh);
    }

    private void ChangeMesh(SkinnedMeshRenderer thisSkinMesh, SkinnedMeshRenderer changeSkinMesh)
    {
        Transform[] bones = Array.Empty<Transform>();
        List<SkinnedMeshRenderer> parts = new List<SkinnedMeshRenderer>() { changeSkinMesh };
        Hashtable boneNameMap = Combiner.CreateBoneNameMap(rootBone);
        Dictionary<string, float> blendShapeNames = new Dictionary<string, float>();
        for (int i = 0; i < thisSkinMesh.sharedMesh.blendShapeCount; i++)
        {
            blendShapeNames[thisSkinMesh.sharedMesh.GetBlendShapeName(i)] = thisSkinMesh.GetBlendShapeWeight(i);
        }

        Transform[] additionalBones = Combiner.FindAdditionalBones(boneNameMap, new List<SkinnedMeshRenderer>(parts));
        if (additionalBones.Length > 0)
        {
            Combiner.JoinAdditionalBonesToBoneArray(bones, additionalBones, boneNameMap);

            boneNameMap = Combiner.CreateBoneNameMap(rootBone);
        }
        Transform[] oldbones = changeSkinMesh.bones;
        Transform[] newbones = new Transform[changeSkinMesh.bones.Length];

        for (int i = 0; i < oldbones.Length; i++)
        {
            newbones[i] = (Transform)boneNameMap[oldbones[i].name];
        }

        thisSkinMesh.sharedMesh = MeshUtils.CopyMesh(changeSkinMesh.sharedMesh);
        thisSkinMesh.rootBone = (Transform)boneNameMap[changeSkinMesh.rootBone.name];

        Combiner.MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(
            new SkinnedMeshRenderer[]
            {
                 changeSkinMesh,
            },
             thisSkinMesh.sharedMesh,
            thisSkinMesh
            );

        thisSkinMesh.bones = newbones;
        for (int i = 0; i < blendShapeNames.Count; i++)
        {
            int index = thisSkinMesh.sharedMesh.GetBlendShapeIndex(blendShapeNames.Keys.ElementAt(i));
            if(index != -1)
            thisSkinMesh.SetBlendShapeWeight(index, blendShapeNames.Values.ElementAt(i));
        }
    }
    #endregion

    #region Change Part
    public void ChangePart(SyntyCharacterPartType syntyCharacterPartType, string path)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = Resources.Load<GameObject>(path).GetComponentInChildren<SkinnedMeshRenderer>();
        ChangeMesh(syntyCharacterPartType, skinnedMeshRenderer);
    }
    public void TurnOffPart(SyntyCharacterPartType syntyCharacterPartType)
    {
        characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.enabled = false;
    }
    #endregion

    #region Turn/Off Character Parts
    public void EnableFace()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.Head,
            SyntyCharacterPartType.Hair,
            SyntyCharacterPartType.EyebrowLeft,
            SyntyCharacterPartType.EyebrowRight,
            SyntyCharacterPartType.EyeLeft,
            SyntyCharacterPartType.EyeRight,
            SyntyCharacterPartType.EarLeft,
            SyntyCharacterPartType.EarRight,
            SyntyCharacterPartType.Nose,
            SyntyCharacterPartType.FacialHair
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
        }
    }
    public void EnableFullBody()
    {
        foreach (var characterPartData in characterDataList)
        {
            if (!characterPartData.meshRenderer.gameObject.activeSelf)
            {
                characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }
    #endregion
}
