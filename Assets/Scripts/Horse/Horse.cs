using UnityEngine;

public class Horse : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] horseMeshes;

    public void SetHorseMaterial(Material material)
    {
        foreach (var mesh in horseMeshes)
        {
            mesh.material = material;
        }
    }
}
