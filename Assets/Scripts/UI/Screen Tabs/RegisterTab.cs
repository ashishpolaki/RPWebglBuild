using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RegisterTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField username_Input;
        [SerializeField] private InputField password_Input;
        [SerializeField] private Button registerPlayer_btn;
        [SerializeField] private TextMeshProUGUI errorMessage_txt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            registerPlayer_btn.onClick.AddListener(() => RegisterPlayer());
            ClearInputFields();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed += OnSignUpFailed;
                UGSManager.Instance.Authentication.OnValidationFail += OnValidationFailed;
            }
        }
        private void OnDisable()
        {
            registerPlayer_btn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed -= OnSignUpFailed;
                UGSManager.Instance.Authentication.OnValidationFail -= OnValidationFailed;
            }
        }
        #endregion

        #region Subscribed Events

        /// <summary>
        /// Sign up failed event, Show error message
        /// </summary>
        /// <param name="message"></param>
        private void OnSignUpFailed(string message)
        {
            errorMessage_txt.text = message;
        }

        private void OnValidationFailed(string obj)
        {
            errorMessage_txt.text = obj;
        }
        #endregion



        #region Private Methods
        /// <summary>
        /// Reset UI Fields
        /// </summary>
        private void ClearInputFields()
        {
            username_Input.text = string.Empty;
            password_Input.text = string.Empty;
            errorMessage_txt.text = string.Empty;
        }

        /// <summary>
        /// Check Username and Password Criteria and Sign up
        /// </summary>
        private async void RegisterPlayer()
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.SignUpAsync(username_Input.text, password_Input.text);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion
    }
}
