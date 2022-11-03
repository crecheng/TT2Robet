using System.Collections.Generic;

public class ItemData
{
    public string Id;
    public string ShowName;
    public string Des;
    public string DesFormat;
    public string Img;
    public string Block;
    public int BlockPre;
    public int MaxLevel;
    public string PreId;
    public string CalType;
    public string RealType;
    public List<ItemLevelData> Level = new List<ItemLevelData>();
    public int Row;
    public int Col;

    private int _blockRow;

    public ItemData()
    {
        Img = "39";
    }

    public int GetBlockRow() => _blockRow;
    public void SetBlockRow(int row) => _blockRow = row;


    public void AddNewLevel(int index=-1)
    {
        var l = new ItemLevelData
        {
            Cost = 0,
        };
        if(index==-1) 
            Level.Add(l);
        else
            Level.Insert(index,l);
        
    }
}
