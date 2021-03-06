﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading.Tasks;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class MylistViewModel : TabItemViewModel, IDropTarget {

        //バックエンドインスタンス
        public static NicoNicoMylist MylistInstance = new NicoNicoMylist();
        public static NicoNicoMylistGroup MylistGroupInstance = new NicoNicoMylistGroup();
        
        #region NicoRepoListCollection変更通知プロパティ
        private DispatcherCollection<MylistListViewModel> _MylistListCollection = new DispatcherCollection<MylistListViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistListViewModel> MylistListCollection {
            get { return _MylistListCollection; }
            set {
                if(_MylistListCollection == value)
                    return;
                _MylistListCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedList変更通知プロパティ
        private MylistListViewModel _SelectedList;

        public MylistListViewModel SelectedList {
            get { return _SelectedList; }
            set {
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public MylistViewModel() : base("マイリスト") { }

        //マイリスト作成
        public void ShowAddMylistDialog() {

            NewMylistViewModel newlist = new NewMylistViewModel(this);
            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Mylist.NewMylistDialog), newlist, TransitionMode.Modal));
        }

        public void Refresh() {

            IsActive = true;

            Task.Run(() => {

                MylistListCollection.Clear();

                Status = "マイリスト取得中(とりあえずマイリスト)";
                NicoNicoMylistGroupData deflist = new NicoNicoMylistGroupData();
                deflist.Name = "とりあえずマイリスト";
                deflist.Description = "";
                deflist.Id = "0";

                MylistListCollection.Add(new MylistListViewModel(this, deflist, MylistInstance.GetDefMylist()));

                foreach(NicoNicoMylistGroupData group in MylistGroupInstance.GetMylistGroup()) {

                    Status = "マイリスト取得中(" + group.Name + ")";
                    MylistListCollection.Add(new MylistListViewModel(this, group, MylistInstance.GetMylist(group.Id)));
                }
                Status = "マイリスト取得完了";


                IsActive = false;
            });
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                if(e.KeyboardDevice.Modifiers == ModifierKeys.Control || e.KeyboardDevice.Modifiers == ModifierKeys.Shift) {

                    Refresh();
                } else if(SelectedList != null) {

                    SelectedList.Refresh();
                } else {

                    Refresh();
                }
            }
        }

        //マイリストをドラッグしてきた時
        void IDropTarget.DragOver(IDropInfo dropInfo) {

            if(dropInfo.TargetItem is MylistListViewModel) {

                var target = dropInfo.TargetItem as MylistListViewModel;
                var item = dropInfo.Data as MylistListEntryViewModel;

                if(SelectedList.Group.Id == target.Group.Id) {

                    return;
                }

                Status = item.Entry.Title + " を " + item.Owner.Name + " から " + target.Name + " に移動します";

                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        //マイリストをドロップ
        void IDropTarget.Drop(IDropInfo dropInfo) {

            if(dropInfo.TargetItem is MylistListViewModel) {

                var target = dropInfo.TargetItem as MylistListViewModel;
                var item = dropInfo.Data as MylistListEntryViewModel;

                if(SelectedList.Group.Id == target.Group.Id) {

                    return;
                }

                target.SelectedItem = null;
                Status = item.Entry.Title + " を " + item.Owner.Name + " から " + target.Name + " に移動しています";

                Task.Run(() => {

                    MylistInstance.MoveMylist(item, target);
                    Status = item.Entry.Title + " を " + item.Owner.Name + " から " + target.Name + " に移動しました";
                    target.Refresh();
                    item.Owner.Refresh();
                });
            }
        }
    }
}
