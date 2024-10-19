using System;
using System.Threading.Tasks;
using UnityEngine;

#region UGS
public class PlayerAccountManager : MonoBehaviour
{
    #region Unity Methods
    private void OnEnable()
    {
        UGSManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;
    }
    private void OnDisable()
    {
        if (UGSManager.Instance != null)
        {
            UGSManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
        }
    }
    private async void Start()
    {
        //Check if user is already signed in
        if (UGSManager.Instance.Authentication.IsSignInCached())
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.CacheSignInAsync();
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        //If not signed in, open login screen
        else
        {
            UI.UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
        }

        //Request GPS permission
        GPS.RequestPermission();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Sign in successful event
    /// </summary>
    private void SignInSuccessful()
    {
        UI.UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
    }
    #endregion
}
#endregion
