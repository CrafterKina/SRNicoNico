﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using SRNicoNico.Views.Contents.Live;
using AxShockwaveFlashObjects;
using Flash.External;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Views.Contents.Video;
using Codeplex.Data;

namespace SRNicoNico.ViewModels {
    public class VideoFlashHandler : IObjectForScriptable {


        public WebBrowser Browser;


        private VideoViewModel Owner;


        public VideoFlashHandler(VideoViewModel vm)  {

            Owner = vm;
        }

        private VideoData VideoData;
        

        public async void Initialize(WebBrowser browser,  VideoData videoData) {

            Browser = browser;
            //ロードに失敗したら
            if(videoData.ApiData == null) {

                Owner.LoadFailed = true;
                Owner.IsActive = false;
                Owner.Status = "動画の読み込みに失敗しました。";
                return;

            }
            VideoData = videoData;


            Owner.Name = VideoData.ApiData.Title;

            //有料動画なら
            if(VideoData.ApiData.IsPaidVideo) {

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(PaidVideoDialog) , Owner, TransitionMode.Modal));
                return;
            }

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {


                if(VideoData.ApiData.Cmsid.Contains("nm")) {

                    VideoData.VideoType = NicoNicoVideoType.SWF;
                    Browser.Source = new Uri(GetNMPlayerPath());

                } else if(VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp")) {

                    VideoData.VideoType = NicoNicoVideoType.RTMP;
                    Browser.Source = new Uri(GetRTMPPlayerPath());
                } else {

                    if(VideoData.ApiData.MovieType == "flv") {

                        VideoData.VideoType = NicoNicoVideoType.FLV;
                    } else {

                        VideoData.VideoType = NicoNicoVideoType.MP4;
                    }
                    Browser.Source = new Uri(GetPlayerPath());
                }
                Owner.IsActive = false;


            }));
            //動画時間
            Owner.Time.VideoTimeString = NicoNicoUtil.ConvertTime(VideoData.ApiData.Length);
            Owner.Time.Length = VideoData.ApiData.Length;

            if(VideoData.ApiData.GetFlv.IsPremium && !VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp")) {

                    Owner.StoryBoardStatus = "取得中";

                    var sb = new NicoNicoStoryBoard(VideoData.ApiData.GetFlv.VideoUrl);
                    VideoData.StoryBoardData = sb.GetStoryBoardData();

                    if(VideoData.StoryBoardData == null) {

                        Owner.StoryBoardStatus = "データ無し";
                    } else {

                        Owner.StoryBoardStatus = "取得完了";
                    }
            } else {

                Owner.StoryBoardStatus = "データ無し";
            }
            
            Owner.CommentInstance = new NicoNicoComment(VideoData.ApiData, Owner);


            await Browser.Dispatcher.BeginInvoke((Action)(() => {

                browser.ObjectForScripting = new ObjectForScripting(this);
                Browser.InvokeScript("init", VideoData.ApiData.GetFlv.VideoUrl);

            }));
            InitializeComment();

        }

        private async void InitializeComment() {

            Owner.Comment.IsCommentLoading = true;

            var list = await Owner.CommentInstance.GetCommentAsync();
            if(list != null) {


                foreach(var entry in list) {

                    VideoData.CommentData.Add(new CommentEntryViewModel(entry, Owner));
                }

                dynamic json = new DynamicJson();
                json.array = list;

                InjectComment(json.ToString());
                Owner.Comment.CanComment = true;
                Owner.Comment.IsCommentLoading = false;

                //投稿者コメントがあったら取得する
                if(VideoData.ApiData.HasOwnerThread) {

                    var ulist = await Owner.CommentInstance.GetUploaderCommentAsync();
                    dynamic ujson = new DynamicJson();
                    json.array = ulist;

                    InjectUploaderComment(json.ToString());
                }

            }

            if(!Settings.Instance.CommentVisibility) {

                //InvokeScript("AsToggleComment");
            } else {

                Owner.CommentVisibility = true;
            }
        }

        public void ReloadComment() {

           // InvokeScript("PurgeComment");
            VideoData.CommentData.Clear();
            InitializeComment();
        }



        private void OpenVideo() {

            //ここからInvoke可能
            Owner.IsPlaying = true;
            Owner.IsInitialized = true;
            Owner.Mylist.EnableButtons();

            //RTMPの時はサーバートークンも一緒にFlashに渡す
            if(VideoData.VideoType == NicoNicoVideoType.RTMP) {

                //InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl + "^" + VideoData.ApiData.GetFlv.FmsToken, App.ViewModelRoot.Config.Comment.ToJson());
            } else {

                //InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl, App.ViewModelRoot.Config.Comment.ToJson());
            }

            Owner.IsRepeat = Settings.Instance.IsRepeat;
        }

        public void DisposeHandler() {

            Browser.Dispose();
        }


        //ASを呼ぶ
        public void InvokeScript(string func, params string[] args) {

            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
            if(Browser != null) {

                try {
                    
                   Browser.Dispatcher.BeginInvoke((Action)(()=> Browser.InvokeScript(func, args)));
                } catch(COMException) {

                    Console.WriteLine("COMException：" + func);
                }
            }
        }
        

        private object ExternalInterfaceHandler(object sender, ExternalInterfaceCallEventArgs e) {

            InvokeFromActionScript(e.FunctionCall.FunctionName, e.FunctionCall.Arguments);
            return false;
        }
        //ExternalIntarface.callでActionscriptから呼ばれる
        public void InvokeFromActionScript(string func, params string[] args) {
            
            switch(func) {
                case "Ready":
                    OpenVideo();
                    break;
                case "CsFrame": //毎フレーム呼ばれる
                    CsFrame(float.Parse(args[0]), float.Parse(args[1]), long.Parse(args[2]), args[3]);
                    break;
                case "NetConnection.Connect.Closed":    //RTMP動画再生時にタイムアウトになったら
                    Owner.RTMPTimeOut();
                    break;
                case "ShowController":  //マウスを動かしたら呼ばれる
                    Owner.ShowFullScreenPopup();
                    break;
                case "HideController":  //マウスを数秒動画の上で静止させたら呼ばれる
                    Owner.HideFullScreenPopup();
                    break;
                case "Initialized": //動画が再生される直前に呼ばれる
                    int vol = Settings.Instance.Volume;
                    Owner.Volume = 0;    //保存された値をFlash側に伝える
                    Owner.Volume = vol;    //保存された値をFlash側に伝える
                    break;
                case "WidthHeight":
                    Owner.VideoData.Resolution = args[0];
                    break;
                case "Bitrate":
                    //Owner.VideoData.BitRate = args[0];
                    break;
                case "Framerate":
                    Owner.VideoData.FrameRate = args[0];
                    break;
                case "FileSize":
                    var size = double.Parse(args[0]);
                    size /= 100000.0;
                    size = Math.Floor(size) / 10;

                    Owner.VideoData.FileSize = size.ToString() + "MB";
                    break;
                case "Stop": //動画が最後まで行ったらリピートしたりフルスクリーンから復帰したりする
                    if(Owner.IsRepeat) {

                        Restart();
                    } else if(Owner.IsFullScreen) {

                        if(Owner.IsPlayList) {

                            Owner.PlayListEntry.Owner.Next();
                        } else {

                            Owner.ReturnFromFullScreen();
                        }
                    } else {

                        if(Owner.IsPlayList) {

                            Owner.PlayListEntry.Owner.Next();
                            return;
                        }
                        Owner.IsPlaying = false;


                    } 

                    break;
                case "Click":   //Flash部分がクリックされた時に呼ばれる
                    if(Settings.Instance.ClickOnPause) {   //クリックしたら一時停止する設定になっていたら

                        TogglePlay();
                    }
                    break;
                case "MouseWheel":
                    Console.WriteLine("Wheel:" + args[0]);
                    Owner.Volume += int.Parse(args[0]);
                    break;
                default:
                    Console.WriteLine("Invoked From Actionscript:" + func);
                    break;
            }
        }

        private int prevTime;
        private double sumBPS;
        //1フレーム毎に呼ばれる
        public void CsFrame(float time, float buffer, long bps, string vpos) {

            if(prevTime != (int)time) {

                double comp = sumBPS / 1024;

                //大きいから単位を変える
                if(comp > 1000) {

                    Owner.BPS = Math.Floor((comp / 1024) * 100) / 100 + "MiB/秒";
                } else {

                    Owner.BPS = Math.Floor(comp * 100) / 100 + "KiB/秒";
                }
                sumBPS = 0;
            } else {

                sumBPS += bps;
            }
            prevTime = (int)time;

            Owner.Time.BufferedTime = buffer;
            Owner.Comment.Vpos = vpos;

            //Console.WriteLine(VideoData.ApiData.Cmsid + " time:" + time + " buffer:" + buffer + " vpos:" + vpos);

            Owner.SetSeekCursor(time);
        }


        //最初から
        public void Restart() {

            Seek(0);
        }
        public void Pause() {
            
            Owner.IsPlaying = false;
           // InvokeScript("AsPause");
        }

        public void Resume() {

            Owner.IsPlaying = true;
           // InvokeScript("AsResume");
        }

        //一時停止切り替え
        public void TogglePlay() {

            if(Owner.IsPlaying) {

                Pause();
            } else {

                Resume();
            }
        }

        //コメント設定をFlashに反映させる
        public void ApplyChanges() {

           // InvokeScript("AsApplyChanges", App.ViewModelRoot.Config.Comment.ToJson());
        }


        //Flashにシーク命令を送る
        public void Seek(float pos) {

           InvokeScript("seek", pos.ToString());
        }

        //Flashにコメントリストを送る
        public void InjectComment(string json) {

          //  InvokeScript("AsInjectComment", json);
        }

        //Flashに投稿者コメントリストを送る
        public void InjectUploaderComment(string json) {

           // InvokeScript("AsInjectUploaderComment", json);
        }
        


        private string GetPlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return "file://127.0.0.1/e$/Development/MVVM/WPF/SRNicoNico/SRNicoNico/Html5/videoflash.html";//cur + "./Html5/videoflash.html";
        }

        private string GetNMPlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return cur + "./Flash/NicoNicoNMPlayer.swf";
        }

        private string GetRTMPPlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return cur + "./Flash/NicoNicoRTMPPlayer.swf";
        }

        public void Invoked(string cmd, string args) {


            Console.WriteLine("Invoked " + cmd + " : " + args);


        }
    }
}
