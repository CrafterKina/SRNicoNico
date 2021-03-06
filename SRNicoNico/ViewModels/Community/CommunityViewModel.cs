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
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace SRNicoNico.ViewModels {
    public class CommunityViewModel : TabItemViewModel {

        private NicoNicoCommunity Community;


        #region Content変更通知プロパティ
        private NicoNicoCommunityContent _Content;

        public NicoNicoCommunityContent Content {
            get { return _Content; }
            set { 
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region TabItems変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _TabItems = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> TabItems {
            get { return _TabItems; }
            set { 
                if(_TabItems == value)
                    return;
                _TabItems = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region SelectedTab変更通知プロパティ
        private TabItemViewModel _SelectedTab;

        public TabItemViewModel SelectedTab {
            get { return _SelectedTab; }
            set {
                if(_SelectedTab == value)
                    return;
                _SelectedTab = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly string CommunityUrl;

        public CommunityViewModel(string url) : base("読込中") {

            CommunityUrl = url;


            Initialize();
        }

        public void Initialize() {

            TabItems.Clear();

            TabItems.Add(new CommunityTopViewModel(this));
            Community = new NicoNicoCommunity(CommunityUrl);

            IsActive = true;
            Status = "コミュニティ情報を取得中";

            Task.Run(() => {


                Content = Community.GetCommunity();
                IsActive = false;
                Status = "";

                Name = Content.CommunityTitle;
            });
        }


        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                Close();
            } else if(e.Key == Key.F5) {

                Refresh();
            }
        }


        public void OpenBrowser() {

            System.Diagnostics.Process.Start(Content.CommunityUrl);
        }

        public void Close() {

            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }

        public void Refresh() {

            Initialize();
        }




    }
}
