
using ConfigData;
using testrobot;

public partial class RaidCal
{
    public static Dictionary<string, string> CardDmageType = new Dictionary<string, string>()
    {
        {"AfflictedChanceSupport", "毒系几率加成"},
        {"AfflictedDamageSupport", "毒系伤害加成"},
        {"AllRaidDamageSupport", "全部伤害加成"},
        {"ArmorDamageSupport", "白条伤害加成"},
        {"BodyDamageSupport", "蓝条伤害加成"},
        {"BurningDamageRate", "火海伤害"},
        {"BurningPartChance", "触发火海提供的毒系几率加成"},
        {"BurstChanceSupport", "爆发几率加成"},
        {"BurstCountBonus", "爆发次数奖励"},
        {"BurstCountDamage", "爆发次数伤害"},
        {"BurstDamageSupport", "爆发伤害加成"},
        {"CelestialStaticBurst", "天体静态爆发"},
        {"CelestialStaticChargePerBurst", "天体静态爆发时消耗"},
        {"CelestialStaticChargePerTap", "天体静态每次点击时加分"},
        {"CelestialStaticMaxCharges", "天体静态最大积分数"},
        {"ChainLightningBurst", "红链伤害"},
        {"ChainLightningMaxTargets", "红链最高支持部位"},
        {"ChestDamageSupport", "胸部伤害加成"},
        {"DecayDamageExpo", "腐败伤害"},
        {"DecayHealthCap", "腐败血量限制"},
        {"DecayPercentRate", "腐败百分比增伤"},
        {"DiseaseBonusRate", "放射性随时间提高的伤害"},
        {"DiseaseDamageRate", "放射性伤害"},
        {"ExposedBodyBoostSupport", "暴露的蓝条数量加成"},
        {"ExposedBodyBoostSupportMaxParts", "暴露的蓝条数量加成上限"},
        {"ExposedSkeletonBoostSupport", "暴露的骨架数量加成"},
        {"ExposedSkeletonBoostSupportMaxParts", "暴露的骨架数量加成上限"},
        {"FairyTotemDamage", "仙女加成"},
        {"FairyTotemDuration", "仙女持续时间"},
        {"FairyTotemRandomChance", "仙女出现几率"},
        {"FairyTotemRate", "仙女飞行速度"},
        {"FlakBurst", "防空伤害"},
        {"FragmentizeArmorMult", "破甲对白条额外加成"},
        {"FragmentizeBurst", "破甲伤害"},
        {"FragmentizeEnchantedArmorMult", "破甲对诅咒额外加成"},
        {"FuseExplosionMult", "核融触发的等待时间"},
        {"FuseRepeatChance", "核融触发几率"},
        {"HaymakerBurst", "激光伤害"},
        {"HaymakerTapsNeeded", "每次触发需要的点击数"},
        {"HeadDamageSupport", "头部伤害加成"},
        {"LimbBurstDamage", "四肢爆发伤害"},
        {"LimbBurstMult", "四肢部位额外加成数"},
        {"LimbDamageSupport", "四肢伤害加成"},
        {"MirrorForceBoost", "镜像的队友加成"},
        {"MirrorForceBoostMax", "镜像队友加成上限"},
        {"MirrorForceBurst", "镜像伤害"},
        {"MoonBeamBurst", "月光伤害"},
        {"MoonBeamChestMult", "月光对胸部的额外加成"},
        {"PlagueAttackDamageMult", "瘟疫额外毒系部位的伤害加成"},
        {"PlagueAttackDamageRate", "瘟疫基础伤害"},
        {"PoisonDamageRate", "酸雨伤害"},
        {"PurifyBonus", "净化清毒加成"},
        {"PurifyDamage", "净化伤害"},
        {"RazorWindBodyMult", "风刃蓝条额外加成"},
        {"RazorWindBurst", "风刃伤害"},
        {"RicochetDamageMult", "防空蓝条加成（疑似）"},
        {"RuinousRustDamageRate", "紫雨伤害"},
        {"RuinousRustEnchantedArmorMult", "紫雨对诅咒时的额外加成"},
        {"RuneDamageBoost", "蓝球加成"},
        {"RuneDamageRate", "蓝球伤害"},
        {"ShadowDamageMult", "影子叠加加成"},
        {"ShadowDamageRate", "影子伤害"},
        {"SkeletonExposedArmorDamageSupport", "骨骼对二段加成"},
        {"SkullBashBurst", "头骨伤害"},
        {"SkullBashHeadMult", "头骨对头部额外加成"},
        {"SwarmDamageRate", "蜂巢伤害"},
        {"TeamTacticsClanMoraleBoost", "团队队友加成"},
        {"WhipOfLightningBurst", "电鞭伤害"},
        {"WhipOfLightningChance", "电鞭触发几率"},
        {"WhipOfLightningChanceCap", "电鞭触发几率上限"},
    };

