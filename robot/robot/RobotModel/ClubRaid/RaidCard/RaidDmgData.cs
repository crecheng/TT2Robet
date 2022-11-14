

public class RaidDmgData
{
    public List<RaidDmgCardDate> CardDate = new List<RaidDmgCardDate>();
    public int RaidLevel;
    public double Dmg;
}

public class RaidDmgCardDate
{
    public string ID;
    public int Level;
    public string CardType;
    public float Add;
    public bool UpdateAdd;
    public float UpdateAllAdd;
    public int UpdateCount;
    public double Dmg;
    public int Count;
}