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

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Contents.Live
{
    /// <summary>
    /// MylistToolTip.xaml の相互作用ロジック
    /// </summary>
    public partial class LiveGate : UserControl {
        public LiveGate() {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is LiveGateViewModel) {

                var vm = (LiveGateViewModel)DataContext;

                vm.DescriptionBrowser = desc;

            }
        }

        private void desc_Navigating(object sender, NavigatingCancelEventArgs e) {

            if(e.Uri != null) {

                NicoNicoOpener.Open(e.Uri);
                e.Cancel = true;
            }
            ;
        }
    }
}
