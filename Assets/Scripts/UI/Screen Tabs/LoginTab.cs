using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LoginTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField username_Input;
        [SerializeField] private InputField password_Input;
        [SerializeField] private Button loginBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            ResetInputFields();
            loginBtn.onClick.AddListener(() => Login());
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed += OnSignInFailed;
                UGSManager.Instance.Authentication.OnValidationFail += OnValidationFailed;
            }
        }
        private void OnDisable()
        {
            loginBtn.onClick.RemoveAllListeners();
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignInFailed -= OnSignInFailed;
                UGSManager.Instance.Authentication.OnValidationFail -= OnValidationFailed;
            }
        }
        #endregion

        #region Subscribed Events
        /// <summary>
        /// Sign in failed event
        /// </summary>
        /// <param name="message"></param>
        private void OnSignInFailed(string message)
        {
            errorMessageTxt.text = message;
        }

        private void OnValidationFailed(string obj)
        {
            errorMessageTxt.text = obj;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reset UI Fields
        /// </summary>
        private void ResetInputFields()
        {
            username_Input.text = string.Empty;
            password_Input.text = string.Empty;
            errorMessageTxt.text = string.Empty;
        }
      
        /// <summary>
        /// Login Request
        /// </summary>
        private async void Login()
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.SignInWithUsernamePasswordAsync(username_Input.text, password_Input.text);
             await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion
    }
}
