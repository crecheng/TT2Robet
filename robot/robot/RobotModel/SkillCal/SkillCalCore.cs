using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public class SkillCalCore
{
    public List<string> Build;
    public Dictionary<string, int> Point;
    public Dictionary<string, int> BlockPoint;
    public Dictionary<string, SkillCalData> _calTable;
    public List<string> NextList;
    public int AllSkillPoint;
    public int HaveSkillPoint;
    public event Action<string> OnException; 

    public SkillCalCore()
    {
        Point = new Dictionary<string, int>();
        BlockPoint = new Dictionary<string, int>();
        _calTable = new Dictionary<string, SkillCalData>();
        NextList = new List<string>();
        ClearInit();
    }

    public void ClearInit()
    {
        Point.Clear();
        BlockPoint.Clear();
        _calTable.Clear();
        NextList.Clear();
        
        AllSkillPoint = 0;
        HaveSkillPoint = 0;
        
        foreach (var (key, value) in SkillCalDataManage.Instance.Datas)
        {
            Point.Add(value.Id,0);
            if(!BlockPoint.ContainsKey(value.Block))
                BlockPoint.Add(value.Block,0);
            _calTable.Add(value.Id,default);
        }
    }
    

    public int NextStepNum = 1;
    public async Task CalRun(int skill, List<string> build)
    {
        if(build.Count==0)
            return;
        Build = new List<string>(build);
        ClearInit();
        RefreshTable("",true);
        AllSkillPoint = skill;
        HaveSkillPoint = skill;
        int addNext = -1;
        try
        {
            while (true)
            {
                //Debug.Log($"{HaveSkillPoint}/{AllSkillPoint}");
                var next = GetNext();
                List<SkillCalData> list = new List<SkillCalData>();
                if(next.List!=null)
                    list.AddRange(next.List);
                else 
                    list.Add(next);
                foreach (var data in list)
                {
                    if (data.SP > HaveSkillPoint && addNext==-1)
                    {
                        addNext = 0;
                    }

                    if (addNext>-1)
                    {
                        NextList.Add(data.Id);
                    }
                    else
                    {
                        HaveSkillPoint -= data.SP;
                        Point[data.Id] += 1;
                        if (SkillCalDataManage.Instance.Datas[data.Id].MaxLevel == Point[data.Id])
                            _calTable.Remove(data.Id);
                        BlockPoint[data.Block] += data.SP;
                    }
                }

                if (addNext>=NextStepNum)
                    break;
                if(addNext!=-1) addNext++;
                RefreshTable(next.Block);
            }
        }
        catch (Exception e)
        {
            OnException?.Invoke(e.ToString());
           
        }
    }

    public void Rest(List<string> build)
    {
        Build = new List<string>(build);
        ClearInit();
        RefreshTable("",true);
    }

    public void NextStep()
    {
        var next = GetNext();
        List<SkillCalData> list = new List<SkillCalData>();
        if(next.List!=null)
            list.AddRange(next.List);
        else 
            list.Add(next);
        foreach (var data in list)
        {
            Point[data.Id] += 1;
            if (SkillCalDataManage.Instance.Datas[data.Id].MaxLevel == Point[data.Id])
                _calTable.Remove(data.Id);
            BlockPoint[data.Block] += data.SP;
        }
        
        RefreshTable(next.Block);
    }

    SkillCalData GetNext()
    {
        BFloat max = 0;
        SkillCalData res=default;
        foreach (var (key, value) in _calTable)
        {
            if (value.Cal > max)
            {
                max = value.Cal;
                res = value;
            }
        }

        return res;
    }

    void RefreshTable(string block="",bool force=false)
    {
        var list = new List<string>(_calTable.Keys);
        foreach (var key in list)
        {
            var data = SkillCalDataManage.Instance.Datas[key];
            if (block == data.Block || force)
            {
                bool preSlot = !(!string.IsNullOrWhiteSpace(data.PreId) && Point[data.PreId] == 0);
                bool preBlock = BlockPoint[data.Block] >= data.BlockPre;
                if (preSlot && preBlock)
                    _calTable[key] = GetSkillCalData(key, Point[key] + 1);
                else
                    _calTable[key] = GetSkillCalDataByPre(key, Point[key] + 1);
            }
        }
    }

    SkillCalData GetSkillCalData(string id, int level)
    {
        var data = SkillCalDataManage.Instance.Datas[id];
        if (level > data.MaxLevel)
        {
            var re1 = new SkillCalData()
            {
                Id = id,
                SP = 0,
                Level = level,
                Cal = 0,
                Block = data.Block
            };
            return re1;
        }

        var levelData = data.Level[level - 1];
        var lastCalNum = level == 1 ? 1.0 : data.Level[level - 2].CalNum;
        var up = levelData.CalNum / lastCalNum;


        BFloat a = 1;
        foreach (var s in Build)
        {
            if (SkillCalDataManage.Instance.AdditionConvert.ContainsKey(s))
            {
                var all = SkillCalDataManage.Instance.AdditionConvert[s];
                if (!string.IsNullOrEmpty(data.CalType) && all.ContainsKey(data.CalType))
                {
                    var add = all[data.CalType];
                    a *= Math.Pow(up, add);
                }
            }
        }

        BFloat num = BFloat.Pow(a, 1.0 / levelData.Cost);
        var re = new SkillCalData()
        {
            Id = id,
            SP = levelData.Cost,
            Level = level,
            Cal = num,
            AllCal = a,
            Block = data.Block
        };
        return re;

    }

    SkillCalData GetSkillCalDataByPre(string id,int level)
    {
        var data = SkillCalDataManage.Instance.Datas[id];
        bool preBlock = BlockPoint[data.Block] > data.BlockPre;
        List<SkillCalData> list = new List<SkillCalData>();
        if (preBlock)
        {
            BFloat all = 1;
            int cost = 0;
            var pre = id;
            while (!string.IsNullOrWhiteSpace(pre) && pre!="None")
            {
                var preData = SkillCalDataManage.Instance.Datas[pre];
                if(Point[pre]>0)
                    break;
                foreach (var s in Build)
                {
                    if(SkillCalDataManage.Instance.AdditionConvert.ContainsKey(s))
                    {
                        var allAc = SkillCalDataManage.Instance.AdditionConvert[s];
                        if(!string.IsNullOrEmpty(preData.CalType) && allAc.ContainsKey(preData.CalType))
                        {
                            var add = allAc[preData.CalType];
                            var cur = Point[pre];
                            var levelData = preData.Level[cur];
                            var lastCalNum = cur == 0 ? 1 : preData.Level[cur - 1].CalNum;
                            var up = levelData.CalNum / lastCalNum;
                            var cal=Math.Pow(up, add);
                            all *= cal;
                            cost += levelData.Cost;
                            
                            list.Add(new SkillCalData()
                            {
                                Id = pre,
                                SP = levelData.Cost,
                                Level = level,
                                Cal = cal,
                                AllCal = all,
                                Block = data.Block
                            });
                        }
                    }
                }
                pre = preData.PreId;
            }
            return new SkillCalData()
            {
                List = list,
                Id = id,
                SP = cost,
                Level = level,
                AllCal = all,
                Cal = BFloat.Pow(all,1.0/cost),
                Block = data.Block
            };
            
        }

        return GetSkillCalDataByPreBlock(id, level);
    }

    SkillCalData GetSkillCalDataByPreBlock(string id,int level)
    {
        var data = SkillCalDataManage.Instance.Datas[id];
        var rows = SkillCalDataManage.Instance.GetBlockRow(data.Block);

        var lastRow = GetSkillCalDataByPreBlockRow(data.Block, data.GetBlockRow() - 1, data.BlockPre,data.PreId);
        var cur = GetSkillCalData(id, level);
        var all = lastRow.AllCal * cur.AllCal;
        var cost = lastRow.SP + cur.SP;
        if (lastRow.Cal > cur.Cal)
        {
            lastRow.Id = id;
            lastRow.Block = data.Block;
            lastRow.Level = level;
            return lastRow;
        }
        lastRow.List.Add(cur);
        
        var re=new SkillCalData()
        {
            Id = id,
            List = lastRow.List,
            AllCal = all,
            Block = data.Block,
            SP = cost,
            Cal = BFloat.Pow(all, 1.0 / cost),
            Level = level,
        };
        return re;
    }

    SkillCalData GetSkillCalDataByPreBlockRow(string block, int row, int need,string needId)
    {
        var rowList = SkillCalDataManage.Instance.GetBlockRow(block, row);
        var upRowNeed = rowList[0].BlockPre;
        var needData = SkillCalDataManage.Instance.Datas[needId];
        Dictionary<string, int> tmpPoint = new Dictionary<string, int>();
        Dictionary<string, SkillCalData> tmpTable = new Dictionary<string, SkillCalData>();
        List<SkillCalData> res;
        var thisNeed = need - BlockPoint[block];
        BFloat all = 1;
        int cost = 0;
        for (int i = row; i >= 0; i--)
        {
            var tmpRow = SkillCalDataManage.Instance.GetBlockRow(block, i);
            foreach (var itemData in tmpRow)
            {
                tmpPoint.Add(itemData.Id, Point[itemData.Id]);
                tmpTable.Add(itemData.Id, default);
            }
        }

        rowList.ForEach(i =>
        {
            if (i.BlockPre != upRowNeed)
            {
                OnException.Invoke(new Exception("暂时不支持同行技能区域前置不同").ToString());
            }
        });
        if (BlockPoint[block] < upRowNeed )
        {
            var lastRow = GetSkillCalDataByPreBlockRow(block, row - 1, upRowNeed,needData.PreId);
            res = lastRow.List;
            all = lastRow.AllCal;
            thisNeed -= lastRow.SP;
            cost = lastRow.SP;
            foreach (var data in res)
            {
                tmpPoint[data.Id] += 1;
            }
        }
        else
        {
            res = new List<SkillCalData>();
        }

        if (tmpPoint[needId] == 0)
        {
            var needSkillData = GetSkillCalData(needId, 1);
            all *= needSkillData.AllCal;
            cost += needSkillData.SP;
            thisNeed -= needSkillData.SP;
            tmpPoint[needId] += 1;
            res.Add(needSkillData);
        }

        while (thisNeed > 0)
        {
            foreach (var (key, value) in tmpPoint)
            {
                tmpTable[key] = GetSkillCalData(key, value + 1);
            }

            SkillCalData maxData = default;
            maxData.Cal = 0;
            foreach (var (key, value) in tmpTable)
            {
                if (value.Cal > maxData.Cal)
                {
                    maxData = value;
                }
            }

            var selectData = SkillCalDataManage.Instance.Datas[maxData.Id];
            if (!string.IsNullOrWhiteSpace(selectData.PreId) && selectData.PreId != "None" &&
                tmpPoint[selectData.PreId] == 0)
            {
                var needSkillData = GetSkillCalData(selectData.PreId, 1);
                all *= needSkillData.AllCal;
                cost += needSkillData.SP;
                thisNeed -= needSkillData.SP;
                tmpPoint[needId] += 1;
                res.Insert( Math.Max(0,res.Count - 1) , needSkillData);
            }

            thisNeed -= maxData.SP;
            tmpPoint[maxData.Id] += 1;
            all *= maxData.AllCal;
            cost += maxData.SP;
            res.Add(maxData);
        }

        var re= new SkillCalData()
        {
            List = res,
            Block = block,
            Cal = BFloat.Pow(all, 1.0 / cost),
            AllCal = all,
            SP = cost
        };
        return re;
    }

    

    public struct SkillCalData
    {
        public List<SkillCalData> List;
        
        public string Id;

        public string Block;
        /// 消耗技能点
        public int SP;
        /// 等级
        public int Level;
        /// 对应加成
        public BFloat Cal;
        /// 总加成
        public BFloat AllCal;
        /// 收益
        public BFloat Gains;
    }
}
