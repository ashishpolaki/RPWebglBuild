namespace UI.Screen.Tab
{
    public class PlayerNotInRaceTab : BaseTab
    {
        protected override void OnTabBack()
        {
            UIController.Instance.ChangeCurrentScreenTab(ScreenTabType.VenueCheckIn);
        }
    }
}
