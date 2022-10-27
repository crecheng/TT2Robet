using System;
using System.Collections.Generic;

namespace testrobot
{
    public class ClubData
    {
        public double clan_advanced_start;
        public ClubInfo clan_raid;
        public RaidInfo titan_lords;
        public PlayerRaid player_raid;
        public int clan_raid_tickets;
        public int daily_raid_tickets;
        public string highest_raid_difficulty;
        public int loyalty_level;

        public int raid_clan_level;

        public string raid_count_reset_time;

        public int raid_count_this_week;

        public int raids_left_this_week;

        public List<object> rewards;

        public int total_clan_raid_attack_xp;

        public int total_clan_xp;

        public int total_raid_player_xp;


        
        public DateTime RaidBegin;
        public DateTime NextAttackTime;

        public override string ToString()
        {
            return $"部落突袭: {clan_raid}\n" +
                   $"突袭票: {clan_raid_tickets}\n" +
                   $"每日突袭票: {daily_raid_tickets}\n" +
                   $"最高突袭关卡: {highest_raid_difficulty}\n" +
                   $"忠诚度等级: {loyalty_level}\n" +
                   $"突袭等级: {raid_clan_level}\n" +
                   $"钥匙刷新剩余时间: {raid_count_reset_time}\n" +
                   $"本周突袭次数: {raid_count_this_week}\n" +
                   $"本周剩余突袭次数: {raids_left_this_week}\n" +
                   $"突袭奖励: {rewards.ShowAll("\n")}\n" +
                   $"部落经验提升: {total_clan_raid_attack_xp}\n" +
                   $"当前部落经验: {total_clan_xp}\n" +
                   $"个人突袭经验: {total_raid_player_xp}\n" +
                   $"突袭BOSS: {titan_lords}\n" +
                   $"突袭人数: {player_raid}";
        }
        
        public TitanData GetCurrentTitanData()
        {
            return titan_lords.current;
        }
        public void Init()
        {
            titan_lords.blueprints.ForEach((data) =>
            {
                if (titan_lords.target_states.ContainsKey(data.enemy_id))
                {
                    data.Target=new RaidAttackTarget(data.enemy_id,titan_lords.target_states[data.enemy_id]);
                }
            });
            var id = titan_lords.current.enemy_id;
            if (titan_lords.target_states.ContainsKey(id))
            {
                titan_lords.current.Target=new RaidAttackTarget(id,titan_lords.target_states[id]);
            }
            
            RaidBegin = ClubTool.Parse(clan_raid.raid_active_at_utc)+new TimeSpan(8,0,0);
            NextAttackTime = RaidBegin + new TimeSpan(clan_raid.max_potential_raid_attacks / 6 * 12, 0, 0);
        }
    }

    public class ClubInfo
    {
        public string area_id;
        public List<Bonuses> bonuses;
        public BoostBonus boost_bonus;
        
        
        public class Bonuses
        {
            public double BonusAmount;
            public string BonusType;
            public string Suffix;

            public override string ToString()
            {
                return $"{nameof(BonusAmount)}: {BonusAmount}\n" +
                       $" {nameof(BonusType)}: {BonusType}\n" +
                       $" {nameof(Suffix)}: {Suffix}\n";
            }
        }
        public class BoostBonus
        {
            public double BonusAmount;
            public string BonusType;

            public override string ToString()
            {
                return $"{nameof(BonusAmount)}: {BonusAmount}\n" +
                   $" {nameof(BonusType)}: {BonusType}\n";
            }
        }

        public int boost_tickets;
        public string clan_code;
        public string created_at;
        public string created_at_utc;
        public object finalization_started_at_utc;
        public List<PlayerData> leaderboard;
        public int level;
        public int max_potential_raid_attacks;
        public string raid_active_at_utc;
        public string raid_closed_at_utc;
        public string raid_completion_requirement;
        public long raid_id;
        public string raid_retired_at_utc;
        public bool isShowPlayer = false;

