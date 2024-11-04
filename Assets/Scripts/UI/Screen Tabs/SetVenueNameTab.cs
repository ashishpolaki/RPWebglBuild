using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class SetVenueNameTab : BaseTab
    {
        [SerializeField] private Button logOutButton;
        [SerializeField] private Button setVenueNameButton;
        [SerializeField] private TMP_InputField venueInputField;

        public override void Open()
        {
            base.Open();
            if (logOutButton != null)
                logOutButton.onClick.AddListener(() => OnLogOut());
        }

        public override void Close()
        {
            base.Close();
            if (logOutButton != null)
                logOutButton.onClick.RemoveAllListeners();
        }

        private void OnLogOut()
        {
        }
    }
}