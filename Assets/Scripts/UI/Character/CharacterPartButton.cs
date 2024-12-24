using UnityEngine;
using UnityEngine.UI;

public class CharacterPartButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImg;
    [SerializeField] private Outline outline;
    private CharacterPartUIType currentCharacterPartType;
    private bool isSelected;

    public delegate void OnCharacterPartButtonClicked(CharacterPartUIType _currentCharacterPartType);
    public OnCharacterPartButtonClicked onCharacterPartButtonClicked;

    public CharacterPartUIType CurrentCharacterPartType => currentCharacterPartType;

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void SetData(CharacterPartUIType _currentCharacterPartType,Sprite sprite)
    {
        currentCharacterPartType = _currentCharacterPartType;
        iconImg.sprite = sprite;
    }

    public void SetSelected(bool _isSelected)
    {
        isSelected = _isSelected;
        outline.enabled = isSelected;
    }

    private void OnButtonClick()
    {
        if(isSelected)
        {
            return;
        }
        onCharacterPartButtonClicked?.Invoke(currentCharacterPartType);
    }
}
