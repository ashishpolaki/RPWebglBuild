using CharacterCustomisation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPartUI : MonoBehaviour
{
    #region Inspector variables
    [SerializeField] private CharacterPartUIType characterPartType;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI partText;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        forwardButton.onClick.AddListener(OnForwardButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    private void OnDisable()
    {
        forwardButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }
    #endregion

    #region Public Methods
    public void SetPart(CharacterPartUIType _characterPart)
    {
        characterPartType = _characterPart;
        partText.text = characterPartType.ToString();
        CharacterCustomisationManager.Instance.SetCurrentSelectedPart(characterPartType);
    }
    #endregion

    #region Private Methods
    private void OnForwardButtonClicked()
    {
        CharacterCustomisationManager.Instance.ChangeBodyPart(characterPartType,1);
    }
    private void OnBackButtonClicked()
    {
        CharacterCustomisationManager.Instance.ChangeBodyPart(characterPartType,-1);
    }
    #endregion
}
