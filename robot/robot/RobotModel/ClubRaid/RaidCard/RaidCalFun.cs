using ConfigData;
using testrobot;

public partial class RaidCal
{

    #region 支持卡

    public static void TeamTactics(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.AllAdd += data.BonusAValue[level - 1];
    }

    public static void InnerTruth(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.HeadAdd += data.BonusAValue[level - 1];
        cardAdd.ChestAdd += data.BonusBValue[level - 1];
        cardAdd.AfflictedChanceAdd *= data.BonusCValue;
    }

    public static void ImpactAttack(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.BodyAdd += data.BonusAValue[level - 1];
    }

    public static void LimbSupport(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.LegAdd += data.BonusAValue[level - 1];
    }


    public static void BurstBoost(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.BurstAdd += data.BonusAValue[level - 1];
        cardAdd.BurstChanceAdd *= data.BonusCValue;
    }

    public static void MentalFocus(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.AfflictedAdd += data.BonusAValue[level - 1];
        cardAdd.AfflictedChanceAdd *= data.BonusCValue;
    }

    public static void ExecutionersAxe(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.HeadAdd += data.BonusAValue[level - 1];
        cardAdd.ChestAdd += data.BonusBValue[level - 1];
        cardAdd.BurstChanceAdd *= data.BonusCValue;
    }

    public static void SuperheatMetal(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {
        cardAdd.ArmorAdd += data.BonusAValue[level - 1];
    }

    public static void CrushingVoid(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {

    }

    public static void FinisherAttack(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {

    }

    public static void SpinalTap(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {

    }

    public static void TotemFairySkill(CardData data, int level, List<CalPart> parts, CardAdd cardAdd)
    {

    }


    #endregion

    #region 爆发卡

    public static double BeseBrust(CardData data, int level, RaidCalData calData)
    {
        double d = calData.BaseDmg * data.BonusAValue[level-1];
        d *= calData.RaidAdd.GetAdd(calData.CurrentPart, data.Category);
        d *= calData.CurrentPart.GetCardAdd(calData.CardAdd, data.Category);
        return d;
    }
    public static double MoonBeam(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BeseBrust(data, level, calData);
        
        var part = calData.CurrentPart;
        if (part.part_id == TitanData.PartName.ArmorChestUpper || part.part_id == TitanData.PartName.BodyChestUpper)
            d *= data.BonusCValue;
        part.current_hp -= d;
        Console.WriteLine("MoonBeam-"+d);
        return d;
    }
    
    public static double BurstCount(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double ChainLightning(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double FlakShot(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Fragmentize(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BeseBrust(data, level, calData);
        
        var part = calData.CurrentPart;
        if ((int) part.part_id % 2 == 1)
            d *= data.BonusCValue;
        if (part.enchanted)
            d *= data.BonusBValue[level-1];
        part.current_hp -= d;
        Console.WriteLine("LimbBurst-" + d);
        return d;
    }
    
    public static double Haymaker(CardData data, int level,RaidCalData calData)
    {
        if (calData.Cal.otherData.ContainsKey("Haymaker"))
        {
            calData.Cal.otherData["Haymaker"] += 1;
        }
        else
        {
            calData.Cal.otherData["Haymaker"] = 1;
            return 0;
        }

        if (calData.Cal.otherData["Haymaker"] < 70)
            return 0;
        var part = calData.CurrentPart;
        calData.Cal.otherData["Haymaker"] = 0;
        var d = BeseBrust(data, level, calData);
        part.current_hp -= d;
        Console.WriteLine("Haymaker-" + d);
        return d;
    }

    public static double LimbBurst(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BeseBrust(data, level, calData);
        var part = calData.CurrentPart;
        if (part.part_id != TitanData.PartName.ArmorChestUpper &&
            part.part_id != TitanData.PartName.BodyChestUpper &&
            part.part_id != TitanData.PartName.ArmorHead &&
            part.part_id != TitanData.PartName.BodyHead)
            d *= data.BonusCValue;
        part.current_hp -= d;
        Console.WriteLine("LimbBurst-" + d);
        return d;
    }

    public static double MirrorForce(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Purify(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double RazorWind(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BeseBrust(data, level, calData);
        
        var part = calData.CurrentPart;
        if ((int) part.part_id % 2 == 0)
            d *= data.BonusCValue;
        part.current_hp -= d;
        Console.WriteLine("RazorWind-"+d);
        return d;
    }
    
    public static double SkullBash(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        
        var d = BeseBrust(data, level, calData);
        
        var part = calData.CurrentPart;
        if (part.part_id==TitanData.PartName.ArmorHead || part.part_id==TitanData.PartName.BodyHead)
            d *= data.BonusCValue;
        part.current_hp -= d;
        Console.WriteLine("SkullBash-"+d);
        return 0;
    }
    
    public static double WhipOfLightning(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    

    #endregion

    #region 毒卡

    public static double BurningAttack(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double DecayingAttack(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Disease(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Fuse(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double PlagueAttack(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double PoisonAttack(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double RuinousRust(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double RuneAttack(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Shadow(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Swarm(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    

    #endregion

}