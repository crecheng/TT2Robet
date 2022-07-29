using Sora.Entities.Segment;

namespace robot;

public static class Tool
{
    public static string ChangelongNumber(long number)
    {
        List<string> endCharacters = new List<string>() { "", "K", "M", "B", "T" };
        if (number > 999999)//99.9w
        {
            string[] chinaEndCharacters = new string[] { "万", "亿", "兆", "京", "垓" };
            int i = 0;
            while (number / Math.Pow(10, 4 * (i + 1)) > 9999) 
            {
                i++;
            }
            var now = (number / Math.Pow(10, 4 * (i + 1))).ToString("0.0000");
            return $"{now.Substring(0, now[4] == '.'? 4 : 5)}{chinaEndCharacters[i]}";
        }
        else
        {
            return number.ToString();
        }
    }

    public static string AppPath;
    public static readonly Random Random = new Random();

    /// <summary>
    /// 发送图片
    /// </summary>
    /// <param name="path">相对路径</param>
    /// <returns></returns>
    public static SoraSegment Image(string path)
    {
        return SoraSegment.Image($"{AppDomain.CurrentDomain.BaseDirectory}\\{path}");
    }
    
    public static SoraSegment RandomImage(string path)
    {
        return Image(GetRandomName(path));
    }

    public static string GetRandomName(string path)
    {
        if (!Directory.Exists(path))
            return "";

        var files = Directory.GetFiles(path);
        if (files.Length <= 0)
            return "";
        return files[Random.Next(files.Length)];
    }
}