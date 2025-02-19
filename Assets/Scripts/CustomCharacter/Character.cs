using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using CharacterCustomisation;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using UGS;

public class Character : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private List<CharacterData> characterDataList = new List<CharacterData>();
    [SerializeField] private GameObject rootBone;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 faceSwipeColliderCenter;
    [SerializeField] private Vector3 faceSwipeColliderSize;
    [SerializeField] private Vector3 bodySwipeColliderCenter;
    [SerializeField] private Vector3 bodySwipeColliderSize;
    [SerializeField] private CharacterCustomisationEconomy customisationData = new CharacterCustomisationEconomy();
    #endregion

    #region Property
    public int CurrentTorso => customisationData.upperOutfit.torso;
    public int CurrentHips => customisationData.lowerOutfit.hips;
    public CharacterCustomisationEconomy CustomisationData { get { return customisationData; } }
    #endregion

    #region Texture UV's
    private static readonly Vector2Int TorsoUV = new Vector2Int(0, 5);
    private static readonly Vector2Int NoseUV = new Vector2Int(1, 1);
    private static readonly Vector2Int EarLeftUV = new Vector2Int(1, 5);
    private static readonly Vector2Int EarRightUV = new Vector2Int(2, 5);
    private static readonly Vector2Int EyeLidRightUV = new Vector2Int(2, 4);
    private static readonly Vector2Int EyeLidLeftUV = new Vector2Int(1, 4);
    private static readonly Vector2Int EyeInnerLeftUV = new Vector2Int(4, 2);
    private static readonly Vector2Int EyeRightUV = new Vector2Int(5, 5);
    private static readonly Vector2Int EyeLeftUV = new Vector2Int(4, 5);
    private static readonly Vector2Int EyeInnerRightUV = new Vector2Int(5, 2);
    private static readonly Vector2Int EyeBrowRightUV = new Vector2Int(7, 5);
    private static readonly Vector2Int EyeBrowLeftUV = new Vector2Int(6, 5);
    private static readonly Vector2Int HairUV = new Vector2Int(6, 4);
    private static readonly Vector2Int FacialHairUV = new Vector2Int(7, 4);
    #endregion