    public CalPart TargetPart;

    public class RaidCalData
    {
        public float Time;
        public int Tap;
        public int BaseDmg;
        public RaidAdd RaidAdd;
        public List<CalPart> Parts;
        public CalPart CurrentPart;
        public CardAdd CardAdd;
        public RaidCal Cal;

        public int UpdateTime(string key,Action onTimeOut=null)
        {
            if(!Cal.timeCal.ContainsKey(key))
                return 0;
            var listS = Cal.timeCal[key];
            for (var i = listS.Count - 1; i >= 0; i--)
            {
                var time = listS[i];
                time -= 0.05f;
                if (time <= 0)
                {
                    onTimeOut?.Invoke();
                    listS.RemoveAt(i);
                }
                else
                {
                    listS[i] = time;
                }
            }

            return listS.Count;
        }

        public void Reset()
        {
            Time = 0;
            Tap = 0;
            Parts.ForEach(i=>i.Reset());
        }
    }
    private static Dictionary<string, Func<CardData, int,RaidCalData ,double>> BurstFun =
        new Dictionary<string, Func<CardData, int, RaidCalData,double>>()
        {
            {"MoonBeam",MoonBeam},
            {"BurstCount",BurstCount},
            {"ChainLightning",ChainLightning},
            {"FlakShot",FlakShot},
            {"Fragmentize",Fragmentize},
            {"Haymaker",Haymaker},
            {"LimbBurst",LimbBurst},
            {"Purify",Purify},
            {"RazorWind",RazorWind},
            {"SkullBash",SkullBash},
            {"WhipOfLightning",WhipOfLightning},
            {"BurningAttack",BurningAttack},
            {"DecayingAttack",DecayingAttack},
            {"Disease",Disease},
            {"Fuse",Fuse},
            {"PlagueAttack",PlagueAttack},
            {"PoisonAttack",PoisonAttack},
            {"RuinousRust",RuinousRust},
            {"RuneAttack",RuneAttack},
            {"Shadow",Shadow},
            {"Swarm",Swarm},
            {"CelestialStatic",CelestialStatic},
            {"MirrorForce",MirrorForce},
        };

    private static Dictionary<string, Func<CardData, int, RaidCalData,float>> SupportFun =
        new Dictionary<string, Func<CardData, int, RaidCalData,float>>()
        {
            {"TeamTactics",TeamTactics},
            {"BurstBoost",BurstBoost},
            {"CrushingVoid",CrushingVoid},
            {"ExecutionersAxe",ExecutionersAxe},
            {"FinisherAttack",FinisherAttack},
            {"ImpactAttack",ImpactAttack},
            {"InnerTruth",InnerTruth},
            {"LimbSupport",LimbSupport},
            {"MentalFocus",MentalFocus},
            {"SpinalTap",SpinalTap},
            {"SuperheatMetal",SuperheatMetal},
            {"TotemFairySkill",TotemFairySkill},

        };

    private static Dictionary<string, Func<CardData, int, RaidCalData, float>> UpdateSupportFun =
        new Dictionary<string, Func<CardData, int, RaidCalData, float>>()
        {
            {"TotemFairySkill",TotemFairySkill},
        };

