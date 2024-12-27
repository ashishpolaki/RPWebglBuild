using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace UGS
{
    public class Authentication
    {
        #region Events
        public event Action OnSignedInEvent;
        public event Action<string> OnSignInFailed;
        public event Action OnSignedOut;
        public event Action OnSessionExpired;
        public event Action<string> OnPlayerNameChanged;
        public event Action<string> OnValidationFail;
        #endregion

        #region Properties
        public string PlayerID { get; private set; }
        public string PlayerName { get; private set; }
        #endregion

        public Authentication()
        {
        }

        #region Initialize
        public async Task InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
                SubscribeEvents();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
        #endregion

        #region PlayerName
        //public async Task GenerateRandomPlayerName()
        //{
        //    try
        //    {
        //        PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        // Compare error code to AuthenticationErrorCodes
        //        // Notify the player with the proper error message
        //        Debug.LogException(ex);
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        // Compare error code to CommonErrorCodes
        //        // Notify the player with the proper error message
        //        Debug.LogException(ex);
        //    }
        //    OnPlayerNameChanged?.Invoke(PlayerName);
        //}
        public async Task<string> SetPlayerNameAsync(string _playerName)
        {
            try
            {
              
                PlayerName = await AuthenticationService.Instance.UpdatePlayerNameAsync(_playerName);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            OnPlayerNameChanged?.Invoke(PlayerName);
            return string.Empty;
        }
        #endregion

        #region Public Methods
        public void ResetData()
        {
            PlayerID = string.Empty;
            PlayerName = string.Empty;
        }

        public bool IsCurrentlySignedIn()
        {
            return AuthenticationService.Instance.IsSignedIn;
        }

        public bool IsSignInCached()
        {
            // Check if a cached player already exists by checking if the session token exists
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                 return true;
            }
            return false;
        }

        public bool CheckUsernameCriteria(string userName)
        {
            return new Regex(StringUtils.USERNAMEPATTERN, RegexOptions.IgnoreCase).IsMatch(userName);
        }
        public bool CheckPasswordCriteria(string password)
        {
            return new Regex(StringUtils.PASSWORDPATTERN).IsMatch(password);
        }
      

        public async Task SignUpAsync(string userName, string password)
        {
            try
            {
                //Check if username meets the criteria.
                if (!CheckUsernameCriteria(userName))
                {
                    OnValidationFail?.Invoke(StringUtils.USERNAMEERROR);
                    return;
                }
                //Check if password meets the criteria.
                if (!CheckPasswordCriteria(password))
                {
                    OnValidationFail?.Invoke(StringUtils.PASSWORDERROR);
                    return;
                }
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(userName, password);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                // Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public async Task SignInWithUsernamePasswordAsync(string username, string password)
        {
            try
            {
                //Check if username meets the criteria.
                if (!CheckUsernameCriteria(username))
                {
                    OnValidationFail?.Invoke(StringUtils.USERNAMEERROR);
                    return;
                }
                //Check if password meets the criteria.
                if (!CheckPasswordCriteria(password))
                {
                    OnValidationFail?.Invoke(StringUtils.PASSWORDERROR);
                    return;
                }
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.Log(ex.Message);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.Log(ex.Message);
            }
        }

        public async Task SignInAnonymouslyAsync()
        {
            //if session token is available, sign out the player
            if (IsSignInCached())
            {
                AuthenticationService.Instance.ClearSessionToken();
            }

            //Random signin for anonymous user with random username
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public async Task CacheSignInAsync()
        {
            //Random signin for anonymous user with random username
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public void Signout()
        {
            AuthenticationService.Instance.SignOut(true);
            AuthenticationService.Instance.ClearSessionToken();
        }
        #endregion

        #region Subscribe/Desubscribe Methods

        private void SubscribeEvents()
        {
            AuthenticationService.Instance.SignedIn += () => HandleSignInSuccessEvent();

            AuthenticationService.Instance.SignInFailed += (err) => HandleSigninFailedEvent(err);

            AuthenticationService.Instance.SignedOut += () => HandleSignOutEvent();

            AuthenticationService.Instance.Expired += () => HandleSessionExpireEvent();
        }

        public void DeSubscribeEvents()
        {
            AuthenticationService.Instance.SignedIn -= () => HandleSignInSuccessEvent();

            AuthenticationService.Instance.SignInFailed -= (err) => HandleSigninFailedEvent(err);

            AuthenticationService.Instance.SignedOut -= () => HandleSignOutEvent();

            AuthenticationService.Instance.Expired -= () => HandleSessionExpireEvent();
        }

        private async void HandleSignInSuccessEvent()
        {
            PlayerID = AuthenticationService.Instance.PlayerId;
            PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync(false);
            OnSignedInEvent?.Invoke();
        }

        private void HandleSigninFailedEvent(RequestFailedException err)
        {
            OnSignInFailed?.Invoke(err.Message);
        }
        private void HandleSignOutEvent()
        {
            OnSignedOut?.Invoke();
            Debug.Log("Player signed out.");
        }
        private void HandleSessionExpireEvent()
        {
            Debug.Log("Player session could not be refreshed and expired.");
        }
        #endregion
    }
}

