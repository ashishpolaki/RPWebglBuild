using UnityEngine;

#region UGS
public class PlayerAccountManager : MonoBehaviour
{
    #region Unity Methods
    private void Start()
    {
        UI.UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);

        if (!GPS.IsLocationPermissionGranted())
        {
            //Request GPS permission
            GPS.RequestPermission();
        }
    }
    #endregion


    //Create a method for inspector to signout
    [ContextMenu("SignOut")]
    public void SignOut()
    {
        UGSManager.Instance.Authentication.Signout();
    }

}
#endregion
