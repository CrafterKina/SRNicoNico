﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Models.NicoNicoViewer;
using System.Threading;

namespace SRNicoNico.ViewModels {

	public class LiveCommentViewModel : ViewModel {


        #region IsPopupOpen変更通知プロパティ
        private bool _IsPopupOpen;

        public bool IsPopupOpen {
            get { return _IsPopupOpen; }
            set { 
                if(_IsPopupOpen == value)
                    return;
                _IsPopupOpen = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public bool IsPremium {
            get {
                return NicoNicoWrapperMain.Session.Authority == NiconicoAccountAuthority.Premium;
            }
        }

        #region AcceptEnter変更通知プロパティ
        private bool _AcceptEnter;

        public bool AcceptEnter {
            get { return _AcceptEnter; }
            set { 
                if(_AcceptEnter == value)
                    return;
                _AcceptEnter = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CanComment変更通知プロパティ
        private bool _CanComment = false;

        public bool CanComment {
            get { return _CanComment; }
            set { 
                if(_CanComment == value)
                    return;
                _CanComment = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsTextBoxEnabled変更通知プロパティ
        private bool _IsTextBoxEnabled = true;

        public bool IsTextBoxEnabled {
            get { return _IsTextBoxEnabled; }
            set { 
                if(_IsTextBoxEnabled == value)
                    return;
                _IsTextBoxEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Mail変更通知プロパティ
        private string _Mail = "";

        public string Mail {
            get { return _Mail; }
            set { 
                if(_Mail == value)
                    return;
                _Mail = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region Size変更通知プロパティ
        private CommentSize _Size = CommentSize.Medium;

        public CommentSize Size {
            get { return _Size; }
            set { 
                if(_Size == value)
                    return;
                _Size = value;
                UpdateMail();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Position変更通知プロパティ
        private CommentPosition _Position = CommentPosition.Naka;

        public CommentPosition Position {
            get { return _Position; }
            set { 
                if(_Position == value)
                    return;
                _Position = value;
                UpdateMail();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Color変更通知プロパティ
        private string _Color = "";

        public string Color {
            get { return _Color; }
            set { 
                if(_Color == value)
                    return;
                _Color = value;
                UpdateMail();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Text変更通知プロパティ
        private string _Text = "";

        public string Text {
            get { return _Text; }
            set { 
                if(_Text == value)
                    return;
                _Text = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //位置
        public string Vpos { get; set; }

        private LiveWatchViewModel Owner;

        public LiveCommentViewModel(LiveWatchViewModel vm) {

            Owner = vm;
		}

        public void UpdateMail() {

            Mail = Size.ToString().ToLower() + " " + Position.ToString().ToLower();
            if(Color != "white") {

                Mail += " " + Color;
            }

            //省略できるやつはする
            Mail = Mail.Replace("medium", "").Replace("naka", "");
        }

        public void Post() {
            
            if(Text.Length == 0 || !IsTextBoxEnabled) {

                return;
            }

            IsTextBoxEnabled = false;
            Task.Run(() => {

                var mail = Mail;

                //公式動画は184を付けて投稿出来ない
                if(Settings.Instance.Use184) {
                    
                    mail += " 184";
                }
                var no = "";// Owner.CommentInstance.Post(Text, mail, Vpos);

                if(no != null) {

                    var entry = new NicoNicoCommentEntry();
                    entry.No = no;
                    entry.Mail = Mail;
                    entry.Vpos = Vpos;
                    entry.Content = Text;

                   // Owner.Proxy.Call("AsInjectMyComment", entry.ToJson());
                    Text = "";
                   
                }
                IsTextBoxEnabled = true;
            });
        }
	}
}
