using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

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
}
