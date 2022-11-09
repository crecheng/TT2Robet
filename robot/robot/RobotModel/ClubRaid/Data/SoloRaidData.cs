namespace testrobot;

public class SoloRaidData
{
    public string avatar_frame_id;
    public int completion_time_in_seconds;
    public string current_avatar;
    public string display_name;
    public int floors_completed;
    public string player_code;
    public int rank;
    public int world_id;
}

public class AllSoloRaidData
{
    public List<SoloRaidData> clan;
    public List<SoloRaidData> global;
    public List<SoloRaidData> world;
}