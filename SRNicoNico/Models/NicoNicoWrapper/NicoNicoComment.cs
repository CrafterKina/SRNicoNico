﻿using System;
using System.Collections.Generic;

using Livet;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

using Codeplex.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using System.Web;
using System.Linq;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoComment : NotificationObject {


		//スレッドキー取得API
		private const string GetThreadKeyApiUrl = "http://flapi.nicovideo.jp/api/getthreadkey";

        //ポストキー取得API
        private const string GetPostKeyApiUrl = "http://flapi.nicovideo.jp/api/getpostkey";

		//ウェイバックキー取得API
		private const string GetWayBackKeyApiUrl = "http://flapi.nicovideo.jp/api/getwaybackkey";

        //------
        private NicoNicoGetFlvData GetFlv;
        private WatchApiData ApiData;

        private VideoViewModel Video;
        //------

        //チケット 何に使うのかは謎
        private string Ticket;

        //コメント数
        private int LastRes;

        public NicoNicoComment(WatchApiData apiData, VideoViewModel vm) {

            ApiData = apiData;
            GetFlv = apiData.GetFlv;
            Video = vm;
		}

        //コメント取得
        public async Task<List<NicoNicoCommentEntry>> GetCommentAsync() {

            Video.CommentStatus = "取得中";
            var list = new List<NicoNicoCommentEntry>();

            //---ユーザーコメント取得---
            var root = new XmlDocument();
            var packet = root.CreateElement("packet");

            var thread = root.CreateElement("thread");
            thread.SetAttribute("thread", GetFlv.ThreadID);
            thread.SetAttribute("version", "20090904");
            thread.SetAttribute("user_id", GetFlv.UserId);
            thread.SetAttribute("userkey", GetFlv.UserKey);
            thread.SetAttribute("scores", "1");

            var leaves = root.CreateElement("thread_leaves");
            leaves.SetAttribute("thread", GetFlv.ThreadID);
            leaves.SetAttribute("user_id", GetFlv.UserId);
            leaves.SetAttribute("userkey", GetFlv.UserKey);
            leaves.SetAttribute("scores", "1");
            leaves.InnerText = "0-" + ((GetFlv.Length / 60) + 1) + ":100,1000";

            //公式動画だったらThreadkeyも取得する
            if(ApiData.IsOfficial) {

                try {
                    var query = new GetRequestQuery(GetThreadKeyApiUrl);
                    query.AddQuery("thread", GetFlv.ThreadID);
                    query.AddQuery("language_id", 0);

                    var threadKeyRaw = await NicoNicoWrapperMain.Session.GetAsync(query.TargetUrl);
                    var temp = threadKeyRaw.Split('&');
                    var threadKey = temp[0].Split('=')[1];
                    var force184 = temp[1].Split('=')[1];

                    thread.SetAttribute("threadkey", threadKey);
                    thread.SetAttribute("force_184", force184);
                    leaves.SetAttribute("threadkey", threadKey);
                    leaves.SetAttribute("force_184", force184);

                    thread.RemoveAttribute("userkey");
                    leaves.RemoveAttribute("userkey");

                } catch(RequestTimeout) {

                    Video.CommentStatus = "取得失敗";
                }
            }
            packet.AppendChild(thread);
            packet.AppendChild(leaves);
            root.AppendChild(packet);

            try {

                var request = new HttpRequestMessage(HttpMethod.Post, GetFlv.CommentServerUrl);
                request.Content = new StringContent(root.InnerXml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                var a = await NicoNicoWrapperMain.Session.GetAsync(request);

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var resultcode = doc.DocumentNode.SelectSingleNode("/packet/thread").Attributes["resultcode"].Value;

                if(resultcode == "11") {

                    Video.Status = "取得失敗（復帰中）";

                    var recv = new GetRequestQuery(GetFlv.CommentServerUrl + "thread");
                    recv.AddQuery("version", "20090904");
                    recv.AddQuery("thread", GetFlv.ThreadID);
                    recv.AddQuery("res_from", "-1000");

                    a = NicoNicoWrapperMain.Session.GetAsync(recv.TargetUrl).Result;
                    doc.LoadHtml2(a);
                }

                StoreEntry(doc, list);
                //------

                if(resultcode != "0") {

                    Video.CommentStatus = "取得失敗";
                    return null;
                }


                if(list.Count == 0 && resultcode == "0") {

                    Video.CommentStatus = "取得完了";
                    return list;
                }
                list.Sort();


                Ticket = doc.DocumentNode.SelectSingleNode("packet/thread").Attributes["ticket"].Value;
                LastRes = int.Parse(doc.DocumentNode.SelectSingleNode("packet/thread").Attributes["last_res"].Value);

                Video.CommentStatus = "取得完了";

                return list;
            } catch(RequestTimeout) {

                Video.CommentStatus = "取得失敗（タイムアウト）";
                return null;
            }
		}
        public async Task<List<NicoNicoCommentEntry>> GetUploaderCommentAsync() {

            try {
                
                var list = new List<NicoNicoCommentEntry>();


                Video.CommentStatus = "投稿者コメント取得中";

                //---投稿者コメント取得---
                var root = new XmlDocument();
                var packet = root.CreateElement("packet");
                var thread = root.CreateElement("thread");
                thread.SetAttribute("thread", GetFlv.ThreadID);
                thread.SetAttribute("version", "20090904");
                thread.SetAttribute("res_from", "-1000");
                thread.SetAttribute("fork", "1");

                packet.AppendChild(thread);
                root.AppendChild(packet);

                var request = new HttpRequestMessage(HttpMethod.Post, GetFlv.CommentServerUrl);
                request.Content = new StringContent(root.InnerXml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                
                var a = await NicoNicoWrapperMain.Session.GetAsync(request);
                
                var doc = new HtmlDocument();
                doc.LoadHtml2(a);
                
                StoreEntry(doc, list);
                //------

                if(list.Count == 0) {

                    Video.CommentStatus = "投稿者コメント取得失敗";
                    return null;
                }
                list.Sort();

                Video.CommentStatus = "投稿者コメント取得完了";

                return list;
            } catch(RequestTimeout) {

                Video.CommentStatus = "投稿者コメント取得失敗（タイムアウト）";
                return null;
            }

        }


        //コメントナンバーを返す
        public  string Post(string comment, string mail, string vpos) {

            Video.Status = "コメント投稿中";

            try {
                var query = new GetRequestQuery(GetPostKeyApiUrl);
                query.AddQuery("version_sub", "2");
                query.AddQuery("block_no", Math.Floor((decimal)(LastRes + 1) / 100).ToString());
                query.AddQuery("version", "1");
                query.AddQuery("yugi", "");
                query.AddQuery("device", "1");
                query.AddQuery("thread", GetFlv.ThreadID);

                var postkey = NicoNicoWrapperMain.Session.GetAsync(query.TargetUrl).Result;
                postkey = HttpUtility.UrlDecode(postkey).Split('=')[1];

                var root = new XmlDocument();
                var chat = root.CreateElement("chat");
                chat.SetAttribute("thread", GetFlv.ThreadID);
                chat.SetAttribute("vpos", vpos);
                chat.SetAttribute("mail", mail);
                chat.SetAttribute("ticket", Ticket);
                chat.SetAttribute("user_id", GetFlv.UserId);
                chat.SetAttribute("postkey", postkey);
                if(GetFlv.IsPremium) {

                    chat.SetAttribute("premium", "1");
                }
                chat.InnerText = comment;
                root.AppendChild(chat);

                var request = new HttpRequestMessage(HttpMethod.Post, GetFlv.CommentServerUrl);
                request.Content = new StringContent(root.InnerXml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var status = doc.DocumentNode.SelectSingleNode("packet/chat_result").Attributes["status"].Value;
                if(status != "0") {

                    Video.Status = "コメントの投稿に失敗しました";
                    return null;
                } else {

                    Video.Status = "コメントを投稿しました";
                    return doc.DocumentNode.SelectSingleNode("packet/chat_result").Attributes["no"].Value;
                }
            } catch(RequestTimeout) {

                Video.Status = "コメントの投稿に失敗しました（タイムアウト）";
                return null;
            }
        }



        private void StoreEntry(HtmlDocument doc, List<NicoNicoCommentEntry> list) {

            var nodes = doc.DocumentNode.SelectNodes("/packet/chat");

            if(nodes == null) {

                return;
            }

            foreach(var node in nodes) {

                var attr = node.Attributes;

                //削除されていたら登録しない もったいないしね
                if(attr.Contains("deleted")) {

                    continue;
                }

                var entry = new NicoNicoCommentEntry();
                
                entry.No = attr["no"].Value;
                entry.Vpos = attr["vpos"].Value;
                entry.RenderTime = NicoNicoUtil.GetTimeFromVpos(entry.Vpos);
                var unix = UnixTime.FromUnixTime(long.Parse(attr["date"].Value));

                entry.Date = unix.ToLongDateString() + " " + unix.ToLongTimeString();
                entry.UserId = attr.Contains("user_id") ? attr["user_id"].Value : "contributor";
                entry.Mail = attr.Contains("mail") ? attr["mail"].Value : "";
                entry.Content = HttpUtility.HtmlDecode(node.InnerText);
                entry.Score = attr.Contains("score") ? int.Parse(attr["score"].Value) : 0;
                
                if(!NicoNicoNGComment.Filter(entry)) {

                    list.Add(entry);
                }
            }
        }
	}


	public class NicoNicoCommentEntry : IComparable<NicoNicoCommentEntry> {

		//コメントナンバー
		public string No { get; set; }

		//コメント再生位置
		public string Vpos { get; set; }

        //コメント再生時間
        public string RenderTime { get; set; }

		//コマンド
		public string Mail { get; set; }

		//コメントを投稿したユーザーID
		public string UserId { get; set; }

		//コメント
		public string Content { get; set; }

		//投稿日時 Unixタイム
		public string Date { get; set; }

        //NGスコア
        public int Score { get; set; }
        
		//Vposでソートする
		public int CompareTo(NicoNicoCommentEntry obj) {

            if(int.Parse(Vpos) == int.Parse(obj.Vpos)) {

                return int.Parse(No).CompareTo(int.Parse(obj.No));
            }

			return int.Parse(Vpos).CompareTo(int.Parse(obj.Vpos));
		}

        public string ToJson() {

            return DynamicJson.Serialize(this);
        }

    }


}
