﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.ObjectModel;

using GongSolutions.Wpf.DragDrop;

namespace SRNicoNico.ViewModels {
    public class MylistListViewModel : TabItemViewModel, IDragSource  {

        #region Mylist変更通知プロパティ
        private ObservableSynchronizedCollection<MylistListEntryViewModel> _Mylist;

        public ObservableSynchronizedCollection<MylistListEntryViewModel> Mylist {
            get { return _Mylist; }
            set { 
                if(_Mylist == value)
                    return;
                _Mylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedItem変更通知プロパティ
        private MylistListEntryViewModel _SelectedItem;

        public MylistListEntryViewModel SelectedItem {
            get { return _SelectedItem; }
            set {
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SortIndex変更通知プロパティ
        private int _SortIndex;

        public int SortIndex {
            get { return _SortIndex; }
            set { 
                if(_SortIndex == value)
                    return;
                _SortIndex = value;
                Sort(value);
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if(_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region EditMode変更通知プロパティ
        private bool _EditMode;

        public bool EditMode {
            get { return _EditMode; }
            set { 
                if(_EditMode == value)
                    return;
                _EditMode = value;
                if(value) {

                    Group.BeforeName = Group.Name;
                    Group.BeforeDescription = Group.Description;
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        //リスト情報
        public NicoNicoMylistGroupData Group { get; private set; }

        //オーナー
        public MylistViewModel Owner { get; private set; }

        //エディットモード時
        public MylistEditModeViewModel EditModeViewModel { get; set; }

        public MylistListViewModel(MylistViewModel vm, NicoNicoMylistGroupData group, List<NicoNicoMylistData> list) : base(group.Name) {

            EditModeViewModel = new MylistEditModeViewModel(this);
            Owner = vm;
            Group = group;
            Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>();
            foreach(NicoNicoMylistData data in list) {

                Mylist.Add(new MylistListEntryViewModel(this, data));
            }
            Sort(0);

        }

        /*
            <ComboBoxItem Content="登録が新しい順" />
            <ComboBoxItem Content="登録が古い順" />
            <ComboBoxItem Content="タイトル昇順" />
            <ComboBoxItem Content="タイトル降順" />
            <ComboBoxItem Content="マイリストコメント昇順" />
            <ComboBoxItem Content="マイリストコメント降順" />
            <ComboBoxItem Content="投稿が新しい順" />
            <ComboBoxItem Content="投稿が古い順" />
            <ComboBoxItem Content="再生数が多い順" />
            <ComboBoxItem Content="再生数が少ない順" />
            <ComboBoxItem Content="コメントが新しい順" />
            <ComboBoxItem Content="コメントが古い順" />
            <ComboBoxItem Content="コメントが多い順" />
            <ComboBoxItem Content="コメントが少ない順" />
            <ComboBoxItem Content="マイリスト登録が多い順" />
            <ComboBoxItem Content="マイリスト登録が少ない順" />
        */
        //ソート
        public void Sort(int index) {

            switch(index) {
                case 0:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(entry => entry.Entry.CreateTime));
                    break;
                case 1:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.CreateTime));
                    break;
                case 2:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.Title));
                    break;
                case 3:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(r => r.Entry.Title));
                    break;
                case 4:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.Description));
                    break;
                case 5:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(r => r.Entry.Description));
                    break;
                case 6:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(entry => entry.Entry.FirstRetrieve));
                    break;
                case 7:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.FirstRetrieve));
                    break;
                case 8:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(entry => entry.Entry.ViewCounter));
                    break;
                case 9:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.ViewCounter));
                    break;
                case 10:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(entry => entry.Entry.UpdateTime));
                    break;
                case 11:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.UpdateTime));
                    break;
                case 12:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(entry => entry.Entry.CommentCounter));
                    break;
                case 13:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.CommentCounter));
                    break;
                case 14:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderByDescending(entry => entry.Entry.MylistCounter));
                    break;
                case 15:
                    Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>(Mylist.OrderBy(r => r.Entry.MylistCounter));
                    break;
            }


        }

        //選択したマイリストを開く
        public void Open() {

            if(SelectedItem != null) {

                if(SelectedItem.Entry.Type == 0) {

                    new VideoViewModel("http://www.nicovideo.jp/watch/" + SelectedItem.Entry.Id);
                }
                SelectedItem = null;
            }
        }

        //更新
        public void Reflesh() {

            IsActive = true;
            Mylist.Clear();

            Task.Run(() => {

                Mylist = new ObservableSynchronizedCollection<MylistListEntryViewModel>();

                foreach(NicoNicoMylistData data in MylistViewModel.MylistInstance.GetMylist(Group.Id)) {

                    Mylist.Add(new MylistListEntryViewModel(this, data));
                }

                //エディットモードの情報をクリア
                EditModeViewModel.AllSelect = false;
                EditModeViewModel.IsAnyoneChecked = false;
                IsActive = false;
            });
        }

        //マイリスト削除ダイアログ表示
        public void ShowDeleteDialog() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Mylist.DeleteMylistDialog), this, TransitionMode.Modal));
        }

        //マイリスト削除
        public void DeleteMylist() {

            Owner.Status = Group.Name + " を削除しています";
            Task.Run(() => {

                MylistViewModel.MylistInstance.DeleteMylistGroup(Group.Id);
                CloseDialog();
                Owner.Reflesh();
                Owner.Status = Group.Name + " を削除しました";

            });
        }

        //ドラッグ開始
        void IDragSource.StartDrag(IDragInfo dragInfo) {

            dragInfo.Data = SelectedItem;
            dragInfo.Effects = System.Windows.DragDropEffects.All;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo) {

            return true;
        }

        void IDragSource.Dropped(IDropInfo dropInfo) {

        }

        void IDragSource.DragCancelled() {

            SelectedItem = null;
            Owner.Status = "";
        }
    }
}
