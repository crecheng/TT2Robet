namespace robot;

public class GroupRobot: IRobot
{
    public long ID { get; set; }
    public long BaseId { get; set; }
    
    public virtual async Task<string> GetMsg(long sendPlayer, string text, object? obj = null)
    {
        await SendMsg(text);
        return "";
    }

    public virtual async Task<bool> SendMsg(string text, object? obj = null)
    {
        return true;
    }
}