    private Dictionary<string, List<float>> timeCal = new Dictionary<string, List<float>>();
    private Dictionary<string, float> otherData = new Dictionary<string, float>();
    private RaidCalData CalData = new RaidCalData();
    private int currentBlue;
    private int currentWhite;
    private int currentBone;
    public RaidDmgData Cal(Dictionary<string,int>card, int level,ConfigDataManage dataManage,List<CalPart>parts,RaidAdd raidAdd)
    {
        int baseDmg = level + 100;
        double tapDmg = baseDmg * 600;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
        Dictionary<string, CardData> cardSupportData = new Dictionary<string, CardData>();
        Dictionary<string, CardData> updateCardSupportData = new Dictionary<string, CardData>();
        List<CardData> cardDmgData = new List<CardData>();
        Dictionary<string, RaidDmgCardDate> raidDmgCardDate = new Dictionary<string, RaidDmgCardDate>();
        bool haveUpdateSupport = false;
        foreach (var (key, value) in card)
        {
            var cd = dataManage.GetCardDataDataFirst(key);
            var n = new RaidDmgCardDate()
            {
                ID = key,
                Level = value,
                CardType = cd.Category
            };
            
            if (cd.Category == "Support")
            {
                if (key == "TotemFairySkill")
                {
                    haveUpdateSupport = true;
                    updateCardSupportData.Add(key,cd);
                    n.UpdateAdd = true;
                }
                else
                {
                    cardSupportData.Add(key,cd);
                }
            }
            else if(key=="RuneAttack")
                cardDmgData.Insert(0,cd);
            else
                cardDmgData.Add(cd);
            raidDmgCardDate.Add(key,n);
        }
        otherData.Clear();
        timeCal.Clear();
        
        OtherDataInit();
        
        CardAdd cardAdd = new CardAdd();
        double allDmg = 0;
        cardAdd.Reset();
        CalData.Cal = this;
        CalData.Parts = parts;
        CalData.BaseDmg = baseDmg;
        CalData.CardAdd = cardAdd;
        CalData.RaidAdd = raidAdd;
        CalData.Reset();
        CalData.CurrentPart = FindPart(parts);
        
        
        foreach (var (key, value) in cardSupportData)
        {
            var a= SupportFun[key].Invoke(value, card[key], CalData);
            raidDmgCardDate[key].Add = a;
        }

        cardAdd.Save();
        
        for (int i = 0; i < 600; i++)
        {
            cardAdd.Load();
            CalData.Time += 0.05f;
            CalData.Tap += 1;
            CalData.CurrentPart = FindPart(parts);
            if (i  %10==0)
            {
                otherData["BurstCountLast"] = otherData["BurstCount"];
            }
            if (haveUpdateSupport)
            {
                foreach (var (key, value) in updateCardSupportData)
                {
                    var a= UpdateSupportFun[key].Invoke(value, card[key], CalData);
                    raidDmgCardDate[key].UpdateAllAdd += a;
                    raidDmgCardDate[key].UpdateCount+=1;
                }
            }
            
            allDmg += GetTapDmg(baseDmg, raidAdd, cardAdd, CalData.CurrentPart);

            foreach (var value in cardDmgData)
            {
                var d = BurstFun[value.ID].Invoke(value, card[value.ID], CalData);
                if (d > 0)
                {
                    raidDmgCardDate[value.ID].Count++;
                    raidDmgCardDate[value.ID].Dmg += d;
                }
                allDmg += d;
            }
        }
        
        return new RaidDmgData()
        {
            CardDate = new List<RaidDmgCardDate>(raidDmgCardDate.Values),
            Dmg = allDmg,
            RaidLevel = level
        };
    }

    public void OtherDataInit()
    {
        otherData.Add("BurstCount",0);
        otherData.Add("BurstCountLast",0);
    }