        public List<BoostBonus> special_card_info;
        public string state;
        public int tier;
        public bool was_highest_completed_raid;

        public override string ToString()
        {
            return $"突袭区等级: {area_id}\n" +
                   $"bonusesff类型: {bonuses.ShowAll("\n","--")}\n" +
                   $"boost_bonusff数值: {boost_bonus}\n" +
                   $" {nameof(boost_tickets)}: {boost_tickets}\n" +
                   $"部落码: {clan_code}\n" +
                   $"突袭开始时间: {created_at}\n" +
                   $"突袭开始标准时间: {created_at_utc}\n" +
                   $" {nameof(finalization_started_at_utc)}: {finalization_started_at_utc}\n" +
                   $" {nameof(leaderboard)}: {(isShowPlayer?leaderboard.ShowAll("\n"):"...")}\n" +
                   $" {nameof(level)}: {level}\n" +
                   $" {nameof(max_potential_raid_attacks)}: {max_potential_raid_attacks}\n" +
                   $" {nameof(raid_active_at_utc)}: {raid_active_at_utc}\n" +
                   $" {nameof(raid_closed_at_utc)}: {raid_closed_at_utc}\n" +
                   $" {nameof(raid_completion_requirement)}: {raid_completion_requirement}\n" +
                   $" {nameof(raid_id)}: {raid_id}\n" +
                   $" {nameof(raid_retired_at_utc)}: {raid_retired_at_utc}\n" +
                   $" {nameof(special_card_info)}: {special_card_info.ShowAll("\n")}\n" +
                   $" {nameof(state)}: {state}\n" +
                   $" {nameof(tier)}: {tier}\n" +
                   $" {nameof(was_highest_completed_raid)}: {was_highest_completed_raid}";
        }
    }

    public class RaidInfo
    {
        public List<TitanData> blueprints;
        public TitanData current;
        public int currentIndex;
        public List<string> sequence;
        public Dictionary<string,string> target_states;

        public double GetNeedAllDmg()
        {
            double all = 0;
            blueprints.ForEach((i) =>
            {
                sequence.ForEach((enemy) =>
                {
                    if (i.enemy_id == enemy)
                    {
                        Console.WriteLine($"{TitanData.EnemyIdName(enemy)}:{i.GetNeedAllDmg().ShowNum()}");
                        all += i.GetNeedAllDmg();
                    }
                });
            });

            return all;
        }

        public override string ToString()
        {
            return $"{nameof(blueprints)}: {blueprints.ShowAll("\n","--")}\n" +
                   $" {nameof(current)}: {current}\n" +
                   $" {nameof(currentIndex)}: {currentIndex}\n" +
                   $" {nameof(sequence)}: {sequence.ShowAll("\n","--", TitanData.EnemyIdName)}\n" +
                   $" {nameof(target_states)}: {target_states.ShowAll("\n","--")}";
        }
    }
    
    public class PlayerRaid
    {
        public int attacks_remaining;
        public string attacks_reset_time;
        public bool is_participating;
        public string next_attack_time;
        public RecentSkills recent_skills;
        public int remaining_holiday_currency_grants;
        public class RecentSkills
        {
            public string expires_at;
            public List<object> skill_ids;

            public override string ToString()
            {
                return $"{nameof(expires_at)}: {expires_at}\n" +
                       $" {nameof(skill_ids)}: {skill_ids.ShowAll()}";
            }
        }

        public override string ToString()
        {
            return $"{nameof(attacks_remaining)}: {attacks_remaining}\n" +
                   $" {nameof(attacks_reset_time)}: {attacks_reset_time}\n" +
                   $" {nameof(is_participating)}: {is_participating}\n" +
                   $" {nameof(next_attack_time)}: {next_attack_time}\n" +
                   $" {nameof(recent_skills)}: {recent_skills}\n" +
                   $" {nameof(remaining_holiday_currency_grants)}: {remaining_holiday_currency_grants}";
        }
    }
}