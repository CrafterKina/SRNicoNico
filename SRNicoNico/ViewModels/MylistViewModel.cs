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

namespace SRNicoNico.ViewModels {
    public class MylistViewModel : TabItemViewModel {




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




        #region NicoRepoListCollection変更通知プロパティ
        private ObservableCollection<MylistListViewModel> _MylistListCollection = new ObservableCollection<MylistListViewModel>();

        public ObservableCollection<MylistListViewModel> MylistListCollection {
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

        public MylistViewModel() : base("マイリスト") {



        }



        public void Reflesh() {

            IsActive = true;


            MylistListCollection.Clear();

            MylistListCollection.Add(new MylistListViewModel("とりあえずマイリスト", 0));
            IsActive = false;


        }


    }
}
