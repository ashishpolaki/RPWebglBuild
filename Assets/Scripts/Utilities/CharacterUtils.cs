using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;
using UGS;


public class CharacterUtils
{
    public static Hashtable CreateBoneNameMap(GameObject currentBone)
    {
        Hashtable boneNameMap = new Hashtable();
        boneNameMap.Add(currentBone.name, currentBone.transform);
        for (int i = 0; i < currentBone.transform.childCount; i++)
        {
            Hashtable childBoneMap = CreateBoneNameMap(currentBone.transform.GetChild(i).gameObject);
            foreach (DictionaryEntry entry in childBoneMap)
            {
                if (!boneNameMap.ContainsKey(entry.Key))
                {
                    boneNameMap.Add(entry.Key, (Transform)entry.Value);
                }
            }
        }
        return boneNameMap;
    }
    public static Transform[] FindAdditionalBones(Hashtable boneMap, List<SkinnedMeshRenderer> meshes)
    {
        List<Transform> newBones = new List<Transform>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            foreach (Transform bone in mesh.bones)
            {
                if (!boneMap.ContainsKey(bone.name))
                {
                    newBones.Add(bone);
                }
            }
        }
        return newBones.ToArray();
    }
    public static Transform[] JoinAdditionalBonesToBoneArray(Transform[] bones, Transform[] additionBones, Hashtable boneMap)
    {
        List<Transform> fullBones = new List<Transform>();
        fullBones.AddRange(bones);
        foreach (Transform bone in additionBones)
        {
            Transform newParent = (Transform)boneMap[bone.parent.name];

            if (newParent != null && !newParent.Find(bone.name))
            {
                GameObject newBone = GameObject.Instantiate(bone.gameObject, newParent);
                newBone.name = newBone.name.Replace("(Clone)", "");
                fullBones.Add(newBone.transform);
                if (!boneMap.ContainsKey(bone.name))
                {
                    boneMap.Add(bone.name, newBone.transform);
                }
            }
        }
        return fullBones.ToArray();
    }
    public static Mesh CopyMesh(Mesh mesh)
    {
        Mesh newMesh = new Mesh
        {
            name = mesh.name,
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            uv = mesh.uv,
            normals = mesh.normals,
            colors = mesh.colors,
            tangents = mesh.tangents,
            boneWeights = mesh.boneWeights,
            bindposes = mesh.bindposes
        };
        return newMesh;
    }
    public class BlendShapeData
    {
        public string blendShapeFrameName = "";
        public int blendShapeFrameIndex = -1;
        public float blendShapeCurrentValue = 0.0f;
        public List<Vector3> startDeltaVertices = new List<Vector3>();
        public List<Vector3> startDeltaNormals = new List<Vector3>();
        public List<Vector3> startDeltaTangents = new List<Vector3>();
        public List<Vector3> finalDeltaVertices = new List<Vector3>();
        public List<Vector3> finalDeltaNormals = new List<Vector3>();
        public List<Vector3> finalDeltaTangents = new List<Vector3>();
        public string blendShapeNameOnCombinedMesh = "";
    }

    public static void MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(
           SkinnedMeshRenderer[] skinnedMeshesToMerge,
           Mesh finalMesh,
           SkinnedMeshRenderer finalSkinnedMeshRenderer
       )
    {
        List<BlendShapeData> allBlendShapeData = new List<BlendShapeData>();

        //Verify each skinned mesh renderer and get info about all blendshapes of all meshes
        int totalVerticesVerifiedAtHereForBlendShapes = 0;

        foreach (SkinnedMeshRenderer combine in skinnedMeshesToMerge)
        {
            // Skip any parts that have not been assigned
            if (combine == null)
            {
                continue;
            }

            allBlendShapeData.AddRange(
                GetBlendShapeData(
                    finalMesh,
                    combine,
                    Array.Empty<string>(),
                    totalVerticesVerifiedAtHereForBlendShapes
                )
            );

            //Set vertices verified at here, after processing all blendshapes for this mesh
            totalVerticesVerifiedAtHereForBlendShapes += combine.sharedMesh.vertexCount;
        }
        RestoreBlendShapeData(allBlendShapeData, finalMesh, finalSkinnedMeshRenderer);
    }
    public static void RestoreBlendShapeData(List<BlendShapeData> blendData, Mesh meshToRestoreTo, SkinnedMeshRenderer meshRenderer)
    {
        Dictionary<string, int> alreadyAddedBlendShapesNames = new Dictionary<string, int>();

        foreach (BlendShapeData blendShape in blendData)
        {
            string blendShapeName = blendShape.blendShapeFrameName;
            if (alreadyAddedBlendShapesNames.TryGetValue(blendShape.blendShapeFrameName, out int name))
            {
                blendShapeName += " (" + name + ")";
            }

            meshToRestoreTo.AddBlendShapeFrame(
                blendShapeName,
                0.0f,
                blendShape.startDeltaVertices.ToArray(),
                blendShape.startDeltaNormals.ToArray(),
                blendShape.startDeltaTangents.ToArray()
            );
            meshToRestoreTo.AddBlendShapeFrame(
                blendShapeName,
                100.0f,
                blendShape.finalDeltaVertices.ToArray(),
                blendShape.finalDeltaNormals.ToArray(),
                blendShape.finalDeltaTangents.ToArray()
            );

            blendShape.blendShapeNameOnCombinedMesh = blendShapeName;

            if (alreadyAddedBlendShapesNames.ContainsKey(blendShape.blendShapeFrameName))
            {
                alreadyAddedBlendShapesNames[blendShape.blendShapeFrameName] += 1;
            }
            else
            {
                alreadyAddedBlendShapesNames.Add(blendShape.blendShapeFrameName, 0);
            }

        }

        foreach (BlendShapeData blendShape in blendData)
        {
            meshRenderer.SetBlendShapeWeight(
                meshToRestoreTo.GetBlendShapeIndex(blendShape.blendShapeFrameName),
                blendShape.blendShapeCurrentValue
            );
        }
    }

    public static List<BlendShapeData> GetBlendShapeData(
           Mesh mesh,
           SkinnedMeshRenderer skinnedMesh,
           string[] excludedBlendNames,
           int verticesCountStartIndex
       )
    {
        List<BlendShapeData> allBlendShapeData = new List<BlendShapeData>();

        int totalVerticesVerifiedAtHereForBlendShapes = verticesCountStartIndex;

        string[] blendShapes = new string[skinnedMesh.sharedMesh.blendShapeCount];
        for (int i = 0; i < skinnedMesh.sharedMesh.blendShapeCount; i++)
        {
            string blendShapeName = skinnedMesh.sharedMesh.GetBlendShapeName(i);
            if (excludedBlendNames.All(ebn => !blendShapeName.Contains(ebn)))
            {
                blendShapes[i] = blendShapeName;
            }
        }

        for (int i = 0; i < blendShapes.Length; i++)
        {
            if (blendShapes[i] == null)
            {
                continue;
            }

            BlendShapeData blendShapeData = new BlendShapeData
            {
                blendShapeFrameName = blendShapes[i],
                blendShapeFrameIndex = skinnedMesh.sharedMesh.GetBlendShapeIndex(blendShapes[i])
            };
            blendShapeData.blendShapeCurrentValue = skinnedMesh.GetBlendShapeWeight(blendShapeData.blendShapeFrameIndex);

            Mesh sharedMesh = skinnedMesh.sharedMesh;
            int framesCount = sharedMesh.GetBlendShapeFrameCount(blendShapeData.blendShapeFrameIndex);

            Vector3[] originalDeltaVertices = new Vector3[sharedMesh.vertexCount];
            Vector3[] originalDeltaNormals = new Vector3[sharedMesh.vertexCount];
            Vector3[] originalDeltaTangents = new Vector3[sharedMesh.vertexCount];

            Vector3[] finalDeltaVertices = new Vector3[mesh.vertexCount];
            Vector3[] finalDeltaNormals = new Vector3[mesh.vertexCount];
            Vector3[] finalDeltaTangents = new Vector3[mesh.vertexCount];

            blendShapeData.startDeltaVertices.AddRange(finalDeltaVertices);
            blendShapeData.startDeltaNormals.AddRange(finalDeltaNormals);
            blendShapeData.startDeltaTangents.AddRange(finalDeltaTangents);

            blendShapeData.finalDeltaVertices.AddRange(finalDeltaVertices);
            blendShapeData.finalDeltaNormals.AddRange(finalDeltaNormals);
            blendShapeData.finalDeltaTangents.AddRange(finalDeltaTangents);

            if (skinnedMesh.sharedMesh.GetBlendShapeIndex(blendShapes[i]) != -1)
            {
                skinnedMesh.sharedMesh.GetBlendShapeFrameVertices(
                    blendShapeData.blendShapeFrameIndex,
                    framesCount - 1,
                    originalDeltaVertices,
                    originalDeltaNormals,
                    originalDeltaTangents
                );
            }

            for (int x = 0; x < originalDeltaVertices.Length; x++)
            {
                blendShapeData.finalDeltaVertices[x + totalVerticesVerifiedAtHereForBlendShapes] = originalDeltaVertices[x];
            }

            for (int x = 0; x < originalDeltaNormals.Length; x++)
            {
                blendShapeData.finalDeltaNormals[x + totalVerticesVerifiedAtHereForBlendShapes] = originalDeltaNormals[x];
            }

            for (int x = 0; x < originalDeltaTangents.Length; x++)
            {
                blendShapeData.finalDeltaTangents[x + totalVerticesVerifiedAtHereForBlendShapes] = originalDeltaTangents[x];
            }

            allBlendShapeData.Add(blendShapeData);
        }

        return allBlendShapeData;
    }

    #region Character Combine Child Meshes

    //private static void PopulateUVDictionary(List<SkinnedMeshRenderer> usedParts)
    //{
    //    _currentUVList = new List<Vector2>();
    //    _currentUVDictionary = new Dictionary<ColorPartType, List<Vector2>>();

    //    foreach (ColorPartType type in Enum.GetValues(typeof(ColorPartType)))
    //    {
    //        _currentUVDictionary.Add(type, new List<Vector2>());
    //    }
    //    int i = 0;
    //    foreach (SkinnedMeshRenderer skinnedMesh in usedParts)
    //    {
    //        ColorPartType type = Enum.Parse<ColorPartType>(/*ExtractPartType(skinnedMesh.name)*/ ((CharacterPartType)i).ToString());
    //        i++;
    //        List<Vector2> partUVs = _currentUVDictionary[type];
    //        foreach (Vector2 uv in skinnedMesh.sharedMesh.uv)
    //        {
    //            int scaledU = (int)Math.Floor(uv.x * 16);
    //            int scaledV = (int)Math.Floor(uv.y * 16);

    //            if (scaledU == 16)
    //            {
    //                scaledU = 15;
    //            }

    //            if (scaledV == 16)
    //            {
    //                scaledV = 15;
    //            }

    //            Vector2 scaledUV = new Vector2(scaledU, scaledV);
    //            // For the global UV list, we don't want any duplicates on a global level
    //            if (!_currentUVList.Contains(scaledUV))
    //            {
    //                _currentUVList.Add(scaledUV);
    //            }

    //            // For the part specific UV list we may have UVs that are in the global list already, we don't want to exclude these, so check
    //            // them separately to the global list
    //            if (!partUVs.Contains(scaledUV))
    //            {
    //                partUVs.Add(scaledUV);
    //            }
    //        }

    //        _currentUVDictionary[type] = partUVs;
    //    }
    //}
    //private static Dictionary<ColorPartType, List<Vector2>> _currentUVDictionary = new Dictionary<ColorPartType, List<Vector2>>();
    // private static CharacterPartType ExtractPartType(string partName)
    // {
    //     string partIndexString = partName.Split('_').Reverse().ElementAt(1).Substring(0, 2);
    //     bool valueParsed = int.TryParse(partIndexString, out int index);
    //     return valueParsed ? (CharacterPartType)index : 0;
    // }
    // private static GameObject CreateCombinedSkinnedMesh(
    //       List<SkinnedMeshRenderer> skinnedMeshesToCombine,
    //       GameObject baseModel, GameObject parent,
    //       Material baseMaterial
    //   )
    // {
    //     // Create the new base GameObject. This will store all the combined meshes.
    //     GameObject combinedModel = new GameObject("Combined Character");
    //     GameObject combinedSkinnedMesh = new GameObject("Mesh");
    //     combinedSkinnedMesh.transform.parent = combinedModel.transform;

    //     Transform modelRootBone = baseModel.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;

    //     // Initialise bone data stores.
    //     Transform[] bones = Array.Empty<Transform>();
    //     int boneCount = 0;

    //     skinnedMeshesToCombine.Sort((a, b) => string.Compare(a.name, b.name));
    //     Material material = null;
    //     Mesh mesh = new Mesh();
    //     int boneOffset = 0;
    //     GameObject rootBone = GameObject.Instantiate(modelRootBone.gameObject, combinedModel.transform, true);
    //     rootBone.name = modelRootBone.name;
    //     Hashtable boneNameMap = CreateBoneNameMap(rootBone);
    //     Transform[] additionalBones = FindAdditionalBones(boneNameMap, new List<SkinnedMeshRenderer>(skinnedMeshesToCombine));
    //     if (additionalBones.Length > 0)
    //     {
    //         JoinAdditionalBonesToBoneArray(bones, additionalBones, boneNameMap);
    //         // Need to redo the name map now that we have updated the bone array.
    //         boneNameMap = CreateBoneNameMap(rootBone);
    //     }

    //     List<CombineInstance> combineInstances = new List<CombineInstance>();
    //     List<Matrix4x4> bindPosesToMerge = new List<Matrix4x4>();

    //     // Iterate through the skinned meshes and process them into Material groupings, and also process the bones as required.
    //     foreach (SkinnedMeshRenderer child in skinnedMeshesToCombine)
    //     {
    //         material = child.sharedMaterial;

    //         mesh = CopyMesh(child.sharedMesh);

    //         boneCount += child.bones.Length;

    //         Transform[] existingBones = bones;
    //         bones = new Transform[boneCount];
    //         Array.Copy(existingBones, bones, existingBones.Length);
    //         Transform[] newBones = new Transform[child.bones.Length];

    //         for (int i = 0; i < newBones.Length; i++)
    //         {
    //             Transform currentBone = (Transform)boneNameMap[child.bones[i].name];

    //             newBones[i] = currentBone;
    //             bindPosesToMerge.Add(currentBone.worldToLocalMatrix * child.transform.worldToLocalMatrix);
    //         }
    //         Array.Copy(newBones, 0, bones, boneOffset, child.bones.Length);

    //         boneOffset = bones.Length;

    //         Matrix4x4 transformMatrix = child.localToWorldMatrix;

    //         CombineInstance combineInstance = new CombineInstance();
    //         combineInstance.mesh = mesh;
    //         combineInstance.transform = transformMatrix;
    //         combineInstances.Add(combineInstance);
    //     }

    //     SkinnedMeshRenderer renderer = combinedSkinnedMesh.AddComponent<SkinnedMeshRenderer>();
    //     renderer.bones = bones;
    //     Mesh newMesh = new Mesh();
    //     newMesh.CombineMeshes(combineInstances.ToArray(), true, true);
    //     newMesh.RecalculateBounds();
    //     newMesh.name = combinedModel.name;
    //     renderer.rootBone = combinedSkinnedMesh.transform.Find("root");
    //     renderer.sharedMesh = newMesh;
    //     renderer.sharedMesh.bindposes = bindPosesToMerge.ToArray();
    //     renderer.sharedMaterial = baseMaterial == null ? material : baseMaterial;
    //     MergeAndGetAllBlendShapeDataOfSkinnedMeshRenderers(skinnedMeshesToCombine.ToArray(), renderer.sharedMesh, renderer);

    //     return combinedModel;
    // }

    // private static GameObject CreateCharacter(List<SkinnedMeshRenderer> toCombine, GameObject baseModel, GameObject parentModel, Material baseMaterial)
    // {
    //   //  PopulateUVDictionary(toCombine);
    //     GameObject newSpawn = CreateCombinedSkinnedMesh(toCombine, baseModel, parentModel, baseMaterial);
    //     newSpawn.name = "Skinned Combined";
    //     Renderer renderer = newSpawn.GetComponentInChildren<Renderer>();
    //     if (renderer != null)
    //     {
    //         renderer.sharedMaterial = baseMaterial;
    //     }
    //     Animator newModelAnimator = newSpawn.AddComponent<Animator>();
    //     Animator baseModelAnimator = baseModel.GetComponentInChildren<Animator>();
    //     newModelAnimator.avatar = baseModelAnimator.avatar;
    //     newModelAnimator.Rebind();
    //     CharacterCustomisationEconomy characterCustomisation = parentModel.GetComponent<Character>().CustomisationData;
    //     float heavyBlendValue = characterCustomisation.bodyType > 0 ? characterCustomisation.bodyType : 0;
    //     float skinnyBlendValue = characterCustomisation.bodyType < 0 ? Math.Abs(characterCustomisation.bodyType) : 0;

    //     UpdateBlendShapes(newSpawn, characterCustomisation.bodyGenderType, skinnyBlendValue, heavyBlendValue, characterCustomisation.bodyMuscleType);
    //     return newSpawn;
    // }

    // public static void CombineCharacterChildMeshes(List<SkinnedMeshRenderer> toCombine, GameObject baseModel, GameObject parentModel, Material material)
    // {
    //     GameObject clonedModel = CreateCharacter(toCombine, baseModel, parentModel, material);

    //     SkinnedMeshRenderer clonedRenderer = clonedModel.GetComponentInChildren<SkinnedMeshRenderer>();

    //     // Copy mesh, bone weights and bindposes before baking so the mesh can be re-skinned after baking.
    //     Mesh clonedSkinnedMesh = CopyMesh(clonedRenderer.sharedMesh);
    //     BoneWeight[] boneWeights = clonedSkinnedMesh.boneWeights;
    //     Matrix4x4[] bindposes = clonedSkinnedMesh.bindposes;
    //     List<BlendShapeData> blendData = GetBlendShapeData(
    //         clonedSkinnedMesh,
    //         clonedRenderer,
    //         new string[]
    //         {
    //                 "defaultHeavy",
    //                 "defaultBuff",
    //                 "defaultSkinny",
    //                 "masculineFeminine"
    //         },
    //         0
    //     );
    //     clonedRenderer.BakeMesh(clonedSkinnedMesh);
    //     // Re-skin the new baked mesh.
    //     clonedSkinnedMesh.boneWeights = boneWeights;
    //     clonedSkinnedMesh.bindposes = bindposes;
    //     // assign the new mesh to the renderer
    //     clonedRenderer.sharedMesh = clonedSkinnedMesh;

    //     RestoreBlendShapeData(blendData, clonedSkinnedMesh, clonedRenderer);

    //     // now do the bone movements!
    //     CharacterCustomisationEconomy characterCustomisation = parentModel.GetComponent<Character>().CustomisationData;
    //     float heavyBlendValue = characterCustomisation.bodyType > 0 ? characterCustomisation.bodyType : 0;
    //     float skinnyBlendValue = characterCustomisation.bodyType < 0 ? Math.Abs(characterCustomisation.bodyType) : 0;
    //     ProcessRigMovementOnBlendShapeChange(baseModel, characterCustomisation.bodyGenderType, skinnyBlendValue, heavyBlendValue, characterCustomisation.bodyMuscleType);
    //     ProcessBoneMovement(clonedModel);

    //     clonedRenderer.sharedMaterial = material;
    // }

    // private static void UpdateBlendShapes(GameObject model, float _bodyTypeBlendValue, float _bodySizeSkinnyBlendValue,
    //     float _bodySizeHeavyBlendValue, float _musclesBlendValue)
    // {
    //     if (model == null)
    //     {
    //         return;
    //     }

    //     List<SkinnedMeshRenderer> allMeshes = model.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
    //     foreach (SkinnedMeshRenderer skinnedMesh in allMeshes)
    //     {
    //         Mesh sharedMesh = skinnedMesh.sharedMesh;
    //         for (int i = 0; i < sharedMesh.blendShapeCount; i++)
    //         {
    //             string blendName = sharedMesh.GetBlendShapeName(i);
    //             if (blendName.Contains(StringUtils.BLEND_GENDER))
    //             {
    //                 skinnedMesh.SetBlendShapeWeight(i, (_bodyTypeBlendValue + 100) / 2);
    //             }
    //             else if (blendName.Contains(StringUtils.BLEND_SHAPE_SKINNY))
    //             {
    //                 skinnedMesh.SetBlendShapeWeight(i, _bodySizeSkinnyBlendValue);
    //             }
    //             else if (blendName.Contains(StringUtils.BLEND_SHAPE_HEAVY))
    //             {
    //                 skinnedMesh.SetBlendShapeWeight(i, _bodySizeHeavyBlendValue);
    //             }
    //             else if (blendName.Contains(StringUtils.BLEND_MUSCLE))
    //             {
    //                 skinnedMesh.SetBlendShapeWeight(i, (_musclesBlendValue + 100) / 2);
    //             }
    //         }
    //     }
    // }
    // // Feminine_Offset Values
    // private static readonly Vector3 FEMININE_OFFSET_HIP_ATTACH_BACK = new Vector3(0.0f, 0.00095f, 0.00072f);
    // private static readonly Vector3 FEMININE_OFFSET_HIP_ATTACH_L = new Vector3(-0.0031f, -0.00241f, 0.00727f);
    // private static readonly Vector3 FEMININE_OFFSET_HIP_ATTACH_FRONT = new Vector3(0.0f, 0.00138f, -0.01824f);
    // private static readonly Vector3 FEMININE_OFFSET_KNEE_ATTACH_L = new Vector3(-0.00064f, -0.00106f, -0.001f);
    // private static readonly Vector3 FEMININE_OFFSET_KNEE_ATTACH_R = new Vector3(-0.00064f, -0.00106f, -0.001f);
    // private static readonly Vector3 FEMININE_OFFSET_HIP_ATTACH_R = new Vector3(0.0031f, -0.00241f, 0.00727f);
    // private static readonly Vector3 FEMININE_OFFSET_ELBOW_ATTACH_R = new Vector3(0.00125f, -0.00021f, 0.0093f);
    // private static readonly Vector3 FEMININE_OFFSET_SHOULDER_ATTACH_R = new Vector3(-0.01559f, -0.00467f, 0.00124f);
    // private static readonly Vector3 FEMININE_OFFSET_BACK_ATTACH = new Vector3(0.0f, 0.00137f, 0.01903f);
    // private static readonly Vector3 FEMININE_OFFSET_SHOULDER_ATTACH_L = new Vector3(-0.01559f, -0.00467f, -0.00124f);
    // private static readonly Vector3 FEMININE_OFFSET_ELBOW_ATTACH_L = new Vector3(-0.00125f, -0.00021f, 0.0093f);

    // // Heavy_Offset Values
    // private static readonly Vector3 HEAVY_OFFSET_HIP_ATTACH_BACK = new Vector3(0.0f, -0.02628f, -0.15593f);
    // private static readonly Vector3 HEAVY_OFFSET_HIP_ATTACH_L = new Vector3(-0.14163f, 0.00034f, 0.00718f);
    // private static readonly Vector3 HEAVY_OFFSET_HIP_ATTACH_FRONT = new Vector3(0.0f, -0.06413f, 0.14101f);
    // private static readonly Vector3 HEAVY_OFFSET_KNEE_ATTACH_L = new Vector3(0.00331f, -0.00084f, 0.02737f);
    // private static readonly Vector3 HEAVY_OFFSET_KNEE_ATTACH_R = new Vector3(0.00331f, -0.00084f, 0.02737f);
    // private static readonly Vector3 HEAVY_OFFSET_HIP_ATTACH_R = new Vector3(0.14163f, 0.00034f, 0.00718f);
    // private static readonly Vector3 HEAVY_OFFSET_ELBOW_ATTACH_R = new Vector3(0.0f, 0.00138f, -0.02725f);
    // private static readonly Vector3 HEAVY_OFFSET_SHOULDER_ATTACH_R = new Vector3(-0.00707f, 0.03154f, 0.00351f);
    // private static readonly Vector3 HEAVY_OFFSET_BACK_ATTACH = new Vector3(0.0f, 0.02174f, -0.07185f);
    // private static readonly Vector3 HEAVY_OFFSET_SHOULDER_ATTACH_L = new Vector3(-0.00707f, 0.03154f, -0.00351f);
    // private static readonly Vector3 HEAVY_OFFSET_ELBOW_ATTACH_L = new Vector3(0.0f, 0.00138f, -0.02725f);

    // // Skinny_Offset Values
    // private static readonly Vector3 SKINNY_OFFSET_HIP_ATTACH_BACK = new Vector3(0.0f, -0.0042f, 0.00849f);
    // private static readonly Vector3 SKINNY_OFFSET_HIP_ATTACH_L = new Vector3(0.01571f, -0.00161f, -0.00123f);
    // private static readonly Vector3 SKINNY_OFFSET_HIP_ATTACH_FRONT = new Vector3(0.0f, 0.00159f, -0.01787f);
    // private static readonly Vector3 SKINNY_OFFSET_KNEE_ATTACH_L = new Vector3(0.00167f, 0.00145f, -0.00388f);
    // private static readonly Vector3 SKINNY_OFFSET_KNEE_ATTACH_R = new Vector3(0.00167f, 0.00145f, -0.00388f);
    // private static readonly Vector3 SKINNY_OFFSET_HIP_ATTACH_R = new Vector3(-0.01571f, -0.00161f, -0.00123f);
    // private static readonly Vector3 SKINNY_OFFSET_ELBOW_ATTACH_R = new Vector3(0.00128f, -0.00342f, 0.01043f);
    // private static readonly Vector3 SKINNY_OFFSET_SHOULDER_ATTACH_R = new Vector3(0.0003f, -0.00818f, 0.00089f);
    // private static readonly Vector3 SKINNY_OFFSET_BACK_ATTACH = new Vector3(-0.00001f, -0.00079f, 0.00938f);
    // private static readonly Vector3 SKINNY_OFFSET_SHOULDER_ATTACH_L = new Vector3(0.0003f, -0.00818f, -0.00089f);
    // private static readonly Vector3 SKINNY_OFFSET_ELBOW_ATTACH_L = new Vector3(-0.00128f, -0.00342f, 0.01043f);

    // // Bulk_Offset Values
    // private static readonly Vector3 BULK_OFFSET_HIP_ATTACH_BACK = new Vector3(0.0f, 0.00093f, -0.01182f);
    // private static readonly Vector3 BULK_OFFSET_HIP_ATTACH_L = new Vector3(0.00115f, 0.00139f, 0.00299f);
    // private static readonly Vector3 BULK_OFFSET_HIP_ATTACH_FRONT = new Vector3(0.0f, 0.00132f, 0.00489f);
    // private static readonly Vector3 BULK_OFFSET_KNEE_ATTACH_L = new Vector3(0.00041f, 0.00005f, 0.00033f);
    // private static readonly Vector3 BULK_OFFSET_KNEE_ATTACH_R = new Vector3(0.00041f, 0.00005f, 0.00033f);
    // private static readonly Vector3 BULK_OFFSET_HIP_ATTACH_R = new Vector3(-0.00115f, 0.00139f, 0.00299f);
    // private static readonly Vector3 BULK_OFFSET_ELBOW_ATTACH_R = new Vector3(0.00609f, 0.01381f, -0.06119f);
    // private static readonly Vector3 BULK_OFFSET_SHOULDER_ATTACH_R = new Vector3(0.02127f, 0.04615f, -0.00861f);
    // private static readonly Vector3 BULK_OFFSET_BACK_ATTACH = new Vector3(0.0f, 0.00465f, -0.03104f);
    // private static readonly Vector3 BULK_OFFSET_SHOULDER_ATTACH_L = new Vector3(0.02127f, 0.04615f, 0.00861f);
    // private static readonly Vector3 BULK_OFFSET_ELBOW_ATTACH_L = new Vector3(-0.00609f, 0.01381f, -0.06119f);

    // public enum CharacterPartType
    // {
    //     // Specify the starting value as 1 to ensure values match server side values.
    //     Head = 1,
    //     Hair,
    //     EyebrowLeft,
    //     EyebrowRight,
    //     EyeLeft,
    //     EyeRight,
    //     EarLeft,
    //     EarRight,
    //     FacialHair,
    //     Torso,
    //     ArmUpperLeft,
    //     ArmUpperRight,
    //     ArmLowerLeft,
    //     ArmLowerRight,
    //     HandLeft,
    //     HandRight,
    //     Hips,
    //     LegLeft,
    //     LegRight,
    //     FootLeft,
    //     FootRight,
    //     AttachmentHead,
    //     AttachmentFace,
    //     AttachmentBack,
    //     AttachmentHipsFront,
    //     AttachmentHipsBack,
    //     AttachmentHipsLeft,
    //     AttachmentHipsRight,
    //     AttachmentShoulderLeft,
    //     AttachmentShoulderRight,
    //     AttachmentElbowLeft,
    //     AttachmentElbowRight,
    //     AttachmentKneeLeft,
    //     AttachmentKneeRight,
    //     Nose,
    //     Teeth,
    //     Tongue,

    // }
    // public static readonly Dictionary<CharacterPartType, string> PART_TYPE_JOINT_MAP = new Dictionary<CharacterPartType, string>
    // {
    //     [CharacterPartType.AttachmentBack] = "backAttach",
    //     [CharacterPartType.AttachmentHipsFront] = "hipAttachFront",
    //     [CharacterPartType.AttachmentHipsBack] = "hipAttachBack",
    //     [CharacterPartType.AttachmentHipsLeft] = "hipAttach_l",
    //     [CharacterPartType.AttachmentHipsRight] = "hipAttach_r",
    //     [CharacterPartType.AttachmentShoulderLeft] = "shoulderAttach_l",
    //     [CharacterPartType.AttachmentShoulderRight] = "shoulderAttach_r",
    //     [CharacterPartType.AttachmentElbowLeft] = "elbowAttach_l",
    //     [CharacterPartType.AttachmentElbowRight] = "elbowAttach_r",
    //     [CharacterPartType.AttachmentKneeLeft] = "kneeAttach_l",
    //     [CharacterPartType.AttachmentKneeRight] = "kneeAttach_r"
    // };
    // public static Vector3 GetCombinedOffsetValue(
    //    float blendValueFeminine,
    //    float blendValueSize,
    //    float blendValueMuscle,
    //    Vector3 currentPosition,
    //    CharacterPartType partType
    //)
    // {
    //     switch (partType)
    //     {
    //         case CharacterPartType.AttachmentBack:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_BACK_ATTACH, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_BACK_ATTACH, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_BACK_ATTACH, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_BACK_ATTACH, -blendValueSize));
    //         case CharacterPartType.AttachmentHipsFront:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_HIP_ATTACH_FRONT, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_HIP_ATTACH_FRONT, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_HIP_ATTACH_FRONT, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_HIP_ATTACH_FRONT, -blendValueSize));
    //         case CharacterPartType.AttachmentHipsBack:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_HIP_ATTACH_BACK, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_HIP_ATTACH_BACK, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_HIP_ATTACH_BACK, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_HIP_ATTACH_BACK, -blendValueSize));
    //         case CharacterPartType.AttachmentHipsLeft:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_HIP_ATTACH_L, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_HIP_ATTACH_L, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_HIP_ATTACH_L, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_HIP_ATTACH_L, -blendValueSize));
    //         case CharacterPartType.AttachmentHipsRight:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_HIP_ATTACH_R, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_HIP_ATTACH_R, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_HIP_ATTACH_R, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_HIP_ATTACH_R, -blendValueSize));
    //         case CharacterPartType.AttachmentShoulderLeft:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_SHOULDER_ATTACH_L, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_SHOULDER_ATTACH_L, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_SHOULDER_ATTACH_L, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_SHOULDER_ATTACH_L, -blendValueSize));
    //         case CharacterPartType.AttachmentShoulderRight:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_SHOULDER_ATTACH_R, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_SHOULDER_ATTACH_R, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_SHOULDER_ATTACH_R, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_SHOULDER_ATTACH_R, -blendValueSize));
    //         case CharacterPartType.AttachmentElbowLeft:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_ELBOW_ATTACH_L, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_ELBOW_ATTACH_L, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_ELBOW_ATTACH_L, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_ELBOW_ATTACH_L, -blendValueSize));
    //         case CharacterPartType.AttachmentElbowRight:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_ELBOW_ATTACH_R, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_ELBOW_ATTACH_R, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_ELBOW_ATTACH_R, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_ELBOW_ATTACH_R, -blendValueSize));
    //         case CharacterPartType.AttachmentKneeLeft:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_KNEE_ATTACH_L, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_KNEE_ATTACH_L, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_KNEE_ATTACH_L, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_KNEE_ATTACH_L, -blendValueSize));
    //         case CharacterPartType.AttachmentKneeRight:
    //             return currentPosition
    //                 + Vector3.Lerp(Vector3.zero, FEMININE_OFFSET_KNEE_ATTACH_R, blendValueFeminine)
    //                 + Vector3.Lerp(Vector3.zero, BULK_OFFSET_KNEE_ATTACH_R, blendValueMuscle)
    //                 + (blendValueSize > 0
    //                     ? Vector3.Lerp(Vector3.zero, HEAVY_OFFSET_KNEE_ATTACH_R, blendValueSize)
    //                     : Vector3.Lerp(Vector3.zero, SKINNY_OFFSET_KNEE_ATTACH_R, -blendValueSize));
    //     }

    //     return Vector3.zero;
    // }
    // private static void ProcessRigMovementOnBlendShapeChange(GameObject baseModel, float _bodyTypeBlendValue, float _bodySizeSkinnyBlendValue,
    //     float _bodySizeHeavyBlendValue, float _musclesBlendValue)
    // {
    //     GameObject donorModel = baseModel;
    //     Transform modelRootBone = donorModel.transform.Find("root");
    //     Hashtable boneNameMap = CreateBoneNameMap(modelRootBone.gameObject);

    //     foreach (KeyValuePair<CharacterPartType, string> entry in PART_TYPE_JOINT_MAP)
    //     {
    //         Transform bone = (Transform)boneNameMap[entry.Value];
    //         Vector3 bonePosition = bone.position;
    //         float shapeBlendValue = 0f;
    //         if (_bodySizeHeavyBlendValue > 0)
    //         {
    //             shapeBlendValue = _bodySizeHeavyBlendValue / 100;
    //         }
    //         else
    //         {
    //             shapeBlendValue = -(_bodySizeSkinnyBlendValue / 100);
    //         }

    //         float feminineBlendValue = _bodyTypeBlendValue > 0 ? _bodyTypeBlendValue / 100 : 0;

    //         Vector3 allMovement = GetCombinedOffsetValue(
    //             feminineBlendValue,
    //             shapeBlendValue,
    //             (_musclesBlendValue + 100) / 2 / 100,
    //             bonePosition,
    //             entry.Key
    //         );

    //         _blendShapeRigMovement[entry.Value] = allMovement;
    //     }
    // }
    // private static Dictionary<string, Vector3> _blendShapeRigMovement = new Dictionary<string, Vector3>();

    // private static void ProcessBoneMovement(GameObject model)
    // {
    //     Transform modelRootBone = model.transform.Find("root");
    //     Hashtable boneNameMap = CreateBoneNameMap(modelRootBone.gameObject);
    //     ProcessBoneMovement(boneNameMap, _blendShapeRigMovement);
    // }

    // public static void ProcessBoneMovement(Hashtable boneNameMap, Dictionary<string, Vector3> movementDictionary)
    // {
    //     Dictionary<string, Vector3> bonePositionDictionary = new Dictionary<string, Vector3>();
    //     Dictionary<string, Vector3> boneMovementDictionary = new Dictionary<string, Vector3>();
    //     foreach (Transform currentBone in boneNameMap.Values)
    //     {
    //         // Store bone positions from rig before processing joints.
    //         bonePositionDictionary.TryAdd(currentBone.name, currentBone.transform.position);

    //         if (movementDictionary.ContainsKey(currentBone.name))
    //         {
    //             float jointDistance = Vector3.Distance(bonePositionDictionary[currentBone.name], movementDictionary[currentBone.name]);

    //             // If the bone in the new part is at a different location, move the actual bone to the same position.
    //             if (jointDistance > 0.01)
    //             {
    //                 Vector3 rigMovement = movementDictionary[currentBone.name];
    //                 // If an existing joint movement exists, and is further from the standard joint position, use that instead.
    //                 if (boneMovementDictionary.TryGetValue(currentBone.name, out Vector3 existingMovement)
    //                     && Math.Abs(Vector3.Distance(bonePositionDictionary[currentBone.name], existingMovement)) > Math.Abs(jointDistance))
    //                 {
    //                     rigMovement = existingMovement;
    //                 }

    //                 currentBone.transform.position = rigMovement;
    //                 boneMovementDictionary[currentBone.name] = rigMovement;
    //             }
    //         }
    //     }
    // }
    #endregion
}
