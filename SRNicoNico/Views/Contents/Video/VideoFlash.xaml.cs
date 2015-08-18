﻿using System;
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
using System.Windows.Controls.Primitives;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoFlash.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoFlash : UserControl {
        
        public VideoFlash() {
            InitializeComponent();



            browser.Source = new Uri(App.ViewModelRoot.CurrentVideo.Address);
        }




        private void browser_LoadCompleted(object sender, NavigationEventArgs e) {


            //インスタンスを設定
            App.ViewModelRoot.CurrentVideo.WebBrowser = browser;

            //動画オープン
            App.ViewModelRoot.CurrentVideo.OpenVideo();
        }
    }
}
