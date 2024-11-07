using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using CharacterCustomisation;

namespace UI.Screen.Tab
{
    public class CharacterCustomisationTab : BaseTab
    {
        #region Inspector variables
        [SerializeField] private List<CharacterPartSelectUI> characterPartButtonsList = new List<CharacterPartSelectUI>();
        [SerializeField] private List<Button> bodyPartsColorList = new List<Button>();
        [SerializeField] private CharacterPartUI characterPartUI;
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            foreach (var button in bodyPartsColorList)
            {
                button.onClick.AddListener(() => OnBodyPartColorChange(button));
            }
            maleButton.onClick.AddListener(() => OnChangeGender(CharacterGenderType.Male));
            femaleButton.onClick.AddListener(() => OnChangeGender(CharacterGenderType.Female));
        }
        protected override void Start()
        {
            InitializeCharacterPartUI();
            OnSelectBodyPart(0);
            OnChangeGender(CharacterCustomisationManager.Instance.CurrentCharacterGender);
            base.Start();
        }

        private void OnDisable()
        {
            foreach (var button in bodyPartsColorList)
            {
                button.onClick.RemoveAllListeners();
            }
            maleButton.onClick.RemoveListener(() => OnChangeGender(CharacterGenderType.Male));
            femaleButton.onClick.RemoveListener(() => OnChangeGender(CharacterGenderType.Female));
        }
        #endregion

        private void OnBodyPartColorChange(Button button)
        {
            Color color = button.image.color;
            CharacterCustomisationManager.Instance.ChangeBodyPartColor(color);
        }
        private void OnChangeGender(CharacterGenderType characterGender)
        {
            CharacterCustomisationManager.Instance.ChangeGender(characterGender);
            UpdateGenderButtonsState(characterGender);
        }
        private void UpdateGenderButtonsState(CharacterGenderType characterGender)
        {
            bool isMale = characterGender == CharacterGenderType.Male;
            maleButton.interactable = !isMale;
            femaleButton.interactable = isMale;
        }

        private void InitializeCharacterPartUI()
        {
            int characterPartListCount = Enum.GetValues(typeof(CharacterPartUIType)).Length;
            for (int i = 0; i < characterPartListCount; i++)
            {
                characterPartButtonsList[i].Initialize((CharacterPartUIType)i, OnSelectBodyPart);
            }
        }

        private void OnSelectBodyPart(CharacterPartUIType characterPartUIType)
        {
            characterPartUI.SetPart(characterPartUIType);
        }
    }
}