    public double GetTapDmg(int baseDmg, RaidAdd raidAdd, CardAdd cardAdd,CalPart part)
    {
        double hAdd = part.CurrentType == 2 ? raidAdd.ArmorAdd : raidAdd.BodyAdd;
        return baseDmg * (1+part.partAdd) * raidAdd.AllAdd *hAdd * part.GetCardAdd(cardAdd);
    }

    private Dictionary<int, int> partCalDic = new Dictionary<int, int>();
    public CalPart FindPart(List<CalPart> parts)
    {
        if (parts.Count <= 1)
        {
            return TargetPart;
        }

        currentWhite = 0;
        currentBlue = 0;
        currentBone = 0;
        for (int i = 0; i < 8; i++)
        {
            
            partCalDic[i] = 0;
        }
        
        foreach (var part in parts)
        {
            int i = part.CurrentType;
            if(i==2)
                currentWhite++;
            else if(i==1)
                currentBlue++;
            else
                currentBone++;
        }

        return TargetPart;
    }

    private static Random _random = new Random();
    
    public class RaidAdd
    {
        public float BurstAdd = 1;
        public float BurstChanceAdd = 1;
        public float AfflictedAdd = 1;
        public float AfflictedChanceAdd = 1;
        public float AfflictedTime = 1;
        public float ArmorAdd = 1;
        public float BodyAdd = 1;
        public float HeadAdd=1 ;
        public float ChestAdd =1;
        public float LegAdd =1;
        public float AllAdd=1;

        public float GetAdd(CalPart part, string cardType = "")
        {
            float d = 1;
            d *= AllAdd;
            switch (cardType)
            {
                case "Burst": d *= BurstAdd; break;
                case "Affliction":
                case "Afflicted": d *= AfflictedAdd; break;
            }
            if (part.CurrentType == 2)
                d *= ArmorAdd;
            else
                d *= BodyAdd;
            if (part.PartId == CalPart.PartName.Head )
            {
                d *= HeadAdd;
            }
            else if (part.PartId == CalPart.PartName.ChestUpper )
            {
                d *= ChestAdd;
            }
            else
            {
                d *= LegAdd;
            }

            return d;
        }
    }
    
    public class CardAdd
    {
        public float BurstAdd ;
        public float BurstChanceAdd =1;
        public float AfflictedAdd ;
        public float AfflictedChanceAdd = 1;
        public float AllAdd ;
        public float BodyAdd ;
        public float ArmorAdd ;
        public float HeadAdd ;
        public float ChestAdd ;
        public float LegAdd ;
        public CardAdd save ;
        public void Save()
        {
            save ??= new CardAdd();
            save.BurstAdd = BurstAdd;
            save.BurstChanceAdd = BurstChanceAdd;
            save.AfflictedAdd = AfflictedAdd;
            save.AfflictedChanceAdd = AfflictedChanceAdd;
            save.AllAdd = AllAdd;
            save.BodyAdd = BodyAdd;
            save.ArmorAdd = ArmorAdd;
            save.HeadAdd = HeadAdd;
            save.ChestAdd = ChestAdd;
            save.LegAdd = LegAdd;
        }

        public void Load()
        {
            BurstAdd = save.BurstAdd;
            BurstChanceAdd = save.BurstChanceAdd;
            AfflictedAdd = save.AfflictedAdd;
            AfflictedChanceAdd = save.AfflictedChanceAdd;
            AllAdd = save.AllAdd;
            BodyAdd = save.BodyAdd;
            ArmorAdd = save.ArmorAdd;
            HeadAdd = save.HeadAdd;
            ChestAdd = save.ChestAdd;
            LegAdd = save.LegAdd;
        }

        public void Reset(bool flag=false)
        {
            BurstAdd = 0;
            BurstChanceAdd = 1;
            AfflictedAdd = 0;
            AfflictedChanceAdd = 1;
            AllAdd = 0;
            BodyAdd = 0;
            ArmorAdd = 0;
            HeadAdd = 0;
            ChestAdd = 0;
            LegAdd = 0;
            if(flag)
                save.Reset();
        }
    }
    
    public static bool Probability(float p) => _random.Next(100) < (int) (p * 100);

    
    

    
    
}