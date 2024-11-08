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
    CharacterCustomize,
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
    Host,
    Client,
    RaceResults,
    None
}
#endregion

#region Character

public enum CharacterPartType
{
    None,
    Head,
    Eyebrows,
    FacialHair,
    Hair,
    Hat,
    Hips,
    LeftHand,
    LeftLeg,
    LeftLowerArm,
    LeftUpperArm,
    Mask,
    RightHand,
    RightLeg,
    RightLowerArm,
    RightUpperArm,
    Torso
}
public enum CharacterGenderType
{
    Male,
    Female
}
public enum CharacterPartUIType
{
    FACE,
    HAIR,
    EYEBROWS,
    BEARD,
    HAT,
    MASK,
    TORSO,
    ARMS,
    HANDS,
    LEGS
}
#endregion