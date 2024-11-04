using UnityEngine;

public class EnumContainer
{
    
}

#region Particle
public enum ParticleType
{
    None,
    HorseDustCloud
}
#endregion

#region Camera
public enum CameraType
{
    None,
    Overtake,
    RiderCloseUp
}
public enum CameraMode
{
    Basic,
    Special
}
#endregion

#region Sound
public enum SoundType
{
    RaceStart,
    Cheer,
    HorseGallop,
    RaceMusic,
    BeforeRaceStart,
    None
}
#endregion

#region UI
public enum ScreenTabType
{
    None,
    LoginPlayer,
    RegisterPlayer,
    RaceSchedule,
    PlayerName,
    RoleSelection,
    Lobby,
    RegisterVenue,
    RaceInProgress,
    RaceResults,
    Welcome,
    SetVenueName,
    HostSetting,
    VenueCheckIn,
    RaceCheckIn,
    RaceTimer,
    NotInRace
}
public enum ScreenType
{
    Login,
    CharacterCustomization,
    Host,
    Client,
    RaceResults,
    None
}
#endregion