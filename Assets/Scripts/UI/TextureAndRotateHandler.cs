using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextureAndRotateHandler : MonoBehaviour, IDragHandler
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private RawImage rawImage;

    public void OnEnable()
    {
        BodyShapeCustomisationUI.OnCharacterAssign += OnCharacterAssign;
        CharacterHeadCustomisationUI.OnCharacterAssign += OnCharacterAssign;
        SkinColorCustomisationUI.OnCharacterAssign += OnCharacterAssign;
    }

    public void OnDisable()
    {
        BodyShapeCustomisationUI.OnCharacterAssign -= OnCharacterAssign;
        CharacterHeadCustomisationUI.OnCharacterAssign -= OnCharacterAssign;
        SkinColorCustomisationUI.OnCharacterAssign -= OnCharacterAssign;
    }

    public void OnCharacterAssign(Texture texture, Transform character)
    {
        rawImage.texture = texture;
        targetTransform = character;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (targetTransform != null)
        {
             float rotationAmount = eventData.delta.x * rotateSpeed;
            targetTransform.Rotate(Vector3.up, -rotationAmount);
        }
    }
}
