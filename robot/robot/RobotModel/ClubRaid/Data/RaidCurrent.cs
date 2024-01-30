namespace testrobot;


public class Bonus
{
    public double BonusAmount ;
    public string BonusType ;
    public string Suffix ;
}

public class AttackShowInfo
{
    public string name ;
    public int num_attacks ;
    public string player_code ;
    public string role ;
    public long score ;
    public int tie_breaker ;
}
public class TitanLords
{
    public TitanData[] blueprints ;
    public TitanData current ;
    public int currentIndex ;
    public string[] sequence ;
    public Dictionary<string,string> target_states ;
}
