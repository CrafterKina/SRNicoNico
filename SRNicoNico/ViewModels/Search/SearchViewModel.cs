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

using System.Windows.Controls;


using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class SearchViewModel : ViewModel {

        //ソート方法
        private string[] sort_by = { "f:d", "f:a",
                                     "v:d", "v:a",
                                     "n:d", "n:a",
                                     "m:d", "m:a",
                                     "l:d", "l:a"
                                    };

        #region SearchText変更通知プロパティ
        private string _SearchText;

        public string SearchText {
            get { return _SearchText; }
            set {
                if (_SearchText == value)
                    return;
                _SearchText = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedIndex変更通知プロパティ
        private int _SelectedIndex = 2;

        public int SelectedIndex {
            get { return _SelectedIndex; }
            set {
                if (_SelectedIndex == value)
                    return;
                _SelectedIndex = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private NicoNicoSearch currentSearch;

        //検索ボタン押下
        public void DoSearch() {


			if (SearchText == null || SearchText.Length == 0) {

				return;
			}

            App.ViewModelRoot.SearchResult.OwnerViewModel = this;

            App.ViewModelRoot.SearchResult.IsActive = true;
            App.ViewModelRoot.RightContent = App.ViewModelRoot.SearchResult;

            //検索
            currentSearch = new NicoNicoSearch(SearchText, sort_by[SelectedIndex]);

			Task.Run(() => {

                currentSearch.Search();
                App.ViewModelRoot.SearchResult.IsActive = false;
            });
		}

		public void SearchNext() {

            App.ViewModelRoot.SearchResult.IsActive = true;
            Task.Run(() => {

				currentSearch.SearchNext();
                App.ViewModelRoot.SearchResult.IsActive = false;
            });
        }
    }
}
