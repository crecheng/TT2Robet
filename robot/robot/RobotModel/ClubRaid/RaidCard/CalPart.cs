

using testrobot;

public class CalPart
{
    public double current_hp;
    public bool enchanted;
    public TitanData.PartName part_id;
    public double total_hp;
    public double partAdd;
    public Dictionary<string, List<float>> DotDic = new Dictionary<string, List<float>>();

    public void Reset()
    {
        DotDic.Clear();
    }

    public int GetDotCount()
    {
        int c = 0;
        foreach (var (key, value) in DotDic)
        {
            c += value.Count;
        }

        return c;
    }
    
    public int GetDotCount(string key)
    {
        if(!DotDic.ContainsKey(key))
            return 0;
        return DotDic[key].Count;
    }

    public int UpdateDot(string key, float time)
    {
        if(!DotDic.ContainsKey(key))
            return 0;
        var list = DotDic[key];
        for (var i = list.Count - 1; i >= 0; i--)
        {
            var ct = list[i];
            ct -= time;
            if (ct <= 0)
            {
                list.RemoveAt(i);
            }
            else
            {
                list[i] = ct;
            }
        }
        return list.Count;
    }

    public void ClearDot()
    {
        foreach (var (key, value) in DotDic)
        {
            value.Clear();
        }
    }

    
    public bool AddDot(string key, float time,int max)
    {
        if(!DotDic.ContainsKey(key))
            DotDic.Add(key,new List<float>());
        var list = DotDic[key];
        if(list.Count<max)
            list.Add(time);
        else
        {
            float min = float.MaxValue;
            int index=0;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                {
                    min = list[i];
                    index = i;
                }
            }

            list[index] = time;
            return true;
        }

        return false;
    }
    public float GetCardAdd(RaidCal.CardAdd cardAdd,string cardType="")
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