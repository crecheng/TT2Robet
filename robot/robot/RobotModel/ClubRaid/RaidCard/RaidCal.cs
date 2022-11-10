
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
        public int BaseDmg;
        public RaidAdd RaidAdd;
        public List<CalPart> Parts;
        public CalPart CurrentPart;
        public CardAdd CardAdd;
        public RaidCal Cal;
    }
    private static Dictionary<string, Func<CardData, int,RaidCalData ,double>> BurstFun =
        new Dictionary<string, Func<CardData, int, RaidCalData,double>>()
        {
            {"MoonBeam",MoonBeam},
            {"BurstCount",BurstCount},
            //{"CelestialStatic",CelestialStatic},
            {"ChainLightning",ChainLightning},
            {"FlakShot",FlakShot},
            {"Fragmentize",Fragmentize},
            {"Haymaker",Haymaker},
            {"LimbBurst",LimbBurst},
            {"MirrorForce",MirrorForce},
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
        };

    private static Dictionary<string, Action<CardData, int, List<CalPart>, CardAdd>> SupportFun =
        new Dictionary<string, Action<CardData, int, List<CalPart>, CardAdd>>()
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

    private Dictionary<string, int> otherData = new Dictionary<string, int>();
    private RaidCalData CalData = new RaidCalData();
    public double Cal(Dictionary<string,int>card, int level,ConfigDataManage dataManage,List<CalPart>parts,RaidAdd raidAdd)
    {
        int baseDmg = level + 100;
        double tapDmg = baseDmg * 600;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
        Dictionary<string, CardData> cardSupportData = new Dictionary<string, CardData>();
        Dictionary<string, CardData> cardDmgData = new Dictionary<string, CardData>();
        foreach (var (key, value) in card)
        {
            var cd = dataManage.GetCardDataDataFirst(key);
            if (cd.Category == "Support")
                cardSupportData.Add(key,cd);
            else 
                cardDmgData.Add(key,cd);
        }
        otherData.Clear();
        CardAdd cardAdd = new CardAdd();
        double allDmg = 0;
        cardAdd.Reset();
        foreach (var (key, value) in cardSupportData)
        {
            SupportFun[key].Invoke(value, card[key], parts, cardAdd);
        }

        CalData.Cal = this;
        CalData.Parts = parts;
        CalData.BaseDmg = baseDmg;
        CalData.CardAdd = cardAdd;
        CalData.RaidAdd = raidAdd;

        for (int i = 0; i < 600; i++)
        {
            CalData.CurrentPart = FindPart(parts);
            
            allDmg += GetTapDmg(baseDmg, raidAdd, cardAdd, CalData.CurrentPart);

            foreach (var (key, value) in cardDmgData)
            {
                var d = BurstFun[key].Invoke(value, card[key], CalData);
                allDmg += d;
            }
        }
        
        return allDmg;
    }

    public double GetTapDmg(int baseDmg, RaidAdd raidAdd, CardAdd cardAdd,CalPart part)
    {
        double hAdd = ((int) part.part_id % 2 == 1 ? raidAdd.ArmorAdd : raidAdd.BodyAdd);
        return baseDmg * (1+part.partAdd) * raidAdd.AllAdd *hAdd * part.GetCardAdd(cardAdd);
    }

    public CalPart FindPart(List<CalPart> parts)
    {
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
                case "Afflicted": d *= AfflictedAdd; break;
            }
            if ((int)part.part_id % 2 == 1)
                d *= ArmorAdd;
            else
                d *= BodyAdd;
            if (part.part_id == TitanData.PartName.ArmorHead || part.part_id == TitanData.PartName.BodyHead)
            {
                d *= HeadAdd;
            }
            else if (part.part_id == TitanData.PartName.ArmorChestUpper || part.part_id == TitanData.PartName.BodyChestUpper)
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

        public void Reset()
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
        }
    }
    
    public static bool Probability(float p) => _random.Next(100) < (int) (p * 100);

    public class CalPart
    {
        public double current_hp;
        public bool enchanted;
        public TitanData.PartName part_id;
        public double total_hp;
        public double partAdd;

        public float GetCardAdd(CardAdd cardAdd,string cardType="")
        {
            var d = 1f;
            d += cardAdd.AllAdd;
            switch (cardType)
            {
                case "Burst": d += cardAdd.BurstAdd; break;
                case "Afflicted": d += cardAdd.AfflictedAdd; break;
            }

            if ((int) part_id % 2 == 1)
                d += cardAdd.ArmorAdd;
            else
                d += cardAdd.BodyAdd;

            if (part_id == TitanData.PartName.ArmorHead || part_id == TitanData.PartName.BodyHead)
            {
                d += cardAdd.HeadAdd;
            }
            else if (part_id == TitanData.PartName.ArmorChestUpper || part_id == TitanData.PartName.BodyChestUpper)
            {
                d += cardAdd.ChestAdd;
            }
            else
            {
                d += cardAdd.LegAdd;
            }

            return d;
        }
    }
    

    
    
}