using System.Collections.Generic;
using System.Text.RegularExpressions;

 namespace testrobot
{
    public class PlayerData
    {
        public int artifacts;
        public string avatar_frame_id;
        public long qq;

        public class BadgeData
        {
            public int badge_id;
            public string date_acquired;
            public string holiday_event_id;

            public override string ToString()
            {
                return $"徽章id: {badge_id}\n" +
                       $"获得时间: {date_acquired}\n" +
                       $"事件id: {holiday_event_id}";
            }
        }

        public List<BadgeData> badge_list;

        public int challenge_tournaments_highest_tournament;
        public int challenge_tournaments_participation;
        public int challenge_tournaments_undisputed_count;
        public string clan_code;

        public int clan_icon;

        public string clan_name;

        public int clan_quests_count;

        public string country_code;

        public int num_attacks;

        public double score;

        public int crafting_shards_spent;

        private int crates_shared;

        private string current_avatar;

        private int daily_raid_tickets;

        public int player_raid_level;
        

        public int equipment_set_count;
        public string highest_pet_id;
        public int highest_tournament;
        public string last_used;
        public int loyalty_level;
        public int max_stage;
        public string name;
        public string player_background_id;
        public string player_code;
        public string raid_background_id;
        public int raid_tickets_collected;
        public string role;
        public int titan_points;
        public int total_card_level;
        public int total_clan_quests;
        public int total_helper_scrolls;
        public int total_helper_weapons;
        public int total_num_raid_attacks;
        public int total_num_unique_cards;
        public int total_pet_levels;
        public int total_pet_xp;
        public int total_raid_player_xp;
        public int total_skill_points;
        public int total_tournaments;
        public int undisputed_count;
        public int weekly_ticket_count;
        public int raid_wildcard_count;

        public override string ToString()
        {
            return $"名字: {name}\n" +
                   $"最高层数: {max_stage}\n" +
                   $"玩家代码: {player_code}\n" +
                   $"神器: {artifacts}\n" +
                   $"部落名字: {clan_name}\n" +
                   $"部落代码: {clan_code}\n" +
                   $"部落忠诚: {loyalty_level}\n" +
                   $"使用装备碎片: {crafting_shards_spent}\n" +
                   $"发送部落箱: {crates_shared}\n" +
                   $"最后时间: {last_used}\n" +
                   $"每日突袭票: {daily_raid_tickets}\n" +
                   $"总突袭票: {raid_tickets_collected}\n" +
                   $"部落职级: {role}\n" +
                   $"技能点: {total_skill_points}\n" +
                   $"深渊最高: {challenge_tournaments_highest_tournament}\n" +
                   $"深渊场数: {challenge_tournaments_participation}\n" +
                   $"深渊第一次数: {challenge_tournaments_undisputed_count}\n" +
                   $"比赛次数: {total_tournaments}\n" +
                   $"第一次数: {undisputed_count}\n" +
                   $"比赛积分: {titan_points}\n" +
                   $"最高比赛: {highest_tournament}\n" +
                   $"总卡等: {total_card_level}\n" +
                   $"总突袭数: {total_clan_quests}\n" +
                   $"卷轴: {total_helper_scrolls}\n" +
                   $"武器: {total_helper_weapons}\n" +
                   $"突袭次数: {total_num_raid_attacks}\n" +
                   $"卡数: {total_num_unique_cards}\n" +
                   $"宠物: {total_pet_levels}\n" +
                   $"宠物经验: {total_pet_xp}\n" +
                   $"突袭经验: {total_raid_player_xp}\n" +
                   $"周票数: {weekly_ticket_count}\n" +
                   $"头像框: {avatar_frame_id}\n" +
                   $"徽章: {badge_list.ShowAll("\n", "--")}\n" +
                   $"突袭背景: {raid_background_id}\n" +
                   $"部落图标: {clan_icon}\n" +
                   $"部落突袭: {clan_quests_count}\n" +
                   $"城市: {country_code}\n" +
                   $"头像: {current_avatar}\n" +
                   $"装备数量: {equipment_set_count}\n" +
                   $"最高宠物id: {highest_pet_id}\n" +
                   $"背景id: {player_background_id}\n";

        }
        
        
    }
}