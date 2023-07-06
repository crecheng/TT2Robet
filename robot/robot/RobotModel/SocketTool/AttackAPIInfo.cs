using testrobot;

namespace robot.SocketTool;

public class AttackAPIInfo
{
    public string clan_code;
    public long raid_id;
    public PlayerInfo player;
    public AttackLog attack_log;
    public RaidState raid_state;
}

public class AttackLog
{
    public string attack_datetime;
    public DateTime attackTime;
    public List<CardInfo> cards_level;
    public List<CardDamageInfo> cards_damage;
    
}

public class CardInfo
{
    public string id;
    public int value;
}

public class CardDamageInfo
{
    public int titan_index;
    public string id;
    public List<DamageInfo> damage_log;
}   

public class DamageInfo
{
    public string id;
    public int value;
}

public class PlayerInfo
{
    public int attacks_remaining;
    public string player_code;
    public string name;
    public int raid_level;
}

public class RaidState
{
    public int titan_index;
    public RaidCurrent current;

}

public class RaidCurrent
{
    public string enemy_id;
    public double current_hp;
    public List<TitanData.Part> parts;
}

public class CycleResetDate
{
    public string next_reset_at;
}

