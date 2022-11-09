namespace testrobot;

public class RDebuff
{
    public string Suffix;
    public string bonus_type;
    public double bonus_amount;

    public static string Translate(string s)
    {
        if(buffName.ContainsKey(s))
            return buffName[s];
        return s;
    }

    public override string ToString()
    {
        return $"{Translate(bonus_type)}{bonus_amount:P0}";
    }

    public static Dictionary<string, string> buffName = new Dictionary<string, string>()
    {
        { "ArmorDamage", "白条伤害" },
        { "BurstDamage", "爆卡伤害" },
        { "AllRaidDamage", "所有伤害" },
        { "RaidAttackDuration", "突击时长" },
        { "ChestDamage", "胸部伤害" },
        { "AfflictedDamage", "毒卡伤害" },
        { "BodyDamage", "蓝条伤害" },
        { "BurstDamagePerEnchant", "每部位爆卡伤害" },
        { "LimbDamage", "四肢伤害" },
        { "SupportEffect", "支持卡" },
        { "BurstChance", "爆卡几率" },
        { "AfflictedChance", "毒卡几率" },
        { "HeadDamage", "头部伤害" },
        { "AfflictedDuration", "毒卡时间" },
        { "AllLimbsHPMult", "四肢血量" },
        { "AllTorsoHPMult", "胸部血量" },
        { "AllHeadHPMult", "	头部血量" },
        { "ArmorArmsHPMult", "手部白条血量" },
        { "ArmorLegsHPMult", "腿部白条血量" },
        { "AllLegsHPMult", "	腿部血量" },
        { "ArmorTorsoHPMult", "胸部白条血量" },
        { "ArmorLimbsHPMult", "四肢白条血量" },
        { "AllArmorHPMult", "所有白条血量" },
        { "AllArmsHPMult", "	手部血量" },
        { "BurstDamagePerCurse", "爆卡伤害" },
        { "AfflictedDamagePerCurse", "毒卡伤害" },
    };
}