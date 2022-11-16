

using testrobot;

public class CalPart
{
    public double CurrentHp
    {
        get => BodyHp + ArmorHP;
        set
        {
            if (ArmorHP > 0)
                ArmorHP -= value;
            else
                BodyHp -= value;
        }
    }
    public double BodyHp;
    public double ArmorHP;
    public double BodyMaxHp;
    public double ArmorMaxHp;
    public bool enchanted;

    public int CurrentType
    {
        get
        {
            if (ArmorHP > 0)
                return 2;
            if (BodyHp > 0)
                return 1;
            return 0;
        }
    }
    public PartName PartId;
    public double partAdd;
    public float AfflictedAdd;
    public float AfflictedChance=1;
    public Dictionary<string, List<float>> DotDic = new Dictionary<string, List<float>>();

    public void ResetBuff()
    {
        AfflictedAdd = 0;
    }
    public enum PartName
    {
        Head,
        ChestUpper,
        ArmUpperRight,
        ArmUpperLeft,
        LegUpperRight,
        LegUpperLeft,
        HandRight,
        HandLeft,
    }
    public CalPart()
    {
        
    }
    
    public CalPart(int id,int pa)
    {
        PartId = (PartName) id;
        BodyMaxHp = BodyHp = double.MaxValue;
        if (pa == 1)
            ArmorMaxHp = ArmorHP = double.MaxValue;
    }

    public CalPart(TitanData.Part part)
    {
        int i = (int)part.part_id;
        PartId = (PartName)(i / 2);
        if (i % 2 == 0)
        {
            BodyHp = part.current_hp;
            BodyMaxHp = part.total_hp;
        }
        else
        {
            ArmorHP = part.current_hp;
            ArmorMaxHp = part.total_hp;
        }

        enchanted = part.enchanted;
    }

    public void Add(TitanData.Part part)
    {
        int i = (int)part.part_id;
        if (i % 2 == 0)
        {
            BodyHp = part.current_hp;
            BodyMaxHp = part.total_hp;
        }
        else
        {
            ArmorHP = part.current_hp;
            ArmorMaxHp = part.total_hp;
        }
    }
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

    public int UpdateDot(string key, float time,Action<CalPart> onRemove=null)
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
                onRemove?.Invoke(this);
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
            case "Affliction":
            case "Afflicted": 
                d += cardAdd.AfflictedAdd;
                d += AfflictedAdd;
                break;
        }

        if (CurrentType==2)
            d += cardAdd.ArmorAdd;
        else
            d += cardAdd.BodyAdd;

        if (PartId == PartName.Head)
        {
            d += cardAdd.HeadAdd;
        }
        else if (PartId == PartName.ChestUpper)
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