#if UNITY_EDITOR
    public bool isValidate;
    public SkinnedMeshRenderer changeMeshRenderer;
    public SyntyCharacterPartType syntyCharacterPartType;
    public Color color;
    public int u;
    public int v;
    private void OnValidate()
    {
        if (isValidate)
        {
            // ChangePartColor(color, new Vector2Int(u, v));
            ChangeMesh(syntyCharacterPartType, changeMeshRenderer);
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
    #region Unity methods
    private void Awake()
    {
        CreateNewTexture();
    }
    #endregion

    #region Animation
    public void ChangeAnimator(RuntimeAnimatorController runtimeAnimatorController)
    {
        animator.runtimeAnimatorController = runtimeAnimatorController;
    }
    #endregion

    public void CreateNewTexture()
    {
        texture = Instantiate(texture);
        foreach (var characterPartData in characterDataList)
        {
            if (characterPartData.meshRenderer != null)
            {
                foreach (var item in characterPartData.meshRenderer.materials)
                {
                    item.mainTexture = texture;
                }
            }
        }
    }
    public void SyntyBug()
    {
        ChangePartColor(Color.white, EyeRightUV);
        ChangePartColor(Color.white, EyeLeftUV);
    }
    #region Load/Save
    public void Load(CharacterCustomisationEconomy characterCustomisation)
    {
        customisationData = characterCustomisation;
        //Body
        Color skinToneColor = StringUtils.IsStringEmpty(customisationData.skinToneColor) ? CharacterCustomisationManager.Instance.SkinToneColorPreset.colors[0] : Utils.FromHex(customisationData.skinToneColor);
        ChangeSkinToneColor(skinToneColor);
        BodySizeBlendShape(customisationData.bodyType);
        MusculatureBlendShape(customisationData.bodyMuscleType);
        BodyGenderBlendShape(customisationData.bodyGenderType);

        //Outfits
        ChangeUpperOutfit(customisationData.upperOutfit);
        ChangeLowerOutfit(customisationData.lowerOutfit);

        //Synty Bug Fix
        SyntyBug();

        //Face Customisation
        foreach (var customPart in customisationData.customParts)
        {
            ChangePartInHead((BlendPartType)customPart.type, customPart.styleNumber);
            ChangePartColorInHead(customPart.color, (BlendPartType)customPart.type);
            // UpdatePartsBlendShapeInHead((BlendPartType)customPart.type, customPart.blendShapes);
        }
    }

    public async Task Save()
    {
        Func<Task<List<PlayersInventoryItem>>> method = async () => await UGSManager.Instance.Economy.GetInventoryItem(StringUtils.INVENTORYITEMID_CHARACTER, StringUtils.PLAYERINVENTORYITEMID_CHARACTER);
        List<PlayersInventoryItem> playersInventoryItems = await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        bool isCharacterDataExist = playersInventoryItems.Count > 0;
        if (isCharacterDataExist)
        {
            //Update 
            await UGSManager.Instance.Economy.UpdateInventoryItem(StringUtils.PLAYERINVENTORYITEMID_CHARACTER, customisationData);
        }
        else
        {
            //Add
            await UGSManager.Instance.Economy.AddInventoryItem(StringUtils.INVENTORYITEMID_CHARACTER, customisationData, StringUtils.PLAYERINVENTORYITEMID_CHARACTER);
        }
    }

    #endregion

    #region Color
    public void ChangeSkinToneColor(Color color)
    {
        customisationData.skinToneColor = Utils.ToHex(color);
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
    public void ChangePartsColor(Color color, List<Vector2Int> uvList)
    {
        foreach (var uv in uvList)
        {
            UpdateTexture(texture, color, uv.x, uv.y);
        }
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
    public void ChangeUpperOutfit(UpperOutfitEconomy upperOutfit)
    {
        customisationData.upperOutfit = upperOutfit;

        if (upperOutfit.torso != -1)
        {
            SkinnedMeshRenderer Torso = Resources.Load<GameObject>($"CharacterParts/Torso/Torso_{upperOutfit.torso}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.Torso, Torso);
        }

        if (upperOutfit.leftUpperArm != -1)
        {
            SkinnedMeshRenderer LeftUpperArm = Resources.Load<GameObject>($"CharacterParts/LeftUpperArm/LeftUpperArm_{upperOutfit.leftUpperArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.ArmUpperLeft, LeftUpperArm);
        }

        if (upperOutfit.leftLowerArm != -1)
        {
            SkinnedMeshRenderer LeftLowerArm = Resources.Load<GameObject>($"CharacterParts/LeftLowerArm/LeftLowerArm_{upperOutfit.leftLowerArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.ArmLowerLeft, LeftLowerArm);
        }

        if (upperOutfit.leftHand != -1)
        {
            SkinnedMeshRenderer LeftHand = Resources.Load<GameObject>($"CharacterParts/LeftHand/LeftHand_{upperOutfit.leftHand}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.HandLeft, LeftHand);
        }

        if (upperOutfit.rightUpperArm != -1)
        {
            SkinnedMeshRenderer RightUpperArm = Resources.Load<GameObject>($"CharacterParts/RightUpperArm/RightUpperArm_{upperOutfit.rightUpperArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.ArmUpperRight, RightUpperArm);
        }

        if (upperOutfit.rightLowerArm != -1)
        {
            SkinnedMeshRenderer RightLowerArm = Resources.Load<GameObject>($"CharacterParts/RightLowerArm/RightLowerArm_{upperOutfit.rightLowerArm}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.ArmLowerRight, RightLowerArm);
        }

        if (upperOutfit.rightHand != -1)
        {
            SkinnedMeshRenderer RightHand = Resources.Load<GameObject>($"CharacterParts/RightHand/RightHand_{upperOutfit.rightHand}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.HandRight, RightHand);
        }

        foreach (var item in upperOutfit.torsoColors)
        {
            ChangePartColor(Utils.FromHex(item.color), new Vector2Int(item.u, item.v));
        }

        foreach (var item in upperOutfit.upperArmColors)
        {
            ChangePartColor(Utils.FromHex(item.color), new Vector2Int(item.u, item.v));
        }
    }

    public void ChangeLowerOutfit(LowerOutfitEconomy lowerOutfit)
    {
        customisationData.lowerOutfit = lowerOutfit;
        if (lowerOutfit.hips != -1)
        {
            SkinnedMeshRenderer Hips = Resources.Load<GameObject>($"CharacterParts/Hips/Hips_{lowerOutfit.hips}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.Hips, Hips);
        }
        if (lowerOutfit.rightLeg != -1)
        {
            SkinnedMeshRenderer RightLeg = Resources.Load<GameObject>($"CharacterParts/RightLeg/RightLeg_{lowerOutfit.rightLeg}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.LegRight, RightLeg);
        }
        if (lowerOutfit.rightFoot != -1)
        {
            SkinnedMeshRenderer RightFoot = Resources.Load<GameObject>($"CharacterParts/RightFoot/RightFoot_{lowerOutfit.rightFoot}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.FootRight, RightFoot);
        }
        if (lowerOutfit.leftLeg != -1)
        {
            SkinnedMeshRenderer LeftLeg = Resources.Load<GameObject>($"CharacterParts/LeftLeg/LeftLeg_{lowerOutfit.leftLeg}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.LegLeft, LeftLeg);
        }
        if (lowerOutfit.leftFoot != -1)
        {
            SkinnedMeshRenderer LeftFoot = Resources.Load<GameObject>($"CharacterParts/LeftFoot/LeftFoot_{lowerOutfit.leftFoot}").gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            ChangeMesh(SyntyCharacterPartType.FootLeft, LeftFoot);
        }

        foreach (var item in lowerOutfit.hipsColors)
        {
            ChangePartColor(Utils.FromHex(item.color), new Vector2Int(item.u, item.v));
        }

        foreach (var item in lowerOutfit.legColors)
        {
            ChangePartColor(Utils.FromHex(item.color), new Vector2Int(item.u, item.v));
        }

        foreach (var item in lowerOutfit.footColors)
        {
            ChangePartColor(Utils.FromHex(item.color), new Vector2Int(item.u, item.v));
        }

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
        Hashtable boneNameMap = CharacterUtils.CreateBoneNameMap(rootBone);
        Dictionary<string, float> blendShapeNames = new Dictionary<string, float>();
        for (int i = 0; i < thisSkinMesh.sharedMesh.blendShapeCount; i++)
        {
            blendShapeNames[thisSkinMesh.sharedMesh.GetBlendShapeName(i)] = thisSkinMesh.GetBlendShapeWeight(i);
        }

        Transform[] additionalBones = CharacterUtils.FindAdditionalBones(boneNameMap, new List<SkinnedMeshRenderer>(parts));
        if (additionalBones.Length > 0)
        {
            CharacterUtils.JoinAdditionalBonesToBoneArray(bones, additionalBones, boneNameMap);

            boneNameMap = CharacterUtils.CreateBoneNameMap(rootBone);
        }
        Transform[] oldbones = changeSkinMesh.bones;
        Transform[] newbones = new Transform[changeSkinMesh.bones.Length];

        for (int i = 0; i < oldbones.Length; i++)
        {
            newbones[i] = (Transform)boneNameMap[oldbones[i].name];
        }

        thisSkinMesh.sharedMesh = CharacterUtils.CopyMesh(changeSkinMesh.sharedMesh);
        thisSkinMesh.rootBone = (Transform)boneNameMap[changeSkinMesh.rootBone.name];

        CharacterUtils.MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(
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
            if (index != -1)
                thisSkinMesh.SetBlendShapeWeight(index, blendShapeNames.Values.ElementAt(i));
        }
    }
    #endregion

    #region Enable Parts

    public void EnableLowerBody()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.Hips,
            SyntyCharacterPartType.LegLeft,
            SyntyCharacterPartType.LegRight,
            SyntyCharacterPartType.FootLeft,
            SyntyCharacterPartType.FootRight
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }

    public void EnableUpperBody()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.Torso,
            SyntyCharacterPartType.ArmUpperLeft,
            SyntyCharacterPartType.ArmUpperRight,
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }

    }

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
    public void EnableEyebrows()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.EyebrowLeft,
            SyntyCharacterPartType.EyebrowRight,
            SyntyCharacterPartType.Head
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }
    public void EnableNose()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.Nose,
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }
    public void EnableEars()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.EarLeft,
            SyntyCharacterPartType.EarRight,
            SyntyCharacterPartType.Head,
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }
    public void EnableFacialHair()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.FacialHair,
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }

    public void EnableHair()
    {
        List<SyntyCharacterPartType> dontDisableList = new List<SyntyCharacterPartType>()
        {
            SyntyCharacterPartType.Hair,
            SyntyCharacterPartType.Head
        };

        foreach (var characterPartData in characterDataList)
        {
            if (!dontDisableList.Contains(characterPartData.CharacterPartType))
            {
                characterPartData.meshRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (!characterPartData.meshRenderer.gameObject.activeSelf)
                    characterPartData.meshRenderer.gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region Change Part
    public void ChangePart(SyntyCharacterPartType syntyCharacterPartType, string path)
    {
        var skinnedMeshRenderer = Resources.Load<GameObject>(path)?.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            ChangeMesh(syntyCharacterPartType, skinnedMeshRenderer);
        }
    }
    public void TurnOffPart(SyntyCharacterPartType syntyCharacterPartType)
    {
        characterDataList[(int)syntyCharacterPartType - 1].meshRenderer.enabled = false;
    }
    #endregion

    #region Face Customisation
    //public void UpdatePartsBlendShapeInHead(BlendPartType blendPartType, List<BlendShapeEconomy> blendShapes)
    //{
    //    if (blendShapes.Count == 0) return;

    //    CharacterPartSO characterPartSO = CharacterCustomisationManager.Instance.GetCharacterPartSO(blendPartType);
    //    BlendShapePart blendShapePart = characterPartSO.blendPartData;

    //    foreach (BlendShapeEconomy blendShapeEconomy in blendShapes)
    //    {
    //        foreach (var blendShapePartData in blendShapePart.partData)
    //        {
    //            if (blendShapePartData.name == blendShapeEconomy.name)
    //            {
    //                foreach (var syntyCharacterPartType in blendShapePartData.syntyCharacterPartTypes)
    //                {
    //                    SetBlendShape(syntyCharacterPartType, blendShapeEconomy.value, blendShapeEconomy.name);
    //                }
    //            }
    //        }
    //    }
    //}
    public void ChangePartInHead(BlendPartType blendPartType, int styleNumber)
    {
        CharacterPartSO characterPartSO = CharacterCustomisationManager.Instance.GetCharacterPartSO(blendPartType);
        if (characterPartSO.parts.Length == 0) return;

        //Save Customisation Data
        bool isFound = false;
        foreach (var customPartData in customisationData.customParts)
        {
            if (customPartData.type == (int)blendPartType)
            {
                isFound = true;
                customPartData.styleNumber = styleNumber;
                break;
            }
        }
        //If not found
        if (isFound == false)
        {
            customisationData.customParts.Add(new CustomPartEconomy()
            {
                type = (int)blendPartType,
                styleNumber = styleNumber,
                color = "",
            });
        }

        List<SyntyCharacterPartType> characterPartList = CharacterCustomisationManager.Instance.GetCharacterPartType(blendPartType);
        foreach (SyntyCharacterPartType characterPartType in characterPartList)
        {
            if (styleNumber == -1)
            {
                if (characterPartType == SyntyCharacterPartType.FacialHair)
                {
                    TurnOffPart(characterPartType);
                }
            }
            else
            {
                string path = CharacterCustomisationManager.Instance.GetMeshPath(characterPartType, styleNumber);
                ChangePart(characterPartType, path);
            }
        }
    }
    public void ChangePartColorInHead(string _color, BlendPartType blendPartType)
    {
        if (StringUtils.IsStringEmpty(_color)) return;

        //Save Customisation Data
        bool isFound = false;
        foreach (var customPartData in customisationData.customParts)
        {
            if (customPartData.type == (int)blendPartType)
            {
                isFound = true;
                customPartData.color = _color;
                break;
            }
        }
        //If not found
        if (isFound == false)
        {
            customisationData.customParts.Add(new CustomPartEconomy()
            {
                type = (int)blendPartType,
                color = _color,
            });
        }

        CharacterPartSO characterPartSO = CharacterCustomisationManager.Instance.GetCharacterPartSO(blendPartType);
        Color color = Utils.FromHex(_color);
        List<Vector2Int> textureUVList = new List<Vector2Int>();

        switch (characterPartSO.partType)
        {
            case BlendPartType.Hair:
                textureUVList.Add(HairUV);
                break;
            case BlendPartType.FacialHair:
                textureUVList.Add(FacialHairUV);
                break;
            case BlendPartType.Eyes:
                textureUVList.Add(EyeInnerLeftUV);
                textureUVList.Add(EyeInnerRightUV);
                break;
            case BlendPartType.Eyebrows:
                textureUVList.Add(EyeBrowLeftUV);
                textureUVList.Add(EyeBrowRightUV);
                break;
        }

        ChangePartsColor(color, textureUVList);
    }
    #endregion

    #region Body Customisation
    public void BodySizeBlendShape(float value)
    {
        float heavyBlendValue = value > 0 ? value : 0;
        float skinnyBlendValue = value < 0 ? Math.Abs(value) : 0;

        SetBlendShape(heavyBlendValue, StringUtils.BLEND_SHAPE_HEAVY);
        SetBlendShape(skinnyBlendValue, StringUtils.BLEND_SHAPE_SKINNY);
        customisationData.bodyType = value;
    }
    public void MusculatureBlendShape(float value)
    {
        SetBlendShape(value, StringUtils.BLEND_MUSCLE);
        customisationData.bodyMuscleType = value;
    }
    public void BodyGenderBlendShape(float value)
    {
        SetBlendShape(value, StringUtils.BLEND_GENDER);
        customisationData.bodyGenderType = value;
    }
    #endregion
}
