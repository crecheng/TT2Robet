namespace robot;

public class PrivateRobot: IRobot
{
    public long ID { get; set; }
    public long BaseId { get; set; }
    public async Task<string> GetMsg(long sendPlayer, string text, object? obj = null)
    {
        await SendMsg(text);
        return "";
    }

    public async Task<bool> SendMsg(string text, object? obj = null)
    {
        await Task.CompletedTask;
        return true;
        await Task.CompletedTask;
    }
}