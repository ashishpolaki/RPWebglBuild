using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character/Character")]
public class CharacterSO : ScriptableObject
{
    [SerializeField] private Mesh hair;
    [SerializeField] private Mesh hat; 
}
