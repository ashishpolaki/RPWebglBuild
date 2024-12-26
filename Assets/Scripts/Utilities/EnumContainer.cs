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
public enum OutfitType
{
    None,
    Upper,
    Lower
}
public enum ScreenTabType : byte
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
    NotInRace,
    CharacterBodyCustomize,
    CharacterFaceCustomize,
    CharacterOutfitCustomize
}
public enum ScreenType : byte
{
    Login,
    Host,
    Client,
    RaceResults,
    Race,
    CharacterCustomisation,
    None
}
#endregion

#region Character
public enum CharacterBlendShapeType : byte
{
    None,
    BodyType,
    BodySize,
    Musculature
}
//Cloud Data dont change
public enum BlendPartType : byte
{
    Nose,
    Eyebrows,
    Hair,
    FacialHair,
    Eyes,
    Ears,
    Cheek,
    Mouth,
}

public enum SyntyCharacterPartType : byte
{
    None,
    Head = 1,
    Hair,
    EyebrowLeft,
    EyebrowRight,
    EyeLeft,
    EyeRight,
    EarLeft,
    EarRight,
    FacialHair,
    Torso,
    ArmUpperLeft,
    ArmUpperRight,
    ArmLowerLeft,
    ArmLowerRight,
    HandLeft,
    HandRight,
    Hips,
    LegLeft,
    LegRight,
    FootLeft,
    FootRight,
    Nose,
    Teeth,
    Tongue,
}
public enum CharacterPartUIType : byte
{
    Body_Shape,
    Body_SkinTone,
    Head_EyeBrows,
    Head_Ears,
    Head_Eyes,
    Head_FacialHair,
    Head_Hair,
    Head_Nose,
}
#endregion