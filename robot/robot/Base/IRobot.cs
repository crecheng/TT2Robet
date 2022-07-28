namespace robot;

public interface IRobot
{
     long ID { get; set; }
     long BaseId { get; set; }
     public Task<string> GetMsg(long sendPlayer, string text, object? obj=null);
     public Task<bool> SendMsg(string text, object? obj=null);
     
}