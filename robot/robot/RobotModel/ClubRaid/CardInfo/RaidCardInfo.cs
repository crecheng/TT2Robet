
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace ConfigData
{
public partial class CardData
{
	///CardID
	public string ID;
	///名字
	public string Name;
	///Note
	public string Note;
	///类别
	public string Category;
	///Tier
	public string Tier;
	///BestAgainst
	public string BestAgainst;
	///最大叠层
	public int MaxStacks;
	///时间
	public float Duration;
	///几率
	public float Chance;
	///BonusTypeA
	public string BonusTypeA;
	///BonusAValue
	public float[] BonusAValue;
	///BonusTypeB
	public string BonusTypeB;
	///BonusBValue
	public float[] BonusBValue;
	///BonusTypeC
	public string BonusTypeC;
	///BonusCValue
	public float BonusCValue;
	///BonusTypeD
	public string BonusTypeD;
	///BonusAmountD
	public float BonusDValue;
	///BonusTypeE
	public string BonusTypeE;
	///BonusAmountE
	public float BonusEValue;
	///BonusTypeF
	public string BonusTypeF;
	///BonusAmountF
	public float BonusFValue;
}



	class ConfigJsonData
	{
		public CardData[] CardData;
	}
	public class ConfigDataManage
	{
		Dictionary<string, List<CardData>> _CardDataDictionary = new Dictionary<string, List<CardData>>();

		public void InitJson(string json)
		{
			var data = JsonConvert.DeserializeObject<ConfigJsonData>(json);
			if (data.CardData != null)
			{
				foreach (var i in data.CardData)
				{
					if (i != null)
					{
						if(!_CardDataDictionary.ContainsKey(i.ID))
							_CardDataDictionary.Add(i.ID, new List<CardData>(){i}); 
						else
							_CardDataDictionary[i.ID].Add(i);
					}
				}
			}
			else
			{
				Console.WriteLine("CardData not found in json data");
			}

		}

		public bool HasKeyCardData(string id) => _CardDataDictionary.ContainsKey(id);

		public Dictionary<string, List<CardData>>.KeyCollection GetCardDataKeyCollection() => _CardDataDictionary.Keys;

		public List<CardData> GetCardDataData(string id)
		{
			if (_CardDataDictionary.TryGetValue(id, out var list)) 
				return list;
			throw new Exception($"Config Data Error : No {id} in CardDataConfig");
		}

		public CardData GetCardDataDataFirst(string id) => GetCardDataData(id)[0];

	}
}