using UnityEngine;

public class ControlPointsGroup : MonoBehaviour
{
#if UNITY_EDITOR
    public bool canValidate = false;
    private void OnValidate()
    {
        if (canValidate)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject controlPoint = transform.GetChild(i).gameObject;
                controlPoint.name = $"ControlPoint Group ({i + 1})";
            }
        }
    }
#endif
}
