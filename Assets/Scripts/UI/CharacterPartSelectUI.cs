using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPartSelectUI : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private CharacterPartUIType characterPartUIType;
    [SerializeField] private TextMeshProUGUI partText;

    private Action<CharacterPartUIType> selectBodyPartAction;

    public void Initialize(CharacterPartUIType characterPartUIType,Action<CharacterPartUIType> action)
    {
        this.characterPartUIType = characterPartUIType;
        partText.text = characterPartUIType.ToString();
        selectBodyPartAction = action;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectBodyPartAction?.Invoke(characterPartUIType);
    }
}
