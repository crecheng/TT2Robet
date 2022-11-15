using ConfigData;
using testrobot;

public partial class RaidCal
{

    #region 支持卡

    public static float TeamTactics(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.AllAdd += data.BonusAValue[level - 1];
        return data.BonusAValue[level - 1];
    }

    public static float InnerTruth(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.HeadAdd += data.BonusAValue[level - 1];
        calData.CardAdd.ChestAdd += data.BonusBValue[level - 1];
        calData.CardAdd.AfflictedChanceAdd *= data.BonusCValue;
        return data.BonusAValue[level - 1];
    }

    public static float ImpactAttack(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.BodyAdd += data.BonusAValue[level - 1];
        return data.BonusAValue[level - 1];
    }

    public static float LimbSupport(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.LegAdd += data.BonusAValue[level - 1];
        return data.BonusAValue[level - 1];
    }


    public static float BurstBoost(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.BurstAdd += data.BonusAValue[level - 1];
        calData.CardAdd.BurstChanceAdd *= data.BonusCValue;
        return data.BonusAValue[level - 1];
    }

    public static float MentalFocus(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.AfflictedAdd += data.BonusAValue[level - 1];
        calData.CardAdd.AfflictedChanceAdd *= data.BonusCValue;
        return data.BonusAValue[level - 1];
    }

    public static float ExecutionersAxe(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.HeadAdd += data.BonusAValue[level - 1];
        calData.CardAdd.ChestAdd += data.BonusBValue[level - 1];
        calData.CardAdd.BurstChanceAdd *= data.BonusCValue;
        return data.BonusAValue[level - 1];
    }

    public static float SuperheatMetal(CardData data, int level, RaidCalData calData)
    {
        calData.CardAdd.ArmorAdd += data.BonusAValue[level - 1];
        return data.BonusAValue[level - 1];
    }

    public static float CrushingVoid(CardData data, int level, RaidCalData calData)
    {
        var d = data.BonusAValue[level - 1];
        int c = (int)data.BonusCValue;
        d+= data.BonusBValue[level - 1] * Math.Min(c, calData.Cal.currentBlue);
        calData.CardAdd.AllAdd+=d;
        return d;
    }

    public static float FinisherAttack(CardData data, int level, RaidCalData calData)
    {
        var d = data.BonusAValue[level - 1];
        int c = (int)data.BonusDValue;
        d+= data.BonusBValue[level - 1] * Math.Min(c, calData.Cal.currentBone);
        calData.CardAdd.AllAdd+=d;
        return d;
    }

    public static float SpinalTap(CardData data, int level, RaidCalData calData)
    {
        var d = data.BonusAValue[level - 1];
        if (calData.Cal.currentBone > 0)
            d += data.BonusBValue[level - 1];
        return d;
    }

    public static float TotemFairySkill(CardData data, int level, RaidCalData calData)
    {
        if (!calData.Cal.timeCal.ContainsKey("TotemFairySkillS"))
        {
            calData.Cal.timeCal.Add("TotemFairySkillS", new List<float>());
            calData.Cal.timeCal.Add("TotemFairySkill", new List<float>());
            calData.Cal.otherData.Add("TotemFairySkillLast", 0);
            calData.Cal.otherData.Add("TotemFairySkillCount", 0);
        }


        //仙女生成
        if (calData.Cal.CalData.Time - calData.Cal.otherData["TotemFairySkillLast"] >= data.BonusDValue)
        {
            var c = calData.Cal.otherData["TotemFairySkillCount"];
            c += 1;
            calData.Cal.otherData["TotemFairySkillCount"] += 1;
            calData.Cal.otherData["TotemFairySkillLast"] = c * data.BonusDValue;
            //生成后随机生效时间
            calData.Cal.timeCal["TotemFairySkillS"].Add(_random.NextSingle() * 2f + 1f);
        }


        //计算生成后是否生效
        calData.UpdateTime("TotemFairySkillS", () => calData.Cal.timeCal["TotemFairySkill"].Add(data.BonusCValue));
        //当前层数
        int stack = calData.UpdateTime("TotemFairySkill");
        var d = stack * data.BonusAValue[level - 1];
        calData.CardAdd.AllAdd += d;
        return d;
    }


    #endregion

    #region 爆发卡

