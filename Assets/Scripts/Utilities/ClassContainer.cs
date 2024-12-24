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
public class CharacterCustomisationEconomy : EconomyCustom
{
    public float bodyType;
    public float bodyGenderType;
    public float bodyMuscleType;
    public string skinToneColor = "";
    public List<CustomPartEconomy> customParts = new List<CustomPartEconomy>();
    public UpperOutfitEconomy upperOutfit = new UpperOutfitEconomy();
    public LowerOutfitEconomy lowerOutfit = new LowerOutfitEconomy();

    public CharacterCustomisationEconomy() : base()
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
    public int styleNumber; //Part Index
    public string color; //Part Color
    public List<BlendShapeEconomy> blendShapes; //Part BlendShapes

    public CustomPartEconomy()
    {
        styleNumber = -1;
        color = "";
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
public class UpperOutfitEconomy : EconomyCustom
{
    public int torso;
    public int rightUpperArm;
    public int rightLowerArm;
    public int rightHand;
    public int leftUpperArm;
    public int leftLowerArm;
    public int leftHand;

    public OutfitColorEconomy[] torsoColors;
    public OutfitColorEconomy[] upperArmColors;

    public UpperOutfitEconomy()
    {
        torsoColors = new OutfitColorEconomy[0];
        upperArmColors = new OutfitColorEconomy[0];

        torso = -1;
        rightUpperArm = -1;
        rightLowerArm = 0;
        rightHand = 0;
        leftUpperArm = -1;
        leftLowerArm = 0;
        leftHand = 0;
    }
}
[System.Serializable]
public class LowerOutfitEconomy : EconomyCustom
{
    public int hips;
    public int rightLeg;
    public int rightFoot;
    public int leftLeg;
    public int leftFoot;

    public OutfitColorEconomy[] hipsColors;
    public OutfitColorEconomy[] legColors;
    public OutfitColorEconomy[] footColors;

    public LowerOutfitEconomy()
    {
        hipsColors = new OutfitColorEconomy[0];
        legColors = new OutfitColorEconomy[0];
        footColors = new OutfitColorEconomy[0];

        hips = -1;
        rightLeg = -1;
        rightFoot = -1;
        leftLeg = -1;
        leftFoot = -1;
    }
}

[System.Serializable]
public class OutfitColorEconomy
{
    public int u;
    public int v;
    public string color;

    public OutfitColorEconomy()
    {
        u = -1;
        v = -1;
        color = "";
    }
}
#endregion