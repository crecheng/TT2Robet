// using System.Net;
// using System.Security.Cryptography;
// using System.Text;
// using Newtonsoft.Json;
//
// namespace robot;
//
// public static class AI
// {
//     /*{
//     "message": "success",
//     "data": {
//         "type": 5000,
//         "info": {
//             "text": "你请我吃饭"
//         }
//     }
// }*/
//     public class Message
//     {
//         public string message;
//         public MessageData data;
//
//         public string text=>data.Info.text;
//     }
//     
//     public class MessageData
//     {
//         public int type;
//         public MessageInfo Info;
//     }
//     
//     public class MessageInfo
//     {
//         public string text;
//     }
//
//     public const string Appid = "bd6b0fc53c8359766a162f31c47a5078";
//
//     public static string MD5Encrypt16(string text)
//     {
//         var md5 = new MD5CryptoServiceProvider();
//         string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(text)), 4, 8);
//         t2 = t2.Replace("-", "");
//         return t2;
//     }
//
//     public static string postFormat = "https://api.ownthink.com/bot?appid={0}&userid={1}&spoken={2}";
//     
//     public static string MD5Encrypt64(string text)
//     {
//         string cl = text;
//         //string pwd = "";
//         MD5 md5 = MD5.Create(); //实例化一个md5对像
//         // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
//         byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
//         return Convert.ToBase64String(s);
//     }
//
//     public static string GetUserId(long group, long qq)
//     {
//         var g = MD5Encrypt64(group.ToString());
//         var q = MD5Encrypt64(qq.ToString());
//         return Convert.ToBase64String(Encoding.UTF8.GetBytes(MD5Encrypt64(g + q))) ;
//     }
//
//     public static async Task<string> GetAIReply(long group, long qq, string text)
//     {
//         
//         var s = string.Format(postFormat, Appid, GetUserId(group, qq), text);
//         Console.WriteLine($"[{group}]-[{qq}]-[{text}]-[{s}]");
//         HttpWebRequest request = (HttpWebRequest) WebRequest.Create(s);
//         request.Method = WebRequestMethods.Http.Get;
//         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//         Stream myResponseStream = response.GetResponseStream();
//         StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
//         string retString = await myStreamReader.ReadToEndAsync();
//         myStreamReader.Close();
//         myResponseStream.Close();
//         var json = JsonConvert.DeserializeObject<Message>(retString);
//         if (json != null)
//         {
//             return json.text;
//         }
//
//         return "";
//     }
//
//
// }