    public static double BrustBese(CardData data, int level, RaidCalData calData)
    {
        calData.Cal.otherData["BurstCount"] += 1;
        double d = calData.BaseDmg * data.BonusAValue[level-1];
        d *= calData.RaidAdd.GetAdd(calData.CurrentPart, data.Category);
        d *= calData.CurrentPart.GetCardAdd(calData.CardAdd, data.Category);
        return d;
    }
    public static double MoonBeam(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BrustBese(data, level, calData);
        
        var part = calData.CurrentPart;
        if (part.PartId == CalPart.PartName.ChestUpper)
            d *= data.BonusCValue;
        part.CurrentHp -= d;
        //Console.WriteLine("MoonBeam-"+d);
        return d;
    }
    
    public static double BurstCount(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BrustBese(data, level, calData);
        d *= 1 + calData.Cal.otherData["BurstCountLast"] * 0.04;
        return d;
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
        var d = BrustBese(data, level, calData);
        
        var part = calData.CurrentPart;
        if (part.CurrentType == 2)
            d *= data.BonusCValue;
        if (part.enchanted)
            d *= data.BonusBValue[level-1];
        part.CurrentHp -= d;
       // Console.WriteLine("LimbBurst-" + d);
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
        var d = BrustBese(data, level, calData);
        part.CurrentHp -= d;
        //Console.WriteLine("Haymaker-" + d);
        return d;
    }

    public static double LimbBurst(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BrustBese(data, level, calData);
        var part = calData.CurrentPart;
        if (part.PartId != CalPart.PartName.ChestUpper && part.PartId != CalPart.PartName.Head)
            d *= data.BonusCValue;
        part.CurrentHp -= d;
        //Console.WriteLine("LimbBurst-" + d);
        return d;
    }

    public static double MirrorForce(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    
    public static double Purify(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BrustBese(data, level, calData);
        var stack = calData.CurrentPart.GetDotCount();
        d *= (1 + stack * data.BonusCValue);
        calData.CurrentPart.ClearDot();
        return d;
    }
    
    public static double RazorWind(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        var d = BrustBese(data, level, calData);
        
        var part = calData.CurrentPart;
        if (part.CurrentType == 1)
            d *= data.BonusCValue;
        part.CurrentHp -= d;
        //Console.WriteLine("RazorWind-"+d);
        return d;
    }
    
    public static double SkullBash(CardData data, int level,RaidCalData calData)
    {
        if (!Probability(data.Chance*calData.RaidAdd.BurstChanceAdd*calData.CardAdd.BurstChanceAdd))
            return 0;
        
        var d = BrustBese(data, level, calData);
        
        var part = calData.CurrentPart;
        if (part.PartId==CalPart.PartName.Head )
            d *= data.BonusCValue;
        part.CurrentHp -= d;
        //Console.WriteLine("SkullBash-"+d);
        return d;
    }
    
    public static double WhipOfLightning(CardData data, int level,RaidCalData calData)
    {
        return 0;
    }
    

    #endregion

    #region 毒卡

    public static double DotBase(CardData data, int level,RaidCalData calData,Func<CalPart,
        double,double> onCal=null,Action<CalPart> onAddDot=null)
    {
        if (Probability(data.Chance * calData.RaidAdd.AfflictedChanceAdd * calData.CardAdd.AfflictedChanceAdd))
        {
            foreach (var p in calData.Parts)
            {
                p.UpdateDot(data.ID, 0.05f);
            } 
            calData.CurrentPart.AddDot(data.ID, data.Duration, data.MaxStacks);
            onAddDot?.Invoke(calData.CurrentPart);
        }

        double d = 0;
        if (calData.Tap % 4 == 0)
        {
            double ad = 0;
            foreach (var p in calData.Parts)
            {
                var stack= p.GetDotCount(data.ID);
                if(stack==0)
                    continue;
                double pd= calData.BaseDmg / 5f
                           * data.BonusAValue[level - 1]
                           * stack
                           * calData.RaidAdd.GetAdd(p, data.Category)
                           * p.GetCardAdd(calData.CardAdd, data.Category);
                if (onCal != null)
                {
                    pd = onCal.Invoke(p, pd);
                }

                ad += pd;
            }

            d = ad;
        }
        
        return d;
    }

    
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
        var d=DotBase(data, level, calData, null, (part) =>
        {
            var list = part.DotDic[data.ID];
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = data.Duration;
            }
        });
        return d;
    }
    
    public static double RuinousRust(CardData data, int level,RaidCalData calData)
    {
        var d= DotBase(data, level, calData, (p, dmg) =>
        {
            if (p.enchanted)
                dmg *= data.BonusCValue;
            return dmg;
        });
        return d;
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