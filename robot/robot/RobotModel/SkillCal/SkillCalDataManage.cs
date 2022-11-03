using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class SkillCalDataManage
{
    public static SkillCalDataManage Instance { get; private set; }

    public Dictionary<string, Dictionary<string, float>> AdditionConvert =
        new Dictionary<string, Dictionary<string, float>>();

    public Dictionary<string, ItemData> Datas = new Dictionary<string, ItemData>();
    
    private HashSet<string> _row = new HashSet<string>();

    private Dictionary<string, List<ItemData>> _dicData = new Dictionary<string, List<ItemData>>();

    private Dictionary<string, Dictionary<int, List<ItemData>>> _blockRowDic =
        new Dictionary<string, Dictionary<int, List<ItemData>>>();

    public SkillCalDataManage()
    {
        
    }


    public static void Init(SkillCalDataManage skillCalData)
    {
        Instance = skillCalData;
        Instance.Load();
    }
    public void Load()
    {
        foreach (var (key, dictionary) in AdditionConvert)
        {
            foreach (var (s, value) in dictionary)
            {
                _row.Add(s);
            }
        }
        
        Refresh();
    }

    public string Save()
    {
        return JsonConvert.SerializeObject(this);
    }

    private void RefreshDic()
    {
        _dicData.Clear();
        foreach (var (key, value) in Datas)
        {
            if(!_dicData.ContainsKey(value.Block))
                _dicData.Add(value.Block,new List<ItemData>());
            _dicData[value.Block].Add(value);
        }
        
        _blockRowDic.Clear();
        foreach (var (block,value) in _dicData)
        {
            var dic = new Dictionary<int, List<ItemData>>();
            var tmp = new List<ItemData>(value);
            var index = 0;
            int max = tmp.Count * tmp.Count+10;
            while (tmp.Count>0)
            {
                max--;
                if(max<0)
                    break;
                var d = tmp[index];
                var insert = -1;
                if (string.IsNullOrEmpty(d.PreId) || d.PreId == "None")
                {
                    insert = 0;
                }
                else
                {
                    foreach (var (key, lists) in dic)
                    {
                        if (lists.Contains(Datas[d.PreId]))
                        {
                            insert = key+1;
                            break;
                        }
                    }
                }

                if (insert != -1)
                {
                    if (!dic.ContainsKey(insert))
                        dic.Add(insert, new List<ItemData>());
                    dic[insert].Add(tmp[index]);
                    d.SetBlockRow(insert);
                    tmp.RemoveAt(index);
                }
                else
                    index++;    
                if(tmp.Count==0)
                    break;
                index %= tmp.Count;
            }
            _blockRowDic.Add(block,dic);
        }
    }

    public void RefreshAC()
    {
        _row.Clear();
        foreach (var (key, dictionary) in AdditionConvert)
        {
            foreach (var (s, value) in dictionary)
            {
                _row.Add(s);
            }
        }
    }

    public void Refresh()
    {
        RefreshDic();
    }
    

    public void AddCol(string type)
    {
        if(AdditionConvert.ContainsKey(type) || string.IsNullOrWhiteSpace(type))
            return;
        var dic = new Dictionary<string, float>();
        foreach (var s in _row)
        {
            dic.Add(s, 0);
        }
        AdditionConvert.Add(type,dic);
    }

    public void RemoveCol(string type)
    {
        AdditionConvert.Remove(type);
    }

    public void AddRow(string type)
    {
        if(_row.Contains(type) || string.IsNullOrWhiteSpace(type))
            return;
        _row.Add(type);
        foreach (var (key, value) in AdditionConvert)
        {
            value.Add(type, 0);
        }
    }
    
    public void RemoveRow(string type)
    {
        _row.Remove(type);
        foreach (var (key, value) in AdditionConvert)
        {
            value.Remove(type);
        }
    }

    public HashSet<string> GetRow() => _row;

    public void AddNewData()
    {
        var d = new ItemData
        {
            Id = Datas.Count.ToString(),
            ShowName = "名字",
            Img = "39",
            Des = "描述"
        };
        
        d.AddNewLevel();
        Datas.Add(d.Id,d);
    }

    public int GetDicCol(string block)
    {
        if (!_dicData.ContainsKey(block))
            return 0;

        var d = _dicData[block];
        int max = 0;
        foreach (var itemData in d)
        {
            if (itemData.Col > max)
                max = itemData.Col;
        }

        return max;
    }

    public List<ItemData> GetBlockRow(string block,int row)
    {
        return _blockRowDic[block][row];
    }

    public Dictionary<int, List<ItemData>> GetBlockRow(string block)
    {
        return _blockRowDic[block];
    }

    public ItemData[,] GetDicMat(string block)
    {
        var num = Instance.GetMaxDicColRow();
        var col  = num >> 16;
        var row = num & 0xffff;
        var res = new ItemData[col, row];
        var data = _dicData[block];
        foreach (var itemData in data)
        {
            res[itemData.Col - 1, itemData.Row - 1] = itemData;
        }

        return res;
    }
    
    public ItemData[,] GetDicMat(string block,int col,int row)
    {
        var res = new ItemData[col, row];
        var data = _dicData[block];
        foreach (var itemData in data)
        {
            res[itemData.Col - 1, itemData.Row - 1] = itemData;
        }

        return res;
    }

    public int GetMaxDicColRow()
    {
        int row=0;
        int col=0;
        foreach (var data in _dicData)
        {
            foreach (var itemData in data.Value)
            {
                if (itemData.Col > col)
                    col = itemData.Col;
                if (itemData.Row > row)
                    row = itemData.Row;
            }
        }


        return (col << 16) + row;
    }

    public List<string> GetAllBlock()
    {
        return new List<string>(_dicData.Keys);
    }
}
