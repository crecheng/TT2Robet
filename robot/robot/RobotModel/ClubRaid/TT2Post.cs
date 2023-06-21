using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace testrobot
{
    public class TT2Post
    {
        public TT2PostConfig Tt2Post;
        public PlayerData Player;
        public ClubData Club;

        public enum TT2Fun
        {
            PlayerProfile,
            InfoFiles,
            RaidCurrent,
            SetTarget,
            Forum,
            SoloRaid
        }

        public TT2Post()
        {
        }


        public PlayerData PlayerProfile()
        {
            string s = Post(TT2Fun.PlayerProfile);
            if (s.Length < 200)
                OutError("PlayerProfile");
            return Player = JsonConvert.DeserializeObject<PlayerData>(s);
        }

        public ClubData RaidCurrent(string tmpFile)
        {
            string s = Post(TT2Fun.RaidCurrent);
            if (s.Length < 200)
                OutError("PlayerProfile");
            File.WriteAllText(tmpFile,s);
            return Club = JsonConvert.DeserializeObject<ClubData>(s);
        }

        public MsgDataList GetForum(string tmpFile)
        {
            string s = Post(TT2Fun.Forum);
            if (s.Length < 200)
                OutError("GetForum");
            File.WriteAllText(tmpFile,s);
            return JsonConvert.DeserializeObject<MsgDataList>(s);
        }
        
        public AllSoloRaidData SoloRaid(string tmpFile)
        {
            string s = Post(TT2Fun.SoloRaid);
            if (s.Length < 200)
                OutError("SoloRaid");
            File.WriteAllText(tmpFile,s);
            return JsonConvert.DeserializeObject<AllSoloRaidData>(s);
        }

        public bool SetCurrentTarget()
        {
            var target = Club.titan_lords.current.Target;
            string s = Post(TT2Fun.SetTarget,
                new[] {target.EnemyId, target.GetTarget()});
            Console.WriteLine(s);
            Dictionary<string, string> targets = null;
            try
            {
                targets = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                return targets != null && targets.Count > 0;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string Post(TT2Fun fun, string[] parms = null)
        {
            string s = "";
            if (!Tt2Post.key.ContainsKey(fun))
                return "fun no Contains Key";
            var webKey = Tt2Post.key[fun];
            string url =
                String.Format(
                    $"https://tt2.gamehivegames.com/{webKey.head}?d=android&v={Tt2Post.Version}&s={webKey.s}");

            var dic = new Dictionary<string, string>()
            {
                {"X-HTTP-Method-Override", "POST"},
                {"X-Tt2-Vendor-Id", Tt2Post.vendor},
                {"X-Tt2-Current-Stage", Tt2Post.stage.ToString()},
                {"Authorization", "token " + Tt2Post.token},
                {"X-TT2-session-id", Tt2Post.SessionId}
            };
            string json = webKey.json;
            if (!string.IsNullOrWhiteSpace(webKey.SessionId))
            {
                dic["X-TT2-session-id"] = webKey.SessionId;
            }

            if (parms != null)
            {
                json = String.Format(json, parms);
                json = "{" + json + "}";
            }

            s = InlinePost(url, dic, json);
            return s;
        }

        public class SendInfo
        {
            public string head;
            public string s;
            public string json;
            public string SessionId;

            public SendInfo(string head, string s, string json, string sessionId = "")
            {
                this.head = head;
                this.s = s;
                this.json = json;
                SessionId = sessionId;
            }
        }

        public void OutError(string s)
        {
            Console.WriteLine(s + "error");
        }

        public static bool UseGZip = false;
        public string InlinePost(string url, Dictionary<string, string> dic, string json)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            req.Accept = "application/vnd.gamehive.btb4-v1.0+json";
            req.UserAgent = "BestHTTP/2 v2.3.1";
            req.Host = "tt2.gamehivegames.com";

            StringBuilder builder = new StringBuilder();
            req.Headers.Add("Accept-Encoding", "gzip, identity");
            req.Headers.Add("TE", "identity");
            if (dic != null)
                foreach (var item in dic)
                {
                    req.Headers.Add(item.Key, item.Value);
                }

            byte[] data = Encoding.UTF8.GetBytes(json);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse) req.GetResponse();

            using (Stream stream = resp.GetResponseStream())
            {
                if(UseGZip)
                    result = GZipDecompress(stream, Encoding.UTF8);
                else
                {
                    StreamReader reader = new StreamReader(stream); 
                    result = reader.ReadToEnd();
                }
            }
            //获取响应内容

            return result;
        }

        public static string GZipDecompress(Stream stream, Encoding encoding)
        {
            GZipStream g = new GZipStream(stream, CompressionMode.Decompress);
            byte[] d = new byte[20480];
            int l = g.Read(d, 0, 20480);
            StringBuilder s = new StringBuilder();
            while (l > 0)
            {
                s.Append(encoding.GetString(d, 0, l));
                l = g.Read(d, 0, 20480);
            }

            return s.ToString();
        }

    }

    public partial class TT2PostConfig
    {
        public string SessionId ;
        public string token  ;
        public string vendor ;
        public int stage ;
        public Dictionary<TT2Post.TT2Fun, TT2Post.SendInfo> key;
        public string Version = "5.22.0";
        public long SupplyQQ;
        public string AppToken;
        public string PlayerToken;


        public string GetEasyData()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>()
            {
                {"SessionId",SessionId},
                {"token",token},
                {"vendor",vendor},
            };

            return JsonConvert.SerializeObject(dictionary);
        }
        public bool CanUse()
        {
            bool can = !(string.IsNullOrEmpty(SessionId) || SessionId.Length < 10);
            if (string.IsNullOrEmpty(token) || token.Length < 10)
                can = false;
            if (string.IsNullOrEmpty(vendor) || vendor.Length < 10)
                can = false;
            if (key == null)
                can = false;
            return can;
        }
        public string Save()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
