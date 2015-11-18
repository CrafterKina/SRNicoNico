﻿using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// Video.xaml の相互作用ロジック
	/// </summary>
	public partial class Video : UserControl {
		public Video() {
			InitializeComponent();
            
		}

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is VideoViewModel) {

                VideoViewModel vm = (VideoViewModel)DataContext;
                vm.Video = this;
            }
        }


        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            NicoNicoOpener.Open(e.Uri.OriginalString);
        }

        public void InitializeToolTip(object sender, RoutedEventArgs e) {

            var link = sender as Hyperlink;
            var inline = link.Inlines.First() as Run;
            if(inline != null) {

                var text = link.NavigateUri.OriginalString;
                if(text.StartsWith("http://www.nicovideo.jp/watch/")) {

                    VideoToolTip tooltip = new VideoToolTip();
                    VideoDataViewModel vm = new VideoDataViewModel(text.Substring(30));
                    tooltip.DataContext = vm;
                    link.ToolTip = tooltip;

                    
                } else if(text.StartsWith("http://www.nicovideo.jp/user/")) {

                    UserToolTip tooltip = new UserToolTip();
                    UserDataViewModel vm = new UserDataViewModel(text.Substring(29));
                    tooltip.DataContext = vm;
                    link.ToolTip = tooltip;

                } else if(text.StartsWith("http://www.nicovideo.jp/mylist/")) {

                    MylistToolTip tooltip = new MylistToolTip();
                    MylistDataViewModel vm = new MylistDataViewModel(text.Substring(31));
                    tooltip.DataContext = vm;
                    link.ToolTip = tooltip;

                } else {

                    link.ToolTip = text;
                }
            }
        }
    }
}
