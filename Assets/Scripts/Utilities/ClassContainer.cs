using System.Collections.Generic;

public class ClassContainer
{
    
}

#region Save Race Stats
[System.Serializable]
public class SaveRaceStats
{
    public RaceStats[] raceStats;
}
[System.Serializable]
public class RaceStats
{
    public string raceIdentifier;
    public int predeterminedWinner;
    public Waypoint[] waypoints;
    public HorseData[] horsesData;
}
#endregion

#region Character
public class EconomyCustom
{
    public EconomyCustom()
    {
    }
}

[System.Serializable]
public class FullBodyEconomy : EconomyCustom
{
    public float bodyType;
    public float bodyGenderType;
    public float bodyMuscleType;
    public string skinToneColor;
    public List<CustomPartEconomy> customParts;
    public UpperOutfitEconomy upperOutfit;
    public LowerOutfitEconomy lowerOutfit;

    public FullBodyEconomy() : base()
    {
        bodyType = 0;
        skinToneColor = "";
        customParts = new List<CustomPartEconomy>();
        upperOutfit = new UpperOutfitEconomy();
        lowerOutfit = new LowerOutfitEconomy();
    }
}

[System.Serializable]
public class CustomPartEconomy
{
    public int type; //(int)BlendPartType Enum
    public int styleNumber;
    public string color;
    public List<BlendShapeEconomy> blendShapes;

    public CustomPartEconomy()
    {
        styleNumber = 0;
        blendShapes = new List<BlendShapeEconomy>();
    }
}
[System.Serializable]
public class BlendShapeEconomy
{
    public string name;
    public float value;

    public BlendShapeEconomy()
    {
        name = "";
        value = 0;
    }
}

[System.Serializable]
public class UpperOutfitEconomy
{
    public int torso;
    public int rightUpperArm;
    public int rightLowerArm;
    public int rightHand;
    public int leftUpperArm;
    public int leftLowerArm;
    public int leftHand;
}
[System.Serializable]
public class LowerOutfitEconomy
{
    public int hips;
    public int rightLeg;
    public int rightFoot;
    public int leftLeg;
    public int leftFoot;
}
